using System.Collections.Generic;
using ChessEngine;
using ChessInVR.Grabbing;
using UnityEngine;

namespace ChessInVR.Game
{
    /// <summary>
    /// A component that works with a ChessTilePieceTrigger component to manage a target when pieces are being placed on a tile.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(ChessTilePieceTrigger))]
    public class TileTargetManager : MonoBehaviour
    {
        #region Editor Serialized Setting(s)
        [Header("Settings")]
        [Tooltip("(Optional) The target GameObject that represents the target. This is enabled and disabled when specified based on triggered state.")]
        public GameObject targetObject;
        #endregion
        #region Public Properties
        /// <summary>A reference to the ChessTilePieceTrigger that is driving this component. </summary>
        public ChessTilePieceTrigger Trigger { get; private set; }
        #endregion

        #region Unity Callback(s)
        void Awake()
        {
            // Find 'Trigger' reference.
            Trigger = GetComponent<ChessTilePieceTrigger>();
        }

        void Start()
        {
            // If there is a 'target renderer' copy materials.
            /*if (m_TargetRenderer != null)
            {
                m_Materials = m_TargetRenderer.materials;
                for (int i = 0; i < m_Materials.Length; ++i)
                {
                    m_TargetRenderer.materials[i] = m_Materials[i];
                }
            }*/
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            Trigger.TriggeredTile.AddListener(OnTriggeredTile);
            Trigger.UntriggeredTile.AddListener(OnUntriggeredTile);
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            if (Trigger != null)
            {
                Trigger.TriggeredTile.RemoveListener(OnTriggeredTile);
                Trigger.UntriggeredTile.RemoveListener(OnUntriggeredTile);
            }
        }
        #endregion

        #region Private Callback(s)
        /// <summary>Invoked whenever a ChessGrabber triggers the tile.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pTrigger"></param>
        void OnTriggeredTile(ChessGrabber pGrabber, ChessTilePieceTrigger pTrigger)
        {
            // Enable the target object.
            if (targetObject != null)
                targetObject.SetActive(true);
        }

        /// <summary>Invoked whenever a ChessGrabber stops triggering the tile and whenever the triggered tile is overridden on the table.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pTrigger"></param>
        void OnUntriggeredTile(ChessGrabber pGrabber, ChessTilePieceTrigger pTrigger)
        {
            // Disable the target object.
            if (targetObject != null)
                targetObject.SetActive(false);
        }
        #endregion
    }
}
