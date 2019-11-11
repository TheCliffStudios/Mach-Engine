using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using com.ootii.Helpers;
using com.ootii.Input;

[CanEditMultipleObjects]
[CustomEditor(typeof(EasyInputSource))]
public class EasyInputSourceEditor : Editor
{
    // Helps us keep track of when the list needs to be saved. This
    // is important since some changes happen in scene.
    private bool mIsDirty;

    // The actual class we're storing
    private EasyInputSource mTarget;
    private SerializedObject mTargetSO;

    // Activators that can be selected
    //private int[] mViewActivatorIDs = new int[] { 0, 1, 2, 3 };
    private string[] mActivators = new string[] { "None", "Left Mouse Button", "Right Mouse Button", "Left or Right Mouse Button"};

    // List object for our layer motions
    private ReorderableList mAliasList;

    int[] mEnumIDs = null;
    string[] mEnumNames = null;

    /// <summary>
    /// Called when the object is selected in the editor
    /// </summary>
    private void OnEnable()
    {
        // Grab the serialized objects
        mTarget = (EasyInputSource)target;
        mTargetSO = new SerializedObject(target);

        mEnumIDs = EnumInput.EnumNames.Keys.ToArray<int>();
        mEnumNames = EnumInput.EnumNames.Values.ToArray<string>();

        InstanciateAliasList();
    }

    /// <summary>
    /// This function is called when the scriptable object goes out of scope.
    /// </summary>
    private void OnDisable()
    {
    }

    /// <summary>
    /// Called when the inspector needs to draw
    /// </summary>
    public override void OnInspectorGUI()
    {
        // Pulls variables from runtime so we have the latest values.
        mTargetSO.Update();

        GUILayout.Space(5);

        EditorHelper.DrawInspectorTitle("ootii Easy Input Source");

        EditorHelper.DrawInspectorDescription("Input solution that leverages ootii's advanced 'Easy Input' asset. There should only be one per scene.", MessageType.None);

        GUILayout.Space(5);

        if (!InputManagerHelper.IsDefined("WXLeftStickX"))
        {
            Color lGUIColor = GUI.color;
            GUI.color = new Color(1f, 0.7f, 0.7f, 1f);
            EditorGUILayout.HelpBox("Required Unity Input Manager entries not created. To create the entries, press the 'Reset Input Entries' button below.", MessageType.Warning);
            GUI.color = lGUIColor;

            GUILayout.Space(5);
        }

        EditorGUILayout.BeginVertical(EditorHelper.Box);

        EditorGUILayout.HelpBox("Reset required Unity Input Manager entries.", MessageType.None);

        if (GUILayout.Button("Reset Input Entries", EditorStyles.miniButton))
        {
            ClearInputManagerEntries();
            CreateInputManagerEntries();
        }

        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        bool lNewIsPlayerInputEnabled = EditorGUILayout.Toggle(new GUIContent("Is Input Enabled", "Determines if this component will return user input. This is NOT a global setting and does NOT effect the static 'Input Manager' from returning input."), mTarget.IsEnabled);
        if (lNewIsPlayerInputEnabled != mTarget.IsEnabled)
        {
            mIsDirty = true;
            mTarget.IsEnabled = lNewIsPlayerInputEnabled;
        }

        bool lNewIsXboxControllerEnabled = EditorGUILayout.Toggle(new GUIContent("Is Xbox Enabled", "Determines we can use the Xbox controller for input. This is a global setting that effects the static 'Input Manager'."), mTarget.IsXboxControllerEnabled);
        if (lNewIsXboxControllerEnabled != mTarget.IsXboxControllerEnabled)
        {
            mIsDirty = true;
            mTarget.IsXboxControllerEnabled = lNewIsXboxControllerEnabled;

            // Ensure our input manager entries exist
            if (mTarget.IsXboxControllerEnabled)
            {
                CreateInputManagerEntries();
            }
        }

        GUILayout.Space(5);

        float lNewViewSensativity = EditorGUILayout.FloatField(new GUIContent("View Sensativity", "Sets how quickly the view rotates (while using the mouse)"), mTarget.ViewSensativity);
        if (lNewViewSensativity != mTarget.ViewSensativity)
        {
            mIsDirty = true;
            mTarget.ViewSensativity = lNewViewSensativity;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("View Activator", "Determines what button enables viewing."), GUILayout.Width(EditorGUIUtility.labelWidth));
        int lNewViewActivator = EditorGUILayout.Popup(mTarget.ViewActivator, mActivators);
        if (lNewViewActivator != mTarget.ViewActivator)
        {
            mIsDirty = true;
            mTarget.ViewActivator = lNewViewActivator;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        EditorHelper.DrawLine();

        GUILayout.Space(5f);

        // Show the aliases
        GUILayout.BeginVertical(EditorHelper.GroupBox);
        mAliasList.DoLayoutList();
        GUILayout.EndVertical();

        if (mAliasList.index >= 0)
        {
            EditorGUILayout.BeginVertical(EditorHelper.Box);

            InputAlias lAlias = mTarget.DefaultActionAliases[mAliasList.index];

            bool lListIsDirty = DrawAliasDetailItem(ref lAlias);
            if (lListIsDirty)
            {
                mIsDirty = true;
                mTarget.DefaultActionAliases[mAliasList.index] = lAlias;
            }

            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(5);

        // If there is a change... update.
        if (mIsDirty)
        {
            // Flag the object as needing to be saved
            EditorUtility.SetDirty(mTarget);

#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            EditorApplication.MarkSceneDirty();
#else
            if (!EditorApplication.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
#endif

            // Pushes the values back to the runtime so it has the changes
            mTargetSO.ApplyModifiedProperties();

            // Clear out the dirty flag
            mIsDirty = false;
        }
    }

    /// <summary>
    /// Create the reorderable list
    /// </summary>
    private void InstanciateAliasList()
    {
        SerializedProperty lAliases = mTargetSO.FindProperty("DefaultActionAliases");
        mAliasList = new ReorderableList(mTargetSO, lAliases, true, true, true, true);

        mAliasList.drawHeaderCallback = DrawAliasListHeader;
        mAliasList.drawFooterCallback = DrawAliasListFooter;
        mAliasList.drawElementCallback = DrawAliasListItem;
        mAliasList.onAddCallback = OnAliasListItemAdd;
        mAliasList.onRemoveCallback = OnAliasListItemRemove;
        mAliasList.onSelectCallback = OnAliasListItemSelect;
        mAliasList.onReorderCallback = OnAliasListReorder;
        mAliasList.footerHeight = 17f;

        if (mTarget.EditorActionAliasIndex < mAliasList.count)
        {
            mAliasList.index = mTarget.EditorActionAliasIndex;
            OnAliasListItemSelect(mAliasList);
        }
    }

    /// <summary>
    /// Header for the list
    /// </summary>
    /// <param name="rRect"></param>
    private void DrawAliasListHeader(Rect rRect)
    {
        EditorGUI.LabelField(rRect, "Action Aliases");

        if (GUI.Button(rRect, "", EditorStyles.label))
        {
            mAliasList.index = -1;
            OnAliasListItemSelect(mAliasList);
        }
    }

    /// <summary>
    /// Allows us to draw each item in the list
    /// </summary>
    /// <param name="rRect"></param>
    /// <param name="rIndex"></param>
    /// <param name="rIsActive"></param>
    /// <param name="rIsFocused"></param>
    private void DrawAliasListItem(Rect rRect, int rIndex, bool rIsActive, bool rIsFocused)
    {
        if (rIndex < mTarget.DefaultActionAliases.Count)
        {
            Rect lNameRect = new Rect(rRect.x, rRect.y + 1, rRect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(lNameRect, mTarget.DefaultActionAliases[rIndex].Name);
        }
    }

    /// <summary>
    /// Footer for the list
    /// </summary>
    /// <param name="rRect"></param>
    private void DrawAliasListFooter(Rect rRect)
    {
        Rect lAddRect = new Rect(rRect.x + rRect.width - 28 - 28, rRect.y + 1, 28, 15);
        if (GUI.Button(lAddRect, new GUIContent("+", "Add Action Alias."), EditorStyles.miniButtonLeft)) { OnAliasListItemAdd(mAliasList); }

        Rect lDeleteRect = new Rect(lAddRect.x + lAddRect.width, lAddRect.y, 28, 15);
        if (GUI.Button(lDeleteRect, new GUIContent("-", "Delete Action Alias."), EditorStyles.miniButtonRight)) { OnAliasListItemRemove(mAliasList); };
    }

    /// <summary>
    /// Allows us to add to a list
    /// </summary>
    /// <param name="rList"></param>
    private void OnAliasListItemAdd(ReorderableList rList)
    {
        InputAlias lAlias = Activator.CreateInstance<InputAlias>();
        lAlias.PrimaryID = -1;
        lAlias.SupportID = -1;
        lAlias.IsDoublePress = false;

        mTarget.DefaultActionAliases.Add(lAlias);

        mAliasList.index = mTarget.DefaultActionAliases.Count - 1;
        OnAliasListItemSelect(rList);

        mIsDirty = true;
    }

    /// <summary>
    /// Allows us process when a list is selected
    /// </summary>
    /// <param name="rList"></param>
    private void OnAliasListItemSelect(ReorderableList rList)
    {
        mTarget.EditorActionAliasIndex = rList.index;
        if (mTarget.EditorActionAliasIndex == -1) { return; }
    }

    /// <summary>
    /// Allows us to stop before removing the item
    /// </summary>
    /// <param name="rList"></param>
    private void OnAliasListItemRemove(ReorderableList rList)
    {
        if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the item?", "Yes", "No"))
        {
            int rIndex = rList.index;

            rList.index--;

            InputAlias lAlias = mTarget.DefaultActionAliases[rIndex];
            if (lAlias.Name.Length > 0 && lAlias.PrimaryID >= 0)
            {
                InputManager.RemoveAlias(lAlias.Name, lAlias.PrimaryID, (lAlias.SupportID <= 0 ? 0 : lAlias.SupportID));
            }

            mTarget.DefaultActionAliases.RemoveAt(rIndex);
            OnAliasListItemSelect(rList);

            mIsDirty = true;
        }
    }

    /// <summary>
    /// Allows us to process after the motions are reordered
    /// </summary>
    /// <param name="rList"></param>
    private void OnAliasListReorder(ReorderableList rList)
    {
        mIsDirty = true;
    }

    /// <summary>
    /// Renders the currently selected step
    /// </summary>
    /// <param name="rStep"></param>
    private bool DrawAliasDetailItem(ref InputAlias rAlias)
    {
        bool lIsDirty = false;

        string lOldName = (rAlias.Name == null ? "" : rAlias.Name);
        int lOldPrimaryID = rAlias.PrimaryID;
        int lOldSupportID = rAlias.SupportID;

        if (rAlias.Name == null) { rAlias.Name = ""; }
        if (rAlias.Description == null) { rAlias.Description = ""; }

        EditorHelper.DrawSmallTitle(rAlias.Name);

        if (rAlias.Description.Length > 0)
        {
            EditorHelper.DrawInspectorDescription(rAlias.Description, MessageType.None);
        }
        else
        {
            EditorGUILayout.LabelField("", GUILayout.Width(2));
        }

        // Friendly name
        string lNewName = EditorGUILayout.TextField(new GUIContent("Name", "Friendly name of the alias."), rAlias.Name);
        if (lNewName != rAlias.Name)
        {
            lIsDirty = true;
            rAlias.Name = lNewName;
        }

        // Friendly description
        string lNewDescription = EditorGUILayout.TextField(new GUIContent("Description", "Higher priorities will run over lower priorities."), rAlias.Description);
        if (lNewDescription != rAlias.Description)
        {
            lIsDirty = true;
            rAlias.Description = lNewDescription;
        }

        GUILayout.Space(5);

        // Primary ID
        int lPrimaryIndex = -1;
        for (int i = 0; i < mEnumIDs.Length; i++)
        {
            if (mEnumIDs[i] == rAlias.PrimaryID)
            {
                lPrimaryIndex = i;
                break;
            }
        }

        int lNewPrimaryIndex = EditorGUILayout.Popup("Primary Input", lPrimaryIndex, mEnumNames, GUILayout.MinWidth(50));
        if (lNewPrimaryIndex != lPrimaryIndex)
        {
            if (lNewPrimaryIndex >= 0)
            {
                lIsDirty = true;
                rAlias.PrimaryID = mEnumIDs[lNewPrimaryIndex];
            }
        }

        // Support ID
        int lSupportIndex = -1;
        for (int i = 0; i < mEnumIDs.Length; i++)
        {
            if (mEnumIDs[i] == rAlias.SupportID)
            {
                lSupportIndex = i;
                break;
            }
        }

        EditorGUILayout.BeginHorizontal();

        int lNewSupportIndex = EditorGUILayout.Popup("Support Input", lSupportIndex, mEnumNames, GUILayout.MinWidth(50));
        if (lNewSupportIndex != lSupportIndex)
        {
            if (lNewSupportIndex >= 0)
            {
                lIsDirty = true;
                rAlias.SupportID = mEnumIDs[lNewSupportIndex];
            }
        }

        if (GUILayout.Button(new GUIContent("", "Clear the support"), EditorHelper.RedXButton, GUILayout.Width(16), GUILayout.Height(16)))
        {
            lIsDirty = true;
            rAlias.SupportID = -1;
        }

        GUILayout.Space(2);

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        bool lNewIsDoubleClick = EditorGUILayout.Toggle(new GUIContent("Test As Double Press", "When IsPressed is called, determine if we test as a double press."), rAlias.IsDoublePress);
        if (lNewIsDoubleClick != rAlias.IsDoublePress)
        {
            lIsDirty = true;
            rAlias.IsDoublePress = lNewIsDoubleClick;
        }

        // If the entry is dirty and we're playing, update the InputManager
        if (lIsDirty && Application.isPlaying)
        {
            if (lOldName.Length > 0)
            {
                InputManager.RemoveAlias(lOldName, lOldPrimaryID, (lOldSupportID <= 0 ? 0 : lOldSupportID));
            }

            if (rAlias.Name.Length > 0 && rAlias.PrimaryID >= 0)
            {
                InputManager.AddAlias(rAlias.Name, rAlias.PrimaryID, (rAlias.SupportID <= 0 ? 0 : rAlias.SupportID));
            }
        }

        return lIsDirty;
    }

    /// <summary>
    /// Removes entries from the input manager
    /// </summary>
    private void ClearInputManagerEntries()
    {
        InputManagerHelper.RemoveEntry("WXButton0");
        InputManagerHelper.RemoveEntry("WXButton1");
        InputManagerHelper.RemoveEntry("WXButton2");
        InputManagerHelper.RemoveEntry("WXButton3");
        InputManagerHelper.RemoveEntry("WXLeftStickX");
        InputManagerHelper.RemoveEntry("WXLeftStickY");
        InputManagerHelper.RemoveEntry("WXLeftStickButton");
        InputManagerHelper.RemoveEntry("WXRightStickX");
        InputManagerHelper.RemoveEntry("WXRightStickY");
        InputManagerHelper.RemoveEntry("WXRightStickButton");
        InputManagerHelper.RemoveEntry("WXLeftTrigger");
        InputManagerHelper.RemoveEntry("WXRightTrigger");
        InputManagerHelper.RemoveEntry("WXDPadX");
        InputManagerHelper.RemoveEntry("WXDPadY");
        InputManagerHelper.RemoveEntry("WXLeftBumper");
        InputManagerHelper.RemoveEntry("WXRightBumper");
        InputManagerHelper.RemoveEntry("WXBack");
        InputManagerHelper.RemoveEntry("WXStart");

        InputManagerHelper.RemoveEntry("MXButton0");
        InputManagerHelper.RemoveEntry("MXButton1");
        InputManagerHelper.RemoveEntry("MXButton2");
        InputManagerHelper.RemoveEntry("MXButton3");
        InputManagerHelper.RemoveEntry("MXLeftStickX");
        InputManagerHelper.RemoveEntry("MXLeftStickY");
        InputManagerHelper.RemoveEntry("MXLeftStickButton");
        InputManagerHelper.RemoveEntry("MXRightStickX");
        InputManagerHelper.RemoveEntry("MXRightStickY");
        InputManagerHelper.RemoveEntry("MXRightStickButton");
        InputManagerHelper.RemoveEntry("MXLeftTrigger");
        InputManagerHelper.RemoveEntry("MXRightTrigger");
        InputManagerHelper.RemoveEntry("MXDPadX");
        InputManagerHelper.RemoveEntry("MXDPadY");
        InputManagerHelper.RemoveEntry("MXLeftBumper");
        InputManagerHelper.RemoveEntry("MXRightBumper");
        InputManagerHelper.RemoveEntry("MXBack");
        InputManagerHelper.RemoveEntry("MXStart");
    }

    /// <summary>
    /// If the input manager entries don't exist, create them
    /// </summary>
    private void CreateInputManagerEntries()
    {
        if (!InputManagerHelper.IsDefined("WXButton0"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXButton0";
            lEntry.PositiveButton = "joystick button 0";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXButton1"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXButton1";
            lEntry.PositiveButton = "joystick button 1";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXButton2"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXButton2";
            lEntry.PositiveButton = "joystick button 2";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXButton3"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXButton3";
            lEntry.PositiveButton = "joystick button 3";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXLeftStickX"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXLeftStickX";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.1f;
            lEntry.Sensitivity = 1;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 1;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXLeftStickY"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXLeftStickY";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.1f;
            lEntry.Sensitivity = 1;
            lEntry.Invert = true;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 2;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXLeftStickButton"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXLeftStickButton";
            lEntry.PositiveButton = "joystick button 8";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXRightStickX"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXRightStickX";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 4;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXRightStickY"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXRightStickY";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Invert = true;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 5;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXRightStickButton"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXRightStickButton";
            lEntry.PositiveButton = "joystick button 9";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXLeftTrigger"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXLeftTrigger";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 9;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXRightTrigger"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXRightTrigger";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 10;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXDPadX"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXDPadX";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 6;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXDPadY"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXDPadY";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 7;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXLeftBumper"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXLeftBumper";
            lEntry.PositiveButton = "joystick button 4";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXRightBumper"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXRightBumper";
            lEntry.PositiveButton = "joystick button 5";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXBack"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXBack";
            lEntry.PositiveButton = "joystick button 6";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("WXStart"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "WXStart";
            lEntry.PositiveButton = "joystick button 7";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXButton0"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXButton0";
            lEntry.PositiveButton = "joystick button 16";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXButton1"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXButton1";
            lEntry.PositiveButton = "joystick button 17";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXButton2"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXButton2";
            lEntry.PositiveButton = "joystick button 18";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXButton3"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXButton3";
            lEntry.PositiveButton = "joystick button 19";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXLeftStickX"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXLeftStickX";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 1;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXLeftStickY"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXLeftStickY";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Invert = true;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 2;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXLeftStickButton"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXLeftStickButton";
            lEntry.PositiveButton = "joystick button 11";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXRightStickX"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXRightStickX";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 3;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXRightStickY"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXRightStickY";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Invert = true;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 4;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXRightStickButton"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXRightStickButton";
            lEntry.PositiveButton = "joystick button 12";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXLeftTrigger"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXLeftTrigger";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 5;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXRightTrigger"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXRightTrigger";
            lEntry.Gravity = 1;
            lEntry.Dead = 0.3f;
            lEntry.Sensitivity = 1;
            lEntry.Type = InputManagerEntryType.JOYSTICK_AXIS;
            lEntry.Axis = 6;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXDPadX"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXDPadX";
            lEntry.PositiveButton = "joystick button 7";
            lEntry.NegativeButton = "joystick button 8";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXDPadY"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXDPadY";
            lEntry.PositiveButton = "joystick button 6";
            lEntry.NegativeButton = "joystick button 5";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXLeftBumper"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXLeftBumper";
            lEntry.PositiveButton = "joystick button 13";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXRightBumper"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXRightBumper";
            lEntry.PositiveButton = "joystick button 14";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXBack"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXBack";
            lEntry.PositiveButton = "joystick button 10";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }

        if (!InputManagerHelper.IsDefined("MXStart"))
        {
            InputManagerEntry lEntry = new InputManagerEntry();
            lEntry.Name = "MXStart";
            lEntry.PositiveButton = "joystick button 9";
            lEntry.Gravity = 1000;
            lEntry.Dead = 0.001f;
            lEntry.Sensitivity = 1000;
            lEntry.Type = InputManagerEntryType.KEY_MOUSE_BUTTON;
            lEntry.Axis = 0;
            lEntry.JoyNum = 0;

            InputManagerHelper.AddEntry(lEntry);
        }
    }
}
