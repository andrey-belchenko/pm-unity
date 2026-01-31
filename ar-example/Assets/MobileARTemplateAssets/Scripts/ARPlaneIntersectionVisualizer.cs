using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.Templates.AR
{
    /// <summary>
    /// Visualizes intersections between AR planes as 3D wireframe lines.
    /// Tracks all detected planes and calculates intersections between pairs.
    /// </summary>
    public class ARPlaneIntersectionVisualizer : MonoBehaviour
    {
        [Tooltip("Material used for rendering the intersection lines.")]
        [SerializeField]
        Material m_IntersectionMaterial;

        /// <summary>
        /// Material used for rendering the intersection lines.
        /// </summary>
        public Material intersectionMaterial
        {
            get => m_IntersectionMaterial;
            set => m_IntersectionMaterial = value;
        }

        [Tooltip("Width of the intersection lines in world units.")]
        [SerializeField]
        [Range(0.001f, 0.1f)]
        float m_LineWidth = 0.02f;

        /// <summary>
        /// Width of the intersection lines in world units.
        /// </summary>
        public float lineWidth
        {
            get => m_LineWidth;
            set
            {
                m_LineWidth = value;
                UpdateAllLineRenderers();
            }
        }

        [Tooltip("Maximum distance to check for plane intersections.")]
        [SerializeField]
        float m_MaxIntersectionDistance = 10f;

        /// <summary>
        /// Maximum distance to check for plane intersections.
        /// </summary>
        public float maxIntersectionDistance
        {
            get => m_MaxIntersectionDistance;
            set => m_MaxIntersectionDistance = value;
        }

        [Tooltip("Minimum angle between planes to consider them intersecting (in degrees).")]
        [SerializeField]
        [Range(1f, 90f)]
        float m_MinIntersectionAngle = 5f;

        /// <summary>
        /// Minimum angle between planes to consider them intersecting (in degrees).
        /// </summary>
        public float minIntersectionAngle
        {
            get => m_MinIntersectionAngle;
            set => m_MinIntersectionAngle = value;
        }

        [Tooltip("The plane manager to track planes from.")]
        [SerializeField]
        ARPlaneManager m_PlaneManager;

        /// <summary>
        /// The plane manager to track planes from.
        /// </summary>
        public ARPlaneManager planeManager
        {
            get => m_PlaneManager;
            set => m_PlaneManager = value;
        }

        // Dictionary to store intersection line renderers: key is a pair of plane trackable IDs
        Dictionary<string, LineRenderer> m_IntersectionLines = new Dictionary<string, LineRenderer>();
        Transform m_IntersectionContainer;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Awake()
        {
            // Create a container for intersection lines
            GameObject container = new GameObject("Intersection Lines");
            container.transform.SetParent(transform);
            m_IntersectionContainer = container.transform;

            // Get plane manager if not set
            if (m_PlaneManager == null)
            {
                m_PlaneManager = FindObjectOfType<ARPlaneManager>();
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnEnable()
        {
            if (m_PlaneManager != null)
            {
                m_PlaneManager.trackablesChanged.AddListener(OnPlanesChanged);
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDisable()
        {
            if (m_PlaneManager != null)
            {
                m_PlaneManager.trackablesChanged.RemoveListener(OnPlanesChanged);
            }
        }

        /// <summary>
        /// Callback when planes are added, updated, or removed.
        /// </summary>
        void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> eventArgs)
        {
            UpdateIntersections();
        }

        /// <summary>
        /// Updates all intersection visualizations.
        /// </summary>
        void UpdateIntersections()
        {
            if (m_PlaneManager == null)
                return;

            // Get all current planes
            List<ARPlane> planes = new List<ARPlane>();
            foreach (var plane in m_PlaneManager.trackables)
            {
                if (plane != null && plane.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    planes.Add(plane);
                }
            }

            // Track which intersections we've created
            HashSet<string> activeIntersections = new HashSet<string>();

            // Calculate intersections for all pairs of planes
            for (int i = 0; i < planes.Count; i++)
            {
                for (int j = i + 1; j < planes.Count; j++)
                {
                    ARPlane plane1 = planes[i];
                    ARPlane plane2 = planes[j];

                    // Check if planes are at a reasonable angle
                    float angle = Vector3.Angle(plane1.normal, plane2.normal);
                    if (angle < m_MinIntersectionAngle || angle > 180f - m_MinIntersectionAngle)
                        continue; // Planes are too parallel

                    // Calculate intersection
                    if (ARPlaneIntersectionCalculator.CalculateIntersectionSegment(
                        plane1, plane2, out Vector3 startPoint, out Vector3 endPoint, m_MaxIntersectionDistance))
                    {
                        string key = GetIntersectionKey(plane1.trackableId, plane2.trackableId);
                        activeIntersections.Add(key);

                        // Create or update line renderer
                        if (!m_IntersectionLines.ContainsKey(key))
                        {
                            CreateIntersectionLine(key, startPoint, endPoint);
                        }
                        else
                        {
                            UpdateIntersectionLine(key, startPoint, endPoint);
                        }
                    }
                }
            }

            // Remove intersection lines for planes that no longer exist or intersect
            List<string> keysToRemove = new List<string>();
            foreach (var key in m_IntersectionLines.Keys)
            {
                if (!activeIntersections.Contains(key))
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                DestroyIntersectionLine(key);
            }
        }

        /// <summary>
        /// Creates a unique key for a pair of planes.
        /// </summary>
        string GetIntersectionKey(UnityEngine.XR.ARSubsystems.TrackableId id1, UnityEngine.XR.ARSubsystems.TrackableId id2)
        {
            // Ensure consistent ordering (smaller ID first)
            if (id1.subId1 < id2.subId1 || (id1.subId1 == id2.subId1 && id1.subId2 < id2.subId2))
            {
                return $"{id1.subId1}_{id1.subId2}_{id2.subId1}_{id2.subId2}";
            }
            else
            {
                return $"{id2.subId1}_{id2.subId2}_{id1.subId1}_{id1.subId2}";
            }
        }

        /// <summary>
        /// Creates a new intersection line renderer.
        /// </summary>
        void CreateIntersectionLine(string key, Vector3 startPoint, Vector3 endPoint)
        {
            GameObject lineObject = new GameObject($"Intersection_{key}");
            lineObject.transform.SetParent(m_IntersectionContainer);

            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.loop = false;
            lineRenderer.startWidth = m_LineWidth;
            lineRenderer.endWidth = m_LineWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);

            if (m_IntersectionMaterial != null)
            {
                lineRenderer.material = m_IntersectionMaterial;
            }
            else
            {
                // Create default material
                Material mat = CreateDefaultMaterial();
                lineRenderer.material = mat;
            }

            m_IntersectionLines[key] = lineRenderer;
        }

        /// <summary>
        /// Updates an existing intersection line.
        /// </summary>
        void UpdateIntersectionLine(string key, Vector3 startPoint, Vector3 endPoint)
        {
            if (m_IntersectionLines.TryGetValue(key, out LineRenderer lineRenderer))
            {
                lineRenderer.SetPosition(0, startPoint);
                lineRenderer.SetPosition(1, endPoint);
            }
        }

        /// <summary>
        /// Destroys an intersection line.
        /// </summary>
        void DestroyIntersectionLine(string key)
        {
            if (m_IntersectionLines.TryGetValue(key, out LineRenderer lineRenderer))
            {
                Destroy(lineRenderer.gameObject);
                m_IntersectionLines.Remove(key);
            }
        }

        /// <summary>
        /// Updates all line renderer widths.
        /// </summary>
        void UpdateAllLineRenderers()
        {
            foreach (var lineRenderer in m_IntersectionLines.Values)
            {
                if (lineRenderer != null)
                {
                    lineRenderer.startWidth = m_LineWidth;
                    lineRenderer.endWidth = m_LineWidth;
                }
            }
        }

        /// <summary>
        /// Creates a default unlit material for intersection visualization.
        /// </summary>
        Material CreateDefaultMaterial()
        {
            Material mat = new Material(Shader.Find("Unlit/Color"));
            if (mat != null)
            {
                mat.color = new Color(1f, 0f, 0f, 1f); // Red for visibility
            }
            return mat;
        }

        /// <summary>
        /// Clears all intersection visualizations.
        /// </summary>
        public void ClearAllIntersections()
        {
            List<string> keysToRemove = new List<string>(m_IntersectionLines.Keys);
            foreach (var key in keysToRemove)
            {
                DestroyIntersectionLine(key);
            }
        }
    }
}
