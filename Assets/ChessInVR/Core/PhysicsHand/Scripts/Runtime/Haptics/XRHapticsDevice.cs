using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using HapticsSystem;

namespace PhysicsHand.Haptics
{
    /// <summary>
    /// A component that allows haptics to be played on an XR device.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class XRHapticsDevice : IHapticsDevice
    {
        [Header("Settings")]
        [Tooltip("The channel to play haptics on.")]
        public uint hapticChannel = 0;
        [Tooltip("The XR node for the XR haptics device.")]
        public XRNode xrNode = XRNode.LeftHand;

        /// <summary>A dictionary of XRNodes are their respective InputDevices.</summary>
        Dictionary<XRNode, List<InputDevice>> m_Devices = new Dictionary<XRNode, List<InputDevice>>();

        // Unity callback(s).
        void Start()
        {
            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;
        }

        void OnDestroy()
        {
            InputDevices.deviceConnected -= OnDeviceConnected;
            InputDevices.deviceDisconnected -= OnDeviceDisconnected;
        }

        // Public method(s).
        /// <summary>Registers all XR devices in the XRNode with the haptics manager.</summary>
        /// <param name="pNode"></param>
        public void RegisterXRNode(XRNode pNode)
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(pNode, devices);
            if (devices != null && devices.Count > 0)
            {
                m_Devices[pNode] = devices;
            }
        }

        /// <summary>Deregisters all XR devices in the XRNode with the haptics manager.</summary>
        /// <param name="pNode"></param>
        public void DeregisterXRNode(XRNode pNode)
        {
            m_Devices.Remove(pNode);
        }


        /// <summary>A public method that allows the 'hapticChannel' field to be set. Useful for use with Unity editor events.</summary>
        /// <param name="pChannel"></param>
        public void SetHapticChannel(uint pChannel) { hapticChannel = pChannel; }

        // Protected virtual callback(s).
        protected virtual void OnDeviceConnected(InputDevice pDevice)
        {
            // (Re)register XR node.
            RegisterXRNode(xrNode);
        }

        protected virtual void OnDeviceDisconnected(InputDevice pDevice)
        {
            // Check if this device exists anywhere in the devices dictionary, if it does remove it.
            bool removed = false;
            bool removeNode = false;
            XRNode nodeToRemove = XRNode.Head;
            foreach (var pair in m_Devices)
            {
                for (int i = 0; i < pair.Value.Count; ++i)
                {
                    if (pair.Value[i] == pDevice)
                    {
                        pair.Value.RemoveAt(i);
                        nodeToRemove = pair.Key;
                        removed = true;
                        break;
                    }
                }

                // If a device was removed break out of the loop after checking if the node should be removed from the dictionary.
                if (removed)
                {
                    if (m_Devices[nodeToRemove].Count == 0)
                        removeNode = true;
                    break;
                }
            }

            // If 'removeNode' is true remove the node from the dictionary.
            if (removeNode)
                m_Devices.Remove(nodeToRemove);
        }

        #region Virtual Overridden Methods
        public override void PlayHapticImpulse(float pDuration, float pAmplitude, float pFrequency)
        {
            foreach (var pair in m_Devices)
            {
                foreach (InputDevice device in pair.Value)
                {
                    if (device.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
                        device.SendHapticImpulse(hapticChannel, pAmplitude, pDuration);
                }
            }
        }

        public override void StopHaptics()
        {
            foreach (var pair in m_Devices)
            {
                foreach (InputDevice device in pair.Value)
                {
                    if (device.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
                        device.StopHaptics();
                }
            }
        }
        #endregion
    }
}
