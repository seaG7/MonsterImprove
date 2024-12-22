using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ChessInVR.Calibration
{
    /// <summary>
    /// An abstract component that defines a 'player calibrator'.
	/// There are various derived player celibrator types like:
    ///	- ChessPlayerCalibrator, which situates the player at the player pose for
	///   the current turn.
	/// - AIChessPlayerCalibrator, which is for ChessAIGameManagers where if no
	///   AI opponents are enabled or both AI opponents are enabled the player is 
	///   situated at the player pose for the current turn. If only one player is AI
	///   then the player is always situated on their team's player pose.
	/// - NetworkChessPlayerCalibrator, which is for NetworkChessGameManagers where
	///   the player is always situated on the player pose for their team.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public abstract class IChessPlayerCalibrator : MonoBehaviour
    {
        #region Editor Serialized Setting(s)
        [Header("Inputs")]
        [Tooltip("The input that will recalibrate the player's view.")]
        [SerializeField] protected InputActionProperty m_CalibrateInput;

        [Header("Events")]
        [Tooltip("An event that is invoked just before the calibrator is calibrated.")]
        public UnityEvent PreCalibrated;
        [Tooltip("An event that is invoked after the calibrator is calibrated.")]
        public UnityEvent PostCalibrated;
        #endregion

        #region Unity Callback(s)
        protected virtual void OnEnable()
        {
            // Bind actions.
            BindActions();
        }

        protected virtual void OnDisable()
        {
            // Unbind actions.
            UnbindActions();
        }
        #endregion

        #region Public Calibration Method(s)
        /// <summary>Calibrates the chess player so that they are situated at the appropriate place.</summary>
        public void Calibrate()
        {
            // Invoke the 'PreCalibrated' Unity event.
            PreCalibrated?.Invoke();

            // Invoke overrideable calibrate callback.
            OnCalibrate();

            // Invoke the 'PostCalibrated' Unity event.
            PostCalibrated?.Invoke();
        }
        #endregion

        #region Protected Abstract Calibration Callback(s)
        /// <summary>Invoked when 'Calibrate()' has been called before 'PostCalibrated' is invoked but after 'PreCalibrated'.</summary>
        protected abstract void OnCalibrate();
        #endregion
        #region Protected Virtual Input Method(s)
        protected virtual void BindActions()
        {
            // Enable inputs and subscribe to event(s).
            if (m_CalibrateInput != null)
            {
                m_CalibrateInput.action.Enable();
                m_CalibrateInput.action.started += OnCalibratePressed;
            }
        }

        protected virtual void UnbindActions()
        {
            // Disable inputs and unsubscribe from event(s).
            if (m_CalibrateInput != null)
            {
                m_CalibrateInput.action.Disable();
                m_CalibrateInput.action.started -= OnCalibratePressed;
            }
        }
        #endregion

        #region Protected Virtual Input Callback(s)
        protected virtual void OnCalibratePressed(InputAction.CallbackContext pContext)
        {
            // Recalibrate.
            Calibrate();
        }
        #endregion
    }
}
