using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Collections;

namespace UnityEngine.XR.Templates.AR
{
    /// <summary>
    /// Utility class for calculating intersections between AR planes.
    /// </summary>
    public static class ARPlaneIntersectionCalculator
    {
        /// <summary>
        /// Calculates the intersection line between two planes.
        /// </summary>
        /// <param name="plane1">First AR plane</param>
        /// <param name="plane2">Second AR plane</param>
        /// <param name="intersectionPoint">Output: A point on the intersection line</param>
        /// <param name="intersectionDirection">Output: Direction vector of the intersection line</param>
        /// <returns>True if planes intersect, false if they are parallel</returns>
        public static bool CalculateIntersection(
            ARPlane plane1, 
            ARPlane plane2, 
            out Vector3 intersectionPoint, 
            out Vector3 intersectionDirection)
        {
            intersectionPoint = Vector3.zero;
            intersectionDirection = Vector3.zero;

            if (plane1 == null || plane2 == null)
                return false;

            // Get plane centers and normals
            Vector3 center1 = plane1.center;
            Vector3 normal1 = plane1.normal;
            Vector3 center2 = plane2.center;
            Vector3 normal2 = plane2.normal;

            // Calculate direction of intersection line (cross product of normals)
            intersectionDirection = Vector3.Cross(normal1, normal2).normalized;

            // Check if planes are parallel (cross product is zero)
            if (intersectionDirection.sqrMagnitude < 0.001f)
                return false;

            // Find a point on the intersection line
            // Using the method from: https://stackoverflow.com/questions/6408670/plane-plane-intersection
            // We need to solve: n1 · (p - c1) = 0 and n2 · (p - c2) = 0
            // Where p is a point on the intersection line

            // Use the line direction and find a point that satisfies both plane equations
            // We'll find the point closest to both plane centers
            
            // Calculate the distance from center1 to the intersection line
            Vector3 d = center2 - center1;
            float denominator = Vector3.Dot(intersectionDirection, intersectionDirection);
            
            if (Mathf.Abs(denominator) < 0.001f)
                return false;

            // Project d onto the intersection direction
            float t = Vector3.Dot(d, intersectionDirection) / denominator;
            intersectionPoint = center1 + intersectionDirection * t;

            return true;
        }

        /// <summary>
        /// Calculates the intersection line segment within both planes' boundaries.
        /// </summary>
        /// <param name="plane1">First AR plane</param>
        /// <param name="plane2">Second AR plane</param>
        /// <param name="startPoint">Output: Start point of intersection segment</param>
        /// <param name="endPoint">Output: End point of intersection segment</param>
        /// <param name="maxDistance">Maximum distance to check for intersection</param>
        /// <returns>True if valid intersection segment found within boundaries</returns>
        public static bool CalculateIntersectionSegment(
            ARPlane plane1,
            ARPlane plane2,
            out Vector3 startPoint,
            out Vector3 endPoint,
            float maxDistance = 10f)
        {
            startPoint = Vector3.zero;
            endPoint = Vector3.zero;

            if (!CalculateIntersection(plane1, plane2, out Vector3 intersectionPoint, out Vector3 direction))
                return false;

            // Get plane boundaries
            var boundary1 = plane1.boundary;
            var boundary2 = plane2.boundary;

            if (boundary1.Length == 0 || boundary2.Length == 0)
                return false;

            // Transform boundary points to world space for both planes
            List<Vector3> worldBoundary1 = new List<Vector3>();
            List<Vector3> worldBoundary2 = new List<Vector3>();

            Transform transform1 = plane1.transform;
            Transform transform2 = plane2.transform;

            for (int i = 0; i < boundary1.Length; i++)
            {
                Vector2 localPoint = boundary1[i];
                Vector3 local3D = new Vector3(localPoint.x, 0, localPoint.y);
                worldBoundary1.Add(transform1.TransformPoint(local3D));
            }

            for (int i = 0; i < boundary2.Length; i++)
            {
                Vector2 localPoint = boundary2[i];
                Vector3 local3D = new Vector3(localPoint.x, 0, localPoint.y);
                worldBoundary2.Add(transform2.TransformPoint(local3D));
            }

            // Find intersection points with plane boundaries
            List<Vector3> intersectionPoints = new List<Vector3>();

            // Check intersections with plane1 boundary
            for (int i = 0; i < worldBoundary1.Count; i++)
            {
                Vector3 p1 = worldBoundary1[i];
                Vector3 p2 = worldBoundary1[(i + 1) % worldBoundary1.Count];
                
                if (LineIntersectsPlane(p1, p2, plane2, out Vector3 intersect))
                {
                    if (IsPointInBoundary(intersect, worldBoundary1))
                        intersectionPoints.Add(intersect);
                }
            }

            // Check intersections with plane2 boundary
            for (int i = 0; i < worldBoundary2.Count; i++)
            {
                Vector3 p1 = worldBoundary2[i];
                Vector3 p2 = worldBoundary2[(i + 1) % worldBoundary2.Count];
                
                if (LineIntersectsPlane(p1, p2, plane1, out Vector3 intersect))
                {
                    if (IsPointInBoundary(intersect, worldBoundary2))
                        intersectionPoints.Add(intersect);
                }
            }

            // If we have at least 2 intersection points, use them
            // Otherwise, find the closest points on boundaries to the intersection line
            if (intersectionPoints.Count >= 2)
            {
                // Use the two points that are furthest apart
                float maxDist = 0;
                int idx1 = 0, idx2 = 1;
                for (int i = 0; i < intersectionPoints.Count; i++)
                {
                    for (int j = i + 1; j < intersectionPoints.Count; j++)
                    {
                        float dist = Vector3.Distance(intersectionPoints[i], intersectionPoints[j]);
                        if (dist > maxDist)
                        {
                            maxDist = dist;
                            idx1 = i;
                            idx2 = j;
                        }
                    }
                }
                startPoint = intersectionPoints[idx1];
                endPoint = intersectionPoints[idx2];
                return true;
            }
            else if (intersectionPoints.Count == 1)
            {
                // Find closest point on the other plane's boundary
                startPoint = intersectionPoints[0];
                endPoint = FindClosestPointOnBoundary(intersectionPoint, direction, worldBoundary2, maxDistance);
                return Vector3.Distance(startPoint, endPoint) > 0.01f;
            }
            else
            {
                // Find closest points on both boundaries
                startPoint = FindClosestPointOnBoundary(intersectionPoint, direction, worldBoundary1, maxDistance);
                endPoint = FindClosestPointOnBoundary(intersectionPoint, direction, worldBoundary2, maxDistance);
                
                // Check if both points are within reasonable distance
                float dist1 = Vector3.Distance(intersectionPoint, startPoint);
                float dist2 = Vector3.Distance(intersectionPoint, endPoint);
                
                if (dist1 < maxDistance && dist2 < maxDistance && Vector3.Distance(startPoint, endPoint) > 0.01f)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a line segment intersects with a plane.
        /// </summary>
        private static bool LineIntersectsPlane(Vector3 lineStart, Vector3 lineEnd, ARPlane plane, out Vector3 intersection)
        {
            intersection = Vector3.zero;
            
            Vector3 planeNormal = plane.normal;
            Vector3 planePoint = plane.center;
            
            Vector3 lineDir = (lineEnd - lineStart).normalized;
            Vector3 lineStartToPlane = planePoint - lineStart;
            
            float denominator = Vector3.Dot(planeNormal, lineDir);
            if (Mathf.Abs(denominator) < 0.001f)
                return false; // Line is parallel to plane
            
            float t = Vector3.Dot(lineStartToPlane, planeNormal) / denominator;
            intersection = lineStart + lineDir * t;
            
            // Check if intersection is within line segment
            float lineLength = Vector3.Distance(lineStart, lineEnd);
            float distFromStart = Vector3.Distance(lineStart, intersection);
            
            return distFromStart <= lineLength + 0.01f;
        }

        /// <summary>
        /// Checks if a point is within a boundary polygon.
        /// </summary>
        private static bool IsPointInBoundary(Vector3 point, List<Vector3> boundary)
        {
            // Simple point-in-polygon test (ray casting algorithm)
            bool inside = false;
            for (int i = 0, j = boundary.Count - 1; i < boundary.Count; j = i++)
            {
                if (((boundary[i].z > point.z) != (boundary[j].z > point.z)) &&
                    (point.x < (boundary[j].x - boundary[i].x) * (point.z - boundary[i].z) / (boundary[j].z - boundary[i].z) + boundary[i].x))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        /// <summary>
        /// Finds the closest point on a boundary to an intersection line.
        /// </summary>
        private static Vector3 FindClosestPointOnBoundary(Vector3 linePoint, Vector3 lineDirection, List<Vector3> boundary, float maxDistance)
        {
            Vector3 closestPoint = linePoint;
            float minDistance = float.MaxValue;

            // Project each boundary point onto the line and find the closest
            for (int i = 0; i < boundary.Count; i++)
            {
                Vector3 boundaryPoint = boundary[i];
                Vector3 toPoint = boundaryPoint - linePoint;
                float projection = Vector3.Dot(toPoint, lineDirection);
                Vector3 projectedPoint = linePoint + lineDirection * projection;
                
                float distance = Vector3.Distance(boundaryPoint, projectedPoint);
                if (distance < minDistance && distance < maxDistance)
                {
                    minDistance = distance;
                    closestPoint = projectedPoint;
                }
            }

            return closestPoint;
        }
    }
}
