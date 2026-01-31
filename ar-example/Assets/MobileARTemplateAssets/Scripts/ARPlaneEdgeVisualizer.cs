using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Collections;

namespace UnityEngine.XR.Templates.AR
{
    /// <summary>
    /// Visualizes AR plane edges as 3D wireframe lines using LineRenderer.
    /// Attaches to ARPlane GameObjects and updates when plane boundaries change.
    /// </summary>
    [RequireComponent(typeof(ARPlane))]
    public class ARPlaneEdgeVisualizer : MonoBehaviour
    {
        [Tooltip("Material used for rendering the edge lines.")]
        [SerializeField]
        Material m_EdgeMaterial;

        /// <summary>
        /// Material used for rendering the edge lines.
        /// </summary>
        public Material edgeMaterial
        {
            get => m_EdgeMaterial;
            set
            {
                m_EdgeMaterial = value;
                if (m_LineRenderer != null)
                {
                    m_LineRenderer.material = m_EdgeMaterial;
                }
            }
        }

        [Tooltip("Width of the edge lines in world units.")]
        [SerializeField]
        [Range(0.001f, 0.1f)]
        float m_LineWidth = 0.015f;

        /// <summary>
        /// Width of the edge lines in world units.
        /// </summary>
        public float lineWidth
        {
            get => m_LineWidth;
            set
            {
                m_LineWidth = value;
                if (m_LineRenderer != null)
                {
                    m_LineRenderer.startWidth = m_LineWidth;
                    m_LineRenderer.endWidth = m_LineWidth;
                }
            }
        }

        [Tooltip("Height offset above the plane to avoid z-fighting (in world units).")]
        [SerializeField]
        float m_HeightOffset = 0.001f;

        /// <summary>
        /// Height offset above the plane to avoid z-fighting (in world units).
        /// </summary>
        public float heightOffset
        {
            get => m_HeightOffset;
            set => m_HeightOffset = value;
        }

        [Tooltip("Whether the edge visualization is visible.")]
        [SerializeField]
        bool m_IsVisible = true;

        /// <summary>
        /// Whether the edge visualization is visible.
        /// </summary>
        public bool isVisible
        {
            get => m_IsVisible;
            set
            {
                m_IsVisible = value;
                if (m_LineRenderer != null)
                {
                    m_LineRenderer.enabled = m_IsVisible;
                }
            }
        }

        ARPlane m_Plane;
        LineRenderer m_LineRenderer;
        List<Vector3> m_BoundaryPoints = new List<Vector3>();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Awake()
        {
            m_Plane = GetComponent<ARPlane>();
            
            // Create LineRenderer component
            m_LineRenderer = gameObject.AddComponent<LineRenderer>();
            m_LineRenderer.useWorldSpace = true;
            m_LineRenderer.loop = true;
            m_LineRenderer.startWidth = m_LineWidth;
            m_LineRenderer.endWidth = m_LineWidth;
            m_LineRenderer.enabled = m_IsVisible;
            
            if (m_EdgeMaterial != null)
            {
                m_LineRenderer.material = m_EdgeMaterial;
            }
            else
            {
                // Create a default unlit material if none is assigned
                m_LineRenderer.material = CreateDefaultMaterial();
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnEnable()
        {
            if (m_Plane != null)
            {
                m_Plane.boundaryChanged += OnBoundaryChanged;
                UpdateEdgeVisualization();
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDisable()
        {
            if (m_Plane != null)
            {
                m_Plane.boundaryChanged -= OnBoundaryChanged;
            }
        }

        /// <summary>
        /// Callback when plane boundary changes.
        /// </summary>
        void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
        {
            UpdateEdgeVisualization();
        }

        /// <summary>
        /// Updates the edge visualization based on current plane boundary.
        /// </summary>
        void UpdateEdgeVisualization()
        {
            if (m_Plane == null || m_LineRenderer == null)
                return;

            // Get boundary points from ARPlane
            // ARPlane.boundary returns NativeArray<Vector2> in plane-local space
            var boundary = m_Plane.boundary;
            
            if (boundary.Length == 0)
            {
                m_LineRenderer.positionCount = 0;
                return;
            }

            // Convert boundary points from plane-local 2D space to world 3D space
            m_BoundaryPoints.Clear();
            m_BoundaryPoints.Capacity = boundary.Length;

            Transform planeTransform = m_Plane.transform;
            
            for (int i = 0; i < boundary.Length; i++)
            {
                // Boundary points are in plane-local XZ plane (Y is up)
                Vector2 localPoint = boundary[i];
                Vector3 localPoint3D = new Vector3(localPoint.x, m_HeightOffset, localPoint.y);
                
                // Transform to world space
                Vector3 worldPoint = planeTransform.TransformPoint(localPoint3D);
                m_BoundaryPoints.Add(worldPoint);
            }

            // Update LineRenderer with boundary points
            if (m_BoundaryPoints.Count > 0)
            {
                m_LineRenderer.positionCount = m_BoundaryPoints.Count;
                m_LineRenderer.SetPositions(m_BoundaryPoints.ToArray());
            }
        }

        /// <summary>
        /// Creates a default unlit material for edge visualization.
        /// </summary>
        Material CreateDefaultMaterial()
        {
            // Create a simple unlit material with bright cyan color
            Material mat = new Material(Shader.Find("Unlit/Color"));
            if (mat != null)
            {
                mat.color = new Color(0f, 1f, 1f, 1f); // Cyan
            }
            return mat;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            // Update visualization periodically in case boundary changes aren't detected
            // This is a fallback - boundaryChanged event should handle most cases
            if (m_Plane != null && m_Plane.boundary.Length > 0)
            {
                // Only update if the boundary count has changed (indicates boundary update)
                if (m_Plane.boundary.Length != m_BoundaryPoints.Count)
                {
                    UpdateEdgeVisualization();
                }
            }
        }
    }
}
