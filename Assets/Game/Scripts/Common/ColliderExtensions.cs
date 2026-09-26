using UnityEngine;

namespace SampleGame
{
    public static class ColliderExtensions
    {
        // Точки концов капсулы и радиус в мировых координатах - для PhysicsScene.OverlapCapsule.
        public static void GetPointsAndRadius(
            this CapsuleCollider collider,
            out Vector3 point0,
            out Vector3 point1,
            out float radius
        )
        {
            Transform t = collider.transform;
            Vector3 center = t.TransformPoint(collider.center);
            Vector3 lossyScale = t.lossyScale;

            switch (collider.direction)
            {
                case 0: // X
                    radius = collider.radius * Mathf.Max(lossyScale.y, lossyScale.z);
                    float halfX = Mathf.Max(0f, collider.height * 0.5f * lossyScale.x - radius);
                    point0 = center + t.right * halfX;
                    point1 = center - t.right * halfX;
                    break;

                case 1: // Y
                    radius = collider.radius * Mathf.Max(lossyScale.x, lossyScale.z);
                    float halfY = Mathf.Max(0f, collider.height * 0.5f * lossyScale.y - radius);
                    point0 = center + t.up * halfY;
                    point1 = center - t.up * halfY;
                    break;

                default: // Z
                    radius = collider.radius * Mathf.Max(lossyScale.x, lossyScale.y);
                    float halfZ = Mathf.Max(0f, collider.height * 0.5f * lossyScale.z - radius);
                    point0 = center + t.forward * halfZ;
                    point1 = center - t.forward * halfZ;
                    break;
            }
        }
    }
}
