using UnityEngine;
using System.Collections.Generic;

namespace PhysicsHand.Demo.Joints
{
    /// <summary>
    /// A simple component that enables or disables all joints on the same GameObject as it as it becomes enabled and disabled.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(Joint))]
    public class JointToggler : MonoBehaviour
    {
        // JointEntry.
        public struct JointEntry
        {
            public Joint joint;
            public Rigidbody connectedBody;
 #if UNITY_2020_1_OR_NEWER
            public ArticulationBody articulationBody;
#endif
        }

        // JointToggler.
        List<JointEntry> m_JointEntries = new List<JointEntry>();

        // Unity callback(s).
        void Awake()
        {
            // Find all joints associated with this component.
            Joint[] joints = GetComponents<Joint>();
            foreach (Joint joint in joints)
            {
                // Add new joint entry.
                m_JointEntries.Add(new JointEntry()
                {
                    joint = joint,
                    connectedBody = joint.connectedBody,
#if UNITY_2020_1_OR_NEWER
                    articulationBody = joint.connectedArticulationBody
#endif
                });

                // If the component is disabled de-reference joint stuff.
                if (!enabled)
                {
                    joint.connectedBody = null;
#if UNITY_2020_1_OR_NEWER
                    joint.connectedArticulationBody = null;
#endif
                }
            }
        }

        void OnEnable()
        {
            // Restore all joints.
            for (int i = 0; i < m_JointEntries.Count; ++i)
            {
                // Restore connected body and articulation body references.
                m_JointEntries[i].joint.connectedBody = m_JointEntries[i].connectedBody;
#if UNITY_2020_1_OR_NEWER
                m_JointEntries[i].joint.connectedArticulationBody = m_JointEntries[i].articulationBody;
#endif
            }
        }

        void OnDisable()
        {
            // Disable all joints.
            for (int i = 0; i < m_JointEntries.Count; ++i)
            {
                // Nullify connected body and articulation body references.
                m_JointEntries[i].joint.connectedBody = null;
#if UNITY_2020_1_OR_NEWER
                m_JointEntries[i].joint.connectedArticulationBody = null;
#endif
            }
        }
    }
}
