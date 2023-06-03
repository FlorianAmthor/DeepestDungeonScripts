using System.Collections.Generic;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Utility
{
    /// <summary>
    /// Source: https://james-frowen.github.io/2017/09/28/trajectories-in-unity.html
    /// </summary>
    public class TrajectoryCalculator
    {
        public static Vector3 RenderArc(float speed, float angle, int numberOfLines, Transform player, Transform projectile, LineRenderer lineRenderer, LayerMask lm)
        {
            // 1 point for the start of each line + 1 at the end of the arc.
            lineRenderer.positionCount = numberOfLines + 1;

            List<Vector3> positions = new List<Vector3>();
            //Vector3[] positions = new Vector3[numberOfLines + 1];

            // the angle must be in radian in order to use Unity's Mathf
            float radianAngle = Mathf.Deg2Rad * angle;

            // we want the negative value as our formula's assume gravity is negative.
            float gravity = -Physics.gravity.y;

            float trajectoryDistance = TrajectoryDistance(speed, radianAngle, gravity, projectile.position.y);
            Vector3 direction = CalculateTrajectoryDirection(player, projectile, trajectoryDistance);

            for (int i = 0; i < numberOfLines + 1; i++)
            {
                // cast numberOfLines to float so the answer is calculated as a float
                float t = i / (float)numberOfLines;
                float x = t * trajectoryDistance;

                float y = x * Mathf.Tan(radianAngle) - ((gravity * x * x) / (2 * speed * speed * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));

                positions.Add((y * Vector3.up) + (x * direction) + projectile.position);

                if (i > 0)
                {
                    if (CheckTrajectoryCollision(positions[i - 1], positions[i], out RaycastHit hit, lm))
                    {
                        positions[i] = hit.point;
                        break;
                    }
                }
            }

            lineRenderer.positionCount = positions.Count;

            lineRenderer.SetPositions(positions.ToArray());

            return CalculateVelocity(speed, direction, radianAngle);
        }
        public static float CalculateAngle(Transform playerHead)
        {
            float headAngle = -1 * playerHead.transform.rotation.eulerAngles.x;
            float angle = ClampAngle(headAngle + 30, -90, 89.99f);
            return angle;
        }

        private static float TrajectoryDistance(float speed, float angle, float gravity, float initialHeight = 0f)
        {
            float xSpeed = Mathf.Cos(angle) * speed;
            float ySpeed = Mathf.Sin(angle) * speed;
            return (xSpeed / gravity) * (ySpeed + Mathf.Sqrt(ySpeed * ySpeed + 2f * gravity * initialHeight));
        }

        private static Vector3 CalculateTrajectoryDirection(Transform player, Transform projectile, float trajectoryDistance)
        {
            Vector3 direction = player.forward * trajectoryDistance + player.position - projectile.position;

            // we only want the vector in the xz direction
            direction.y = 0;

            // only want direction not magnitude
            return direction.normalized;
        }


        private static float ClampAngle(float angle, float min, float max)
        {
            // makes sure angle is between -180 and 180
            while (angle <= -180)
            {
                angle += 360;
            }
            while (angle > 180)
            {
                angle -= 360;
            }

            // clamps angle within min and max
            return Mathf.Clamp(angle, min, max);
        }

        private static Vector3 CalculateVelocity(float speed, Vector3 direction, float radianAngle)
        {
            float yDirection = Mathf.Tan(radianAngle);

            Vector3 finalDirection = new Vector3(direction.x, yDirection, direction.z);

            return speed * finalDirection.normalized;
        }

        public static bool CheckTrajectoryCollision(Vector3 start, Vector3 end, out RaycastHit hit, LayerMask layerMask)
        {
            return Physics.Raycast(start, end - start, out hit, (end - start).magnitude) && LayerMaskUtility.IsInLayerMask(hit.transform.gameObject.layer, layerMask);
        }
    }
}