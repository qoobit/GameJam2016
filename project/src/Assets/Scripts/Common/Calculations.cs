using UnityEngine;
using System.Collections;

namespace Common
{
    public class Calculations
    {
        /// <summary>Calculate a projectile's launch vector for hitting targets. Returns false if no solution is found.</summary>
        /// <param name="origin">Starting position</param>
        /// <param name="target">Target position</param>
        /// <param name="speed">Projectile launch speed</param>
        /// <param name="launchVelocity">The calculated Vector3 solution</param>
        /// <param name="weight">
        ///		(Optional) When two solutions are found, one will be selected at random. This parameter will weight the solutions.
        ///		1: 100% chance to pick the smaller angle, 0: 100% chance to pick the larger angle, 0.5: 50% chance to pick either angle (Default).
        /// </param>
        /// 
        public static bool ProjectileLaunchVector3(Vector3 origin, Vector3 target, float speed, out Vector3 launchVelocity, float weight = 0.5f)
        {
            // If gravity is not pointing straight down, rotate our vectors for calculations
            Quaternion gravityRotation = Quaternion.FromToRotation(-Vector3.up, Physics.gravity.normalized);
            Quaternion gravityRotationInverse = Quaternion.Inverse(gravityRotation);
            Vector3 gravityRotated = gravityRotationInverse * Physics.gravity;
            Vector3 delta = target - origin;
            Vector3 deltaRotated = gravityRotationInverse * delta;

            //calculate our x,z hypotenuse
            float deltaH = Mathf.Sqrt(Mathf.Pow(deltaRotated.x, 2) + Mathf.Pow(deltaRotated.z, 2));

            float[] angles;
            if (!TrajectoryAngle(speed, deltaH, delta.y, gravityRotated.y, out angles))
            {
                launchVelocity = Vector3.zero;
                return false;   //no solutions found
            }

            float theta;
            switch (angles.Length)
            {
                case 0:
                    launchVelocity = Vector3.zero;
                    return false;
                case 1:
                    theta = angles[0];
                    break;
                case 2:
                default:
                    float r = Random.Range(0f, 1f);
                    if (r >= weight)
                        theta = Mathf.Max(angles[0], angles[1]);
                    else
                        theta = Mathf.Min(angles[0], angles[1]);
                    break;
            }

            // Calculate our x,y,z from angle
            float y = Mathf.Sin(theta) * speed; //Vertical
            float h = Mathf.Cos(theta) * speed; //x,z hypotenuse
            float x = (h * deltaRotated.x) / deltaH;
            float z = (h * deltaRotated.z) / deltaH;
            Vector3 launchVelocityRotated = new Vector3(x, y, z);

            // Undo our earlier gravity rotation to get real world velocity
            launchVelocity = gravityRotation * launchVelocityRotated;
            return true;
        }

        /// <summary>Calculate the angle required to hit a target. Returns false if no solution is found.</summary>
        /// <param name="launchSpeed">The projectile launch speed</param>
        /// <param name="targetX">The target X position</param>
        /// <param name="targetY">The target Y position</param>
        /// <param name="gravity">Gravity in Y direction</param>
        /// <param name="angles">Output for the solution found</param>
        public static bool TrajectoryAngle(float launchSpeed, float targetX, float targetY, float gravity, out float[] angles)
        {
            float v = launchSpeed;
            float x = targetX;
            float y = targetY;
            float g = -gravity;

            // Trajectory formula 
            // theta = Mathf.Atan((Mathf.Pow(v, 2) + Mathf.Sqrt(Mathf.Pow(v, 4) - g * (g * Mathf.Pow(x, 2) + 2 * y * Mathf.Pow(v, 2)))) / (g * x));

            float vSqr = Mathf.Pow(v, 2);
            float vPow4 = Mathf.Pow(v, 4);
            float xSqr = Mathf.Pow(x, 2);
            float sqrtInside = vPow4 - g * (g * xSqr + 2 * y * vSqr);

            //Check for imaginary numbers
            if (sqrtInside < 0)
            {
                angles = null;
                return false;
            }

            float top1 = vSqr + Mathf.Sqrt(sqrtInside);
            float top2 = vSqr - Mathf.Sqrt(sqrtInside);
            float bottom = (g * x);
            float theta1 = Mathf.Atan(top1 / bottom); //Solution 1
            float theta2 = Mathf.Atan(top2 / bottom); //Solution 2
            angles = new float[2] { theta1, theta2 };

            return true;
        }
        /// <summary>
        /// Solves for zeros in quadratic fomula. Returns true if valid solution is found. Outputs the results to zeros parameter.
        /// </summary>
        /// <param name="a">Constant a in quadratic formula</param>
        /// <param name="b">Constant b in quadratic formula</param>
        /// <param name="c">Constant c in quadratic formula</param>
        /// <returns></returns>
        public static bool QuadraticZeros(float a, float b, float c, out float[] zeros)
        {
            //quadratic formula
            //x = (-b +/- root(b^2 - 4ac) ) / 2a;

            if (a == 0)
                throw new System.Exception("Unable to find zeros due to division by 0");

            float rootInside = Mathf.Pow(b, 2) - (4 * a * c);

            if (rootInside < 0)
            {
                //No solution found
                zeros = null;
                return false;
            }

            float rootSolved = Mathf.Sqrt(rootInside);

            zeros = new float[2];
            zeros[0] = (-b - rootSolved) / (2 * a);
            zeros[1] = (-b + rootSolved) / (2 * a);

            return true;
        }

        /// <summary>
        /// Returns an angle between -180 and 180 degrees
        /// </summary>
        /// <param name="angle">The angle </param>
        /// <returns></returns>
        public static float ClampAngle180(float angle)
        {
            angle %= 360f;
            if (angle >= 180f)
                angle -= 360f;
            return angle;
        }
    }
}
