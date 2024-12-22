using UnityEngine;

namespace PhysicsHand.Lines
{
    /// <summary>
    /// A component that is attached to the same GameObject as a LineRenderer that provides simple methods for setting (or resetting) the end point of the line renderer.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererEndPoint : MonoBehaviour
    {
        #region Public Properties
        /// <summary>A reference to the LineRenderer associated with this component.</summary>
        public LineRenderer Renderer { get; private set; }
        #endregion

        /// <summary>The end point of the LineRenderer.</summary>
        Vector3 m_EndPoint;

        // Unity callback(s).
        #region Unity callback(s)
        void Awake()
        {
            // Find 'LineRenderer' reference.
            Renderer = GetComponent<LineRenderer>();

            // Store the line renderer end point.
            m_EndPoint = Renderer.GetPosition(Renderer.positionCount - 1);
        }
        #endregion

        // Public method(s).
        #region Point Control Method(s)
        /// <summary>Sets the world space end point of the line renderer.</summary>
        /// <param name="pEndPoint">The world space end point for the line renderer.</param>
        public void SetEndPoint(Vector3 pEndPoint)
        {
            Renderer.SetPosition(Renderer.positionCount - 1, Renderer.useWorldSpace ? pEndPoint : transform.InverseTransformPoint(pEndPoint));
        }

        /// <summary>Sets the local space end point of the line renderer.</summary>
        /// <param name="pEndPoint">The local space end point for the line renderer.</param>
        public void SetLocalEndPoint(Vector3 pEndPoint)
        {
            Renderer.SetPosition(Renderer.positionCount - 1, Renderer.useWorldSpace ? transform.TransformPoint(pEndPoint) : pEndPoint);
        }

        /// <summary>Resets the line renderer end point.</summary>
        public void ResetEndPoint()
        {
            // Reset end point.
            Renderer.SetPosition(Renderer.positionCount - 1, m_EndPoint);
        }
        #endregion
    }
}
