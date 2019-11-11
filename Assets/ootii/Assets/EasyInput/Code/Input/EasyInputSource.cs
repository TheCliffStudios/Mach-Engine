using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Input
{
    /// <summary>
    /// Input source that grabs user input from ootii's Easy Input solution.
    /// </summary>
    [AddComponentMenu("ootii/Input Sources/Easy Input Source")]
    public class EasyInputSource : MonoBehaviour, IInputSource, IViewActivator
    {
        /// <summary>
        /// Helps users of the input source to determine if
        /// they are processing user input
        /// </summary>
        public bool _IsEnabled = true;
        public virtual bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; }
        }

        /// <summary>
        /// Determines if the Xbox controller is enable. We default this to off since
        /// the editor needs to enable settings.
        /// </summary>
        public bool _IsXboxControllerEnabled = false;
        public virtual bool IsXboxControllerEnabled
        {
            get { return _IsXboxControllerEnabled; }
            set
            {
                _IsXboxControllerEnabled = value;
                if (Application.isPlaying)
                {
                    InputManager.UseGamepad = _IsXboxControllerEnabled;
                }
            }
        }

        /// <summary>
        /// Set by an external object, it tracks the angle of the
        /// user input compared to the camera's forward direction
        /// Note that this info isn't reliable as objects using it 
        /// before it's set it will get float.NaN.
        /// </summary>
        protected float mInputFromCameraAngle = 0f;
        public virtual float InputFromCameraAngle
        {
            get { return mInputFromCameraAngle; }
            set { mInputFromCameraAngle = value; }
        }

        /// <summary>
        /// Set by an external object, it tracks the angle of the
        /// user input compared to the avatars's forward direction
        /// Note that this info isn't reliable as objects using it 
        /// before it's set it will get float.NaN.
        /// </summary>
        protected float mInputFromAvatarAngle = 0f;
        public virtual float InputFromAvatarAngle
        {
            get { return mInputFromAvatarAngle; }
            set { mInputFromAvatarAngle = value; }
        }

        /// <summary>
        /// Retrieves horizontal movement from the the input
        /// </summary>
        public virtual float MovementX
        {
            get
            {
                if (!_IsEnabled) { return 0f; }
                return InputManager.MovementX;
            }
        }

        /// <summary>
        /// Retrieves vertical movement from the the input
        /// </summary>
        public virtual float MovementY
        {
            get
            {
                if (!_IsEnabled) { return 0f; }
                return InputManager.MovementY;
            }
        }

        /// <summary>
        /// Squared value of MovementX and MovementY (mX * mX + mY * mY)
        /// </summary>
        public virtual float MovementSqr
        {
            get
            {
                if (!_IsEnabled) { return 0f; }

                float lMovementX = MovementX;
                float lMovementY = MovementY;
                return ((lMovementX * lMovementX) + (lMovementY * lMovementY));
            }
        }

        /// <summary>
        /// Retrieves horizontal view movement from the the input
        /// </summary>
        public virtual float ViewX
        {
            get
            {
                if (!_IsEnabled) { return 0f; }
                return InputManager.ViewX;
            }
        }

        /// <summary>
        /// Retrieves vertical view movement from the the input
        /// </summary>
        public virtual float ViewY
        {
            get
            {
                if (!_IsEnabled) { return 0f; }
                return InputManager.ViewY;
            }
        }

        /// <summary>
        /// Allows us to store and reset the view activator
        /// We want to use the Input Manager, but also sync with UnityInputSource
        /// 0 = none (true)
        /// 1 = LMB
        /// 2 = RMB
        /// 3 = LMB || RMB
        /// </summary>
        public int _ViewActivator = 2;
        public virtual int ViewActivator
        {
            get { return _ViewActivator; }

            set
            {
                _ViewActivator = value;
                if (Application.isPlaying)
                {
                    // Clear the activator
                    InputManager.ViewActivator = -1;

                    // Reset the activator (as needed)
                    if (_ViewActivator == 1)
                    {
                        InputManager.ViewActivator = EnumInput.MOUSE_LEFT_BUTTON;
                    }
                    else if (_ViewActivator == 2)
                    {
                        InputManager.ViewActivator = EnumInput.MOUSE_RIGHT_BUTTON;
                    }
                    else if (_ViewActivator == 3)
                    {
                        InputManager.ViewActivator = EnumInput.MOUSE_LEFT_BUTTON;
                        InputManager.ViewActivator = EnumInput.MOUSE_RIGHT_BUTTON;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the player can freely look around
        /// </summary>
        public virtual bool IsViewingActivated
        {
            get
            {
                if (!_IsEnabled) { return false; }

                if (_ViewActivator == -1)
                {
                    return true;
                }
                else if (_ViewActivator == 3)
                {
                    bool lValue = InputManager.IsPressed(EnumInput.MOUSE_LEFT_BUTTON);
                    if (!lValue) { lValue = InputManager.IsPressed(EnumInput.MOUSE_RIGHT_BUTTON); }

                    return lValue;
                }

                return InputManager.IsViewingActivated;
            }
        }

        /// <summary>
        /// Determines how quickly the mouse view movement updates
        /// </summary>
        public float _ViewSensativity = 1f;
        public float ViewSensativity
        {
            get { return _ViewSensativity; }

            set
            {
                _ViewSensativity = value;
                if (Application.isPlaying)
                {
                    InputManager.MouseViewSensativity = _ViewSensativity;
                }
            }
        }

        /// <summary>
        /// Holds onto a set of action aliases that we'll register at the beginning
        /// of the game. Throughout the game, the true values would beheld in the
        /// InputManager and could be changed.
        /// </summary>
        public List<InputAlias> DefaultActionAliases = new List<InputAlias>();

        /// <summary>
        /// Allows us to keep the last selected alias open
        /// </summary>
        public int EditorActionAliasIndex = -1;

        /// <summary>
        /// Called before any Start() or Update() functions in order to initialize.
        /// </summary>
        protected virtual void Awake()
        {
            // Initialize the manager
            InputManager.Initialize();

            // Set the default values
            InputManager.UseGamepad = _IsXboxControllerEnabled;

            // We also need to force it in the singleton
            ViewActivator = _ViewActivator;
            ViewSensativity = _ViewSensativity;

            // Register any default action aliases that exist
            if (DefaultActionAliases != null)
            {
                for (int i = 0; i < DefaultActionAliases.Count; i++)
                {
                    if (DefaultActionAliases[i].Name.Length > 0 && DefaultActionAliases[i].PrimaryID >= 0)
                    {
                        InputManager.AddAlias(DefaultActionAliases[i].Name, DefaultActionAliases[i].PrimaryID, (DefaultActionAliases[i].SupportID <= 0 ? 0 : DefaultActionAliases[i].SupportID));
                    }
                }
            }
        }

        /// <summary>
        /// Test if a specific key is pressed this frame.
        /// </summary>
        /// <param name="rKey"></param>
        /// <returns></returns>
        public virtual bool IsJustPressed(KeyCode rKey)
        {
            if (!_IsEnabled) { return false; }
            return InputManager.IsJustPressed(rKey);
        }

        /// <summary>
        /// Test if a specific key is pressed this frame.
        /// </summary>
        /// <param name="rEnumInput">Input Manager enumerated key to test</param>
        /// <returns>Boolean that determines if the action just took place</returns>
        public virtual bool IsJustPressed(int rKey)
        {
            if (!_IsEnabled) { return false; }
            return InputManager.IsJustPressed(rKey);
        }

        /// <summary>
        /// Test if a specific action is pressed this frame.
        /// </summary>
        /// <param name="rAction">Action to test for</param>
        /// <returns>Boolean that determines if the action just took place</returns>
        public virtual bool IsJustPressed(string rAction)
        {
            if (!_IsEnabled) { return false; }

            for (int i = 0; i < DefaultActionAliases.Count; i++)
            {
                if (DefaultActionAliases[i].Name == rAction)
                {
                    if (DefaultActionAliases[i].IsDoublePress)
                    {
                        return InputManager.IsDoublePressed(rAction);
                    }

                    break;
                }
            }

            return InputManager.IsJustPressed(rAction);
        }

        /// <summary>
        /// Test if a specific key is pressed. This is used for continuous checking.
        /// </summary>
        /// <param name="rKey"></param>
        /// <returns></returns>
        public virtual bool IsPressed(KeyCode rKey)
        {
            if (!_IsEnabled) { return false; }
            return InputManager.IsPressed(rKey);
        }

        /// <summary>
        /// Test if a specific key is pressed. This is used for continuous checking.
        /// </summary>
        /// <param name="rEnumInput">Input Manager enumerated key to test</param>
        /// <returns>Boolean that determines if the action is taking place</returns>
        public virtual bool IsPressed(int rKey)
        {
            if (!_IsEnabled) { return false; }
            return InputManager.IsPressed(rKey);
        }

        /// <summary>
        /// Test if a specific action is pressed. This is used for continuous checking.
        /// </summary>
        /// <param name="rAction">Action to test for</param>
        /// <returns>Boolean that determines if the action is taking place</returns>
        public virtual bool IsPressed(string rAction)
        {
            if (!_IsEnabled) { return false; }
            return InputManager.IsPressed(rAction);
        }

        /// <summary>
        /// Test if a specific key is double pressed. This is used for continuous checking.
        /// </summary>
        /// <param name="rEnumInput">Input Manager enumerated key to test</param>
        /// <returns>Boolean that determines if the action is taking place</returns>
        public virtual bool IsDoublePressed(int rKey)
        {
            if (!_IsEnabled) { return false; }
            return InputManager.IsDoublePressed(rKey);
        }

        /// <summary>
        /// Test if a specific action is double pressed. This is used for continuous checking.
        /// </summary>
        /// <param name="rAction">Action to test for</param>
        /// <returns>Boolean that determines if the action is taking place</returns>
        public virtual bool IsDoublePressed(string rAction)
        {
            if (!_IsEnabled) { return false; }
            return InputManager.IsDoublePressed(rAction);
        }

        /// <summary>
        /// Test if a specific key is released this frame.
        /// </summary>
        /// <param name="rKey"></param>
        /// <returns></returns>
        public virtual bool IsJustReleased(KeyCode rKey)
        {
            if (!_IsEnabled) { return false; }
            return InputManager.IsJustReleased(rKey);
        }

        /// <summary>
        /// Test if a specific key is released this frame.
        /// </summary>
        /// <param name="rKey">Input Manager enumerated key to test</param>
        /// <returns>Boolean that determines if the action just took place</returns>
        public virtual bool IsJustReleased(int rKey)
        {
            if (!_IsEnabled) { return false; }
            return InputManager.IsJustReleased(rKey);
        }

        /// <summary>
        /// Test if a specific action is released this frame.
        /// </summary>
        /// <param name="rAction">Action to test for</param>
        /// <returns>Boolean that determines if the action just took place</returns>
        public virtual bool IsJustReleased(string rAction)
        {
            if (!_IsEnabled) { return false; }
            return InputManager.IsJustReleased(rAction);
        }

        /// <summary>
        /// Test if a specific key is not pressed. This is used for continuous checking.
        /// </summary>
        /// <param name="rKey"></param>
        /// <returns></returns>
        public virtual bool IsReleased(KeyCode rKey)
        {
            if (!_IsEnabled) { return false; }
            return !InputManager.IsPressed(rKey);
        }

        /// <summary>
        /// Test if a specific key is not pressed. This is used for continuous checking.
        /// </summary>
        /// <param name="rEnumInput">Input Manager enumerated key to test</param>
        /// <returns>Boolean that determines if the action is taking place</returns>
        public virtual bool IsReleased(int rKey)
        {
            if (!_IsEnabled) { return false; }
            return !InputManager.IsPressed(rKey);
        }

        /// <summary>
        /// Test if a specific action is not pressed. This is used for continuous checking.
        /// </summary>
        /// <param name="rAction">Action to test for</param>
        /// <returns>Boolean that determines if the action is taking place</returns>
        public virtual bool IsReleased(string rAction)
        {
            if (!_IsEnabled) { return false; }
            return !InputManager.IsPressed(rAction);
        }

        /// <summary>
        /// Test for a specific action value.
        /// </summary>
        /// <param name="rKey">Input Manager enumerated key to test</param>
        /// <returns>Float value as determined by the key</returns>
        public virtual float GetValue(int rKey)
        {
            return InputManager.GetValue(rKey);
        }

        /// <summary>
        /// Test for a specific action value.
        /// </summary>
        /// <param name="rAction">Action to test for</param>
        /// <returns>Float value as determined by the action</returns>
        public virtual float GetValue(string rAction)
        {
            return InputManager.GetValue(rAction);
        }

    }
}
