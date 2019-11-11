using System;
using UnityEngine;
using com.ootii.Input;
//using com.ootii.Utilities.Debug;

namespace com.ootii.Demos.EI
{
    /// <summary>
    /// Provides a central place for core game code and initialization.
    /// </summary>
    public class GameCode : MonoBehaviour
    {
        private UnityEngine.UI.Text mText;

        /// <summary>
        /// Awake is used to initialize any variables or game state before the game starts. 
        /// Awake is called only once during the lifetime of the script instance. Awake is 
        /// called after all objects are initialized so you can safely speak to other objects or query them.
        /// </summary>
        public void Awake()
        {
            mText = GameObject.Find("TextInfo").GetComponent<UnityEngine.UI.Text>();
            mText.text = "Easy Input";

            InputManager.Initialize();

            // Create friendly names and associate multiple inputs to them
            InputManager.AddAlias("Jump", EnumInput.SPACE);
            InputManager.AddAlias("Jump", EnumInput.GAMEPAD_0_BUTTON); // a

            InputManager.AddAlias("Sprint", EnumInput.LEFT_SHIFT);
            InputManager.AddAlias("Sprint", EnumInput.GAMEPAD_3_BUTTON); // y

            InputManager.AddAlias("Aiming", EnumInput.MOUSE_RIGHT_BUTTON);
            InputManager.AddAlias("Aiming", EnumInput.GAMEPAD_LEFT_TRIGGER);

            InputManager.AddAlias("Release", EnumInput.LEFT_SHIFT);
            InputManager.AddAlias("Release", EnumInput.GAMEPAD_3_BUTTON); // y

            InputManager.AddAlias("Stance", EnumInput.T);
            InputManager.AddAlias("Stance", EnumInput.GAMEPAD_RIGHT_TRIGGER);

            InputManager.AddAlias("ChangeStance", EnumInput.T);
            InputManager.AddAlias("ChangeStance", EnumInput.GAMEPAD_RIGHT_TRIGGER);

            InputManager.AddAlias("PrimaryAttack", EnumInput.MOUSE_LEFT_BUTTON);
            InputManager.AddAlias("PrimaryAttack", EnumInput.GAMEPAD_2_BUTTON); // x

            InputManager.AddAlias("Block", EnumInput.Q, EnumInput.LEFT_SHIFT);
            InputManager.AddAlias("Q", EnumInput.Q);

            InputManager.AddAlias("Pause", EnumInput.PAUSE);

            InputManager.AddAlias("Custom", CustomTest);
        }

        /// <summary>
        /// Update() is called once per frame. It is the main workhorse function for frame updates.
        /// </summary>
        public void Update()
        {
            // Test each input and show the results on the screen
            TestButton(EnumInput.MOUSE_LEFT_BUTTON, "mouse_left_button");
            TestButton(EnumInput.MOUSE_RIGHT_BUTTON, "mouse_right_button");
            TestButton(EnumInput.MOUSE_MIDDLE_BUTTON, "mouse_middle_button");

            TestButton(EnumInput.GAMEPAD_0_BUTTON, "xbox_button_0");
            TestButton(EnumInput.GAMEPAD_1_BUTTON, "xbox_button_1");
            TestButton(EnumInput.GAMEPAD_2_BUTTON, "xbox_button_2");
            TestButton(EnumInput.GAMEPAD_3_BUTTON, "xbox_button_3");
            TestButton(EnumInput.GAMEPAD_BACK_BUTTON, "xbox_back");
            TestButton(EnumInput.GAMEPAD_START_BUTTON, "xbox_start");
            TestButton(EnumInput.GAMEPAD_LEFT_BUMPER, "xbox_left_bumper");
            TestButton(EnumInput.GAMEPAD_RIGHT_BUMPER, "xbox_right_bumper");
            TestButton(EnumInput.GAMEPAD_LEFT_BUMPER, "xbox_left_bumper2");
            TestButton(EnumInput.GAMEPAD_RIGHT_BUMPER, "xbox_right_bumper2");
            TestButton(EnumInput.GAMEPAD_LEFT_TRIGGER, "xbox_left_trigger");
            TestButton(EnumInput.GAMEPAD_RIGHT_TRIGGER, "xbox_right_trigger");

            if (!TestAxis(EnumInput.GAMEPAD_LEFT_STICK_X, "xbox_left_stick"))
            {
                TestAxis(EnumInput.GAMEPAD_LEFT_STICK_Y, "xbox_left_stick");
            }

            if (!TestAxis(EnumInput.GAMEPAD_RIGHT_STICK_X, "xbox_right_stick"))
            {
                TestAxis(EnumInput.GAMEPAD_RIGHT_STICK_Y, "xbox_right_stick");
            }

            if (!TestAxis(EnumInput.GAMEPAD_DPAD_X, "xbox_d_pad"))
            {
                TestAxis(EnumInput.GAMEPAD_DPAD_Y, "xbox_d_pad");
            }

            TestKey(EnumInput.BACKSPACE);
            TestKey(EnumInput.TAB);
            TestKey(EnumInput.ENTER);
            TestKey(EnumInput.ESCAPE);
            TestKey(EnumInput.SPACE);

            for (int i = 33; i <= 36; i++)
            {
                //TestKey(i);
            }

            for (int i = 44; i <= 57; i++)
            {
                TestKey(i);
            }

            TestKey(EnumInput.EQUALS);
            TestKey(EnumInput.CAPS_LOCK);
            TestKey(EnumInput.SCROLL_LOCK);
            TestKey(EnumInput.LEFT_SHIFT);
            TestKey(EnumInput.NUM_LOCK);
            TestKey(EnumInput.BACK_QUOTE);

            for (int i = 70; i <= 77; i++)
            {
                TestKey(i);
            }

            for (int i = 79; i <= 82; i++)
            {
                TestKey(i);
            }

            TestKey(EnumInput.BACK_SLASH);
            TestKey(EnumInput.LEFT_BRACKET);
            TestKey(EnumInput.RIGHT_BRACKET);

            for (int i = 97; i <= 122; i++)
            {
                TestKey(i);
            }

            for (int i = 128; i <= 143; i++)
            {
                TestKey(i);
            }

            for (int i = 150; i <= 161; i++)
            {
                TestKey(i);
            }

            TestKey(EnumInput.DELETE);

            UpdateText();

            //string lString = "";
            //if (InputManager.IsJustSingleReleased(EnumInput.MOUSE_LEFT_BUTTON)) { lString += "LMB SR, "; }
            //if (InputManager.IsJustDoubleReleased(EnumInput.MOUSE_LEFT_BUTTON)) { lString += "LMB DR "; }
            //if (lString.Length > 0) { Log.FileWrite(lString); }
        }

        /// <summary>
        /// Update the text we're displaying
        /// </summary>
        private void UpdateText()
        {
            string lText = "";

            lText += "Get more input information" + "\r\n";
            lText += "Change input settings during runtime" + "\r\n";
            lText += "Support Windows & Mac Xbox controllers easily" + "\r\n";

            lText += "\r\n";
            lText += "Keyboard" + "\r\n";

            string lKeys = "";
            if (InputManager.KeysPressedCount == 0)
            {
                lKeys += "none";
            }
            else
            {
                for (int i = 0; i < InputManager.KeysPressedCount; i++)
                {
                    if (lKeys.Length > 0) { lKeys += ", "; }
                    lKeys += EnumInput.EnumNames[InputManager.KeysPressed[i]];
                }
            }

            lText += "  Keys pressed: " + lKeys + "\r\n";

            lText += "\r\n";
            lText += "Mouse" + "\r\n";
            lText += "  X, Y: " + InputManager.MouseX.ToString("0") + ", " + InputManager.MouseY.ToString("0") + "\r\n";
            lText += "  XDelta, YDelta: " + InputManager.MouseXDelta.ToString("0") + ", " + InputManager.MouseYDelta.ToString("0") + "\r\n";
            lText += "  AxisX, AxisY: " + InputManager.MouseAxisX.ToString("0.000") + ", " + InputManager.MouseAxisY.ToString("0.000") + "\r\n";
            lText += "  Wheel: " + InputManager.GetValue(EnumInput.MOUSE_WHEEL) + "\r\n";

            lText += "\r\n";
            lText += "Input Info" + "\r\n";
            lText += "  Use the elapsed information to determine " + "\r\n";
            lText += "  how long any input has been active or inactive. " + "\r\n";
            lText += "\r\n";
            lText += "  Left Mouse Button" + "\r\n";
            lText += "    Is Toggled: " + InputManager.IsToggled(EnumInput.MOUSE_LEFT_BUTTON) + "\r\n";
            lText += "    Is Pressed: " + InputManager.IsPressed(EnumInput.MOUSE_LEFT_BUTTON) + "\r\n";
            lText += "    Is Just Pressed: " + InputManager.IsJustPressed(EnumInput.MOUSE_LEFT_BUTTON) + "\r\n";
            lText += "    Is Just Released: " + InputManager.IsJustReleased(EnumInput.MOUSE_LEFT_BUTTON) + "\r\n";
            lText += "    Is Double Pressed: " + InputManager.IsDoublePressed(EnumInput.MOUSE_LEFT_BUTTON) + "\r\n";
            lText += "    Pressed Elapsed: " + InputManager.PressedElapsedTime(EnumInput.MOUSE_LEFT_BUTTON).ToString("0.000") + "\r\n";
            lText += "    Released Elapsed: " + InputManager.ReleasedElapsedTime(EnumInput.MOUSE_LEFT_BUTTON).ToString("0.000") + "\r\n";

            lText += "\r\n";
            lText += "Actions" + "\r\n";
            lText += "  Create a friendly name like 'Jump' and " + "\r\n";
            lText += "  associate multiple inputs. Test once " + "\r\n";
            lText += "  with InputManager.IsJustPressed('Jump'). " + "\r\n";
            lText += "\r\n";
            lText += "  Move X, Y: " + InputManager.MovementX.ToString("0.000") + ", " + InputManager.MovementY.ToString("0.000") + "\r\n";
            lText += "  View X, Y: " + InputManager.ViewX.ToString("0.000") + ", " + InputManager.ViewY.ToString("0.000") + "\r\n";
            lText += "  Jump (space or Xbox-A): " + InputManager.IsPressed("Jump") + "\r\n";
            lText += "  Attack (mouse or Xbox-X): " + InputManager.IsPressed("PrimaryAttack") + "\r\n";
            lText += "  Block (shift + Q-Key): " + InputManager.IsPressed("Block") + "  Just Q:" + InputManager.IsPressed("Q") + "\r\n";
            lText += "  Custom Test: " + InputManager.IsPressed("Custom") + "\r\n";

            lText += "\r\n";

            float lTicksPerMS = (float)System.TimeSpan.TicksPerMillisecond;
            lText += "Processing Time: " + (InputManager.UpdateElapsedTicks / lTicksPerMS) + " milliseconds";

            mText.text = lText;
        }

        /// <summary>
        /// Test and highlight the button
        /// </summary>
        /// <param name="rButton">Button ID to test</param>
        /// <param name="rSprite">Sprite name to enable/disable</param>
        private bool TestButton(int rButton, string rSprite)
        {
            bool lIsPressed = InputManager.IsPressed(rButton);
            GameObject lImage = GameObject.Find("Highlights/" + rSprite);
            SpriteRenderer lRenderer = lImage.GetComponent<SpriteRenderer>();
            lRenderer.enabled = lIsPressed;

            return lIsPressed;
        }

        /// <summary>
        /// Test and highlight the axis
        /// </summary>
        /// <param name="rAxis">Axis ID to test</param>
        /// <param name="rSprite">Sprite name to enable/disable</param>
        private bool TestAxis(int rAxis, string rSprite)
        {
            float lValue = InputManager.GetValue(rAxis);
            GameObject lImage = GameObject.Find("Highlights/" + rSprite);
            SpriteRenderer lRenderer = lImage.GetComponent<SpriteRenderer>();
            lRenderer.enabled = (Mathf.Abs(lValue) > 0.1f);

            return (Mathf.Abs(lValue) > 0.1f);
        }

        /// <summary>
        /// Test and highlight the key
        /// </summary>
        /// <param name="rKey">Keycode to test</param>
        /// <returns></returns>
        private bool TestKey(int rKey)
        {
            bool lIsPressed = InputManager.IsPressed(rKey);
            GameObject lImage = GameObject.Find("Highlights/" + "key_" + rKey);
            SpriteRenderer lRenderer = lImage.GetComponent<SpriteRenderer>();
            lRenderer.enabled = lIsPressed;

            return lIsPressed;
        }

        /// <summary>
        /// Test for using a custom function with an alias
        /// </summary>
        /// <returns>Result of the custom test</returns>
        private float CustomTest()
        {
            return InputManager.GetValue(EnumInput.Y);
        }
    }
}
