using System;
using UnityEngine;
using UnityEngine.Events;

namespace PhysicsHand.Demo.Gadgets.Keypads
{
    /// <summary>
    /// A component that provides the public methods required to operate a keypad.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class Keypad : MonoBehaviour
    {
        // CodeUnityEvent.
        /// <summary>
        /// Arg0: Keypad - The Keypad involved in the event.
        /// Arg1: string - The code involved in the event.
        /// </summary>
        [Serializable]
        public class CodeUnityEvent : UnityEvent<Keypad, string> { }

        // Keypad.
        #region Editor Serialized Setting(s)
        [Header("Settings")]
        [Tooltip("The code to the keypad.")]
        public string code = "1234";

        [Header("Events")]
        [Tooltip("An event that is invoked when the correct code is submitted to the keypad.\n\nArg0: Keypad - The Keypad involved in the event.\nArg1: string - The code involved in the event.")]
        public CodeUnityEvent SubmittedCorrectCode;
        [Tooltip("An event that is invoked when an incorrect code is submitted to the keypad.\n\nArg0: Keypad - The Keypad involved in the event.\nArg1: string - The code involved in the event.")]
        public CodeUnityEvent SubmittedIncorrectCode;
        #endregion

        // Public method(s).
        /// <summary>Submits a code to the keypad.</summary>
        /// <param name="pCode"></param>
        public void SubmitCode(string pCode)
        {
            TrySubmitCode(pCode);
        }

        /// <summary>Submits a code to the keypad and returns whether it was correct or not.</summary>
        /// <param name="pCode"></param>
        /// <returns>true if pCode == code, otherwise false.</returns>
        public bool TrySubmitCode(string pCode)
        {
            // If the correct code was entered invoke the 'submitted correct code' event.
            if (IsCorrectCode(pCode))
            {
                // Invoke the 'SubmittedCorrectCode' Unity event.
                SubmittedCorrectCode?.Invoke(this, pCode);

                // Return true, correct code submitted.
                return true;
            }
            // Otherwise the incorrect code was submitted, invoke the 'submitted incorrect code' event.
            else
            {
                // Invoke the 'SubmittedIncorrectCode' Unity event.
                SubmittedIncorrectCode?.Invoke(this, pCode);

                // Incorrect code submitted, return false.
                return false;
            }      
        }

        // Public virtual method(s).
        /// <summary>Returns true if pCode is the correct keypad code, otherwise false.</summary>
        /// <param name="pCode"></param>
        /// <returns>true if pCode is the correct keypad code, otherwise false.</returns>
        public virtual bool IsCorrectCode(string pCode)
        {
            return pCode == code;
        }
    }
}
