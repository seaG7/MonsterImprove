using UnityEngine;
using UnityEngine.Events;
using System;

namespace PhysicsHand.Demo.Gadgets.Keypads
{
    /// <summary>
    /// A component that provides public methods that allow for full control of keypad.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class KeypadController : MonoBehaviour
    {
        // InputUnityEvent.
        /// <summary>Arg0: string - The 'input' field value after being trimmed./summary>
        [Serializable]
        public class InputUnityEvent : UnityEvent<string> {}

        /// <summary>
        /// Arg0: KeypadController - The KeypadController that caused the event to fire.
        /// Arg1: Keypad - The Keypad associated with the event.
        /// Arg2: string - The code associated with the event.
        /// </summary>
        [Serializable]
        public class KeypadUnityEvent : UnityEvent<KeypadController, Keypad, string> { }

        // KeypadUIController.
        #region Editor Serialized Settings & Events
        [Header("Instance")]
        [Tooltip("The currect input into the numeric keypad.")]
        public string input = "";

        [Header("Settings")]
        [Tooltip("A reference to the Keypad this controller is controlling.")]
        public Keypad keypad;
        [Tooltip("Only enable this if you plan to allow 'input' to be changed in a way that freely allows invalid characters to be inputted, otherwise it is an unneccesary performance hit.")]
        public bool realtimeTrim;
        [Tooltip("The max length of the input string.")]
        public int maxInputLength = 4;
        [Tooltip("Clear 'input' after the correct code is submitted by the controller?")]
        public bool clearOnCorrect = true;
        [Tooltip("Clear 'input' after the incorrect code is submitted by the controller?")]
        public bool clearOnIncorrect = true;

        [Header("Events")]
        [Tooltip("An event that is invoked when this keypad controller submits the correct code to the relevant Keypad.\n\nArg0: KeypadController - the KeypadController that caused the event to fire.\nArg1: Keypad - the Keypad assocaited with the event.\nArg2: string - the code associated with the event.'")]
        public KeypadUnityEvent SubmittedCorrectCode;
        [Tooltip("An event that is invoked when this keypad controller submits the incorrect code to the relevant Keypad.\n\nArg0: KeypadController - the KeypadController that caused the event to fire.\nArg1: Keypad - the Keypad assocaited with the event.\nArg2: string - the code associated with the event.'")]
        public KeypadUnityEvent SubmittedIncorrectCode;
        [Tooltip("An event that is invoekd when input is added to 'input' using the 'AddInput' method.\n\nArg0: string - the input string that was added.")]
        public InputUnityEvent AddedInput;
        [Tooltip("An event that is invoked when input is removed using the 'RemoveInput' method.")]
        public UnityEvent RemovedInput;
        [Tooltip("An event that is invoked each Update() where 'input' has changed.")]
        public InputUnityEvent InputChanged;
        #endregion
        #region Public Properties
        /// <summary>The 'last fired' input string used to track when 'input' changes..</summary>
        public string LastFiredInput { get; protected set; } = null;
        #endregion

        // Unity callback(s)
        #region Unity Callback(s)
        protected virtual void Awake()
        {
            // Look for Keypad component.
            if (keypad == null)
                keypad = GetComponent<Keypad>();
            if (keypad == null)
                Debug.LogWarning("No 'keypad' referenecd on KeypadController!", gameObject);
        }

        protected virtual void Reset()
        {
            // Look for Keypad component.
            if (keypad == null)
                keypad = GetComponent<Keypad>();
        }

        protected virtual void Update()
        {
            // Trim input if realtime trimming is enabled.
            if (realtimeTrim)
                TrimInput();

            // Check for input change.
            if (LastFiredInput == null || LastFiredInput != input)
            {
                // Invoke the 'InputChanged' Unity event.
                InputChanged?.Invoke(input);
                
                // Update 'last fired input'.
                LastFiredInput = input;
            }
        }
        #endregion

        // Public method(s).
        #region Input Method(s)
        /// <summary>Adds the given string to 'input'.</summary>
        /// <param name="pString"></param>
        public void AddInput(string pString)
        {
            if (input.Length + pString.Length <= maxInputLength)
            {
                input += pString;
                TrimInput();

                // Invoke the 'AddedInput' Unity event.
                AddedInput?.Invoke(pString);
            }
        }

        /// <summary>Removes the last character from 'input'.</summary>
        public void RemoveInput()
        {
            if (!string.IsNullOrEmpty(input))
                input = input.Substring(0, input.Length - 1);
            TrimInput();

            // Invoke the 'RemovedInput' Unity event.
            RemovedInput?.Invoke();
        }

        /// <summary>Clears all input.</summary>
        public void ClearInput()
        {
            input = "";
        }

        /// <summary>Submits the current 'input' to the keypad.</summary>
        public void Submit()
        {
            // Trim the input.
            TrimInput();

            // Submit the input.
            if (keypad.TrySubmitCode(input))
            {
                // Correct code inputted.
                // Invoke the 'SubmittedCorrectCode' Unity event.
                SubmittedCorrectCode?.Invoke(this, keypad, input);

                // Clear 'input' if set to be cleared on correct.
                if (clearOnCorrect)
                    ClearInput();
            }
            else
            {
                // Incorrect code inputted.
                // Invoke the 'SubmittedIncorrectCode' Unity event.
                SubmittedIncorrectCode?.Invoke(this, keypad, input);

                // Clear 'input' if set to be cleared on incorrect.
                if (clearOnIncorrect)
                    ClearInput();
            }
        }

        /// <summary>Ensures 'input' isn't too long.</summary>
        public void TrimInputLength()
        {
            // Ensure input isnt too long.
            if (input.Length > maxInputLength)
                input = input.Substring(0, maxInputLength);
        }
        #endregion

        // Public virtual method(s)
        #region Overrideable Input Method(s)
        /// <summary>Trims any non-valid characters (anything but 0 to 9) from the 'input' field of this component.</summary>
        public virtual void TrimInput()
        {
            // Trim the input length.
            TrimInputLength();
        }
        #endregion
    }
}
