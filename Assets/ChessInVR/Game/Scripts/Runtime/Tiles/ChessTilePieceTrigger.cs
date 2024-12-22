using System;
using UnityEngine;
using UnityEngine.Events;
using GrabSystem;
using ChessEngine.Game;
using ChessInVR.Grabbing;

namespace ChessInVR
{
    /// <summary>
    /// A simple component that manages a chess piece entering a visual chess table tile trigger.
    /// </summary>
    /// Author: Mathew Aloisio
    public class ChessTilePieceTrigger : MonoBehaviour
    {
        // ChessGrabberUnityEvent.
        /// <summary>An event involving a ChessGrabber component and a ChessTilePieceTrigger component.</summary>
        [Serializable]
        public class ChessGrabberTriggerUnityEvent : UnityEvent<ChessGrabber, ChessTilePieceTrigger> {}

        // ChessTilePieceTrigger.
        #region Editor Serialized Setting(s)
        [Header("Settings")]
        [Tooltip("A reference to the VisualChessTableTile this trigger is for.")]
        public VisualChessTableTile visualTile;
        #endregion
        #region Editor Serialized Event(s)
        [Header("Events")]
        [Tooltip("An event that is invoked whenever this tile is triggered.\n\nArg0: ChessGrabber - the chess grabber involved in the event.\nArg1: ChessTilePieceTrigger - the chess tile piece trigger involved in the event.")]
        public ChessGrabberTriggerUnityEvent TriggeredTile;
        [Tooltip("An event that is invoked whenever this tile is untriggered.\n\nArg0: ChessGrabber - the chess grabber involved in the event.\nArg1: ChessTilePieceTrigger - the chess tile piece trigger involved in the event.")]
        public ChessGrabberTriggerUnityEvent UntriggeredTile;
        #endregion

        #region Unity Callback(s)
        void Awake()
        {
            if (visualTile == null)
                Debug.LogWarning("No 'visualTile' specified for ChessTilePieceTrigger!", gameObject);
        }

        void Start() { } // NOTE: This method is only included to force the 'enabled' checkbox to show in the editor.

        void Reset()
        {
            // Look for 'visualTile' reference.
            visualTile = GetComponentInParent<VisualChessTableTile>();
        }

        void OnTriggerEnter(Collider pOther)
        {
            // Only handle if enabled.
            if (enabled)
            {
                // Look for ChessTileActivator that entered.
                ChessTileActivator activator = pOther.GetComponent<ChessTileActivator>();

                // Only handle activators entering the trigger that are valid and enabled.
                if (activator != null && activator.enabled)
                {
                    // Ensure the visual piece is grabbable.
                    GrabbableObject grabbablePiece = activator.visualPiece.GetComponent<GrabbableObject>();
                    if (grabbablePiece != null)
                    {
                        // Ensure the piece is being grabbed.
                        if (grabbablePiece.HeldByCount > 0)
                        {
                            // Check if the first grabber holding this piece (since there should only be 1 anyway) has a ChessGrabber.
                            ChessGrabber chessGrabber = grabbablePiece.GetHeldBy(0).GetComponent<ChessGrabber>();
                            if (chessGrabber != null)
                            {
                                // Trigger the new tile.
                                TriggerTile(chessGrabber);
                            }
                        }
                    }
                }
            }
        }

        void OnTriggerStay(Collider pOther)
        {
            // Only handle if enabled.
            if (enabled)
            {
                // Look for ChessTileActivator that entered.
                ChessTileActivator activator = pOther.GetComponent<ChessTileActivator>();

                // Only handle activators entering the trigger that are valid and enabled.
                if (activator != null && activator.enabled)
                {
                    // Ensure the visual piece is grabbable.
                    GrabbableObject grabbablePiece = activator.visualPiece.GetComponent<GrabbableObject>();
                    if (grabbablePiece != null)
                    {
                        // Ensure the piece is being grabbed.
                        if (grabbablePiece.HeldByCount > 0)
                        {
                            // Check if the first grabber holding this piece (since there should only be 1 anyway) has a ChessGrabber.
                            ChessGrabber chessGrabber = grabbablePiece.GetHeldBy(0).GetComponent<ChessGrabber>();
                            if (chessGrabber != null && chessGrabber.TriggeringTile == null) // NOTE: The 'Stay' event only triggers while there is no triggered tile.
                            {
                                // Trigger the new tile.
                                TriggerTile(chessGrabber);
                            }
                        }
                    }
                }
            }
        }

        void OnTriggerExit(Collider pOther)
        {
            // Look for ChessTileActivator that entered.
            ChessTileActivator activator = pOther.GetComponent<ChessTileActivator>();

            // Only handle activators entering the trigger that are valid.
            if (activator != null)
            {
                // Ensure the visual piece is grabbable.
                GrabbableObject grabbablePiece = activator.visualPiece.GetComponent<GrabbableObject>();
                if (grabbablePiece != null)
                {
                    // Ensure the piece is being grabbed.
                    if (grabbablePiece.HeldByCount > 0)
                    {
                        // Check if the first grabber holding this piece (since there should only be 1 anyway) has a ChessGrabber.
                        ChessGrabber chessGrabber = grabbablePiece.GetHeldBy(0).GetComponent<ChessGrabber>();
                        if (chessGrabber != null)
                        {
                            // Ensure that the chess grabber is currently triggering this tile before un-triggering it.
                            if (chessGrabber.TriggeringTile == this)
                            {
                                // Untrigger the tile.
                                UntriggerTile(chessGrabber);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Public Trigger Method(s)
        /// <summary>Triggers the specified tile untriggering any other non-matching triggered tile.</summary>
        /// <param name="pGrabber"></param>
        public void TriggerTile(ChessGrabber pGrabber)
        {
            // Don't re-trigger.
            if (pGrabber.TriggeringTile != this)
            {
                // Untrigger existing tile if needed.
                if (pGrabber.TriggeringTile != null)
                    pGrabber.TriggeringTile.UntriggerTile(pGrabber);

                // Trigger the new tile.
                pGrabber.TriggerTile(this); // Trigger this tile for the found chess grabber.

                // Invoke the 'TriggeredTile' Unity event.
                TriggeredTile?.Invoke(pGrabber, this);
            }
        }

        /// <summary>Notifies the system that the specified grabber stopped triggering this trigger's tile.</summary>
        /// <param name="pGrabber"></param>
        public void UntriggerTile(ChessGrabber pGrabber)
        {
            // Only untrigger if triggered by this.
            if (pGrabber.TriggeringTile == this)
            {
                pGrabber.UntriggerTile();

                // Invoke the 'UntriggeredTile' Unity event.
                UntriggeredTile?.Invoke(pGrabber, this);
            }
        }
        #endregion
    }
}
