using System;
using UnityEngine;

namespace LightBuzz.Vitruvius
{
    /// <summary>
    /// Provides extension methods for transforming quaternions to rotations.
    /// </summary>
    public static class OrientationExtensions
    {
        /// <summary>
        /// Rotates the specified quaternion around the x axis.
        /// </summary>
        /// <param name="quaternion">The orientation quaternion.</param>
        /// <returns>The rotation in degrees.</returns>
        public static double Pitch(this Vector4 quaternion)
        {
            double value1 = 2.0 * (quaternion.w * quaternion.x + quaternion.y * quaternion.z);
            double value2 = 1.0 - 2.0 * (quaternion.x * quaternion.x + quaternion.y * quaternion.y);

            double roll = Math.Atan2(value1, value2);

            return roll * (180.0 / Math.PI);
        }

        /// <summary>
        /// Rotates the specified quaternion around the y axis.
        /// </summary>
        /// <param name="quaternion">The orientation quaternion.</param>
        /// <returns>The rotation in degrees.</returns>
        public static double yaw(this Vector4 quaternion)
        {
            double value = +2.0 * (quaternion.w * quaternion.y - quaternion.z * quaternion.x);
            value = value > 1.0 ? 1.0 : value;
            value = value < -1.0 ? -1.0 : value;

            double pitch = Math.Asin(value);

            return pitch * (180.0 / Math.PI);
        }

        /// <summary>
        /// Rotates the specified quaternion around the z axis.
        /// </summary>
        /// <param name="quaternion">The orientation quaternion.</param>
        /// <returns>The rotation in degrees.</returns>
        public static double Roll(this Vector4 quaternion)
        {
            double value1 = 2.0 * (quaternion.w * quaternion.z + quaternion.x * quaternion.y);
            double value2 = 1.0 - 2.0 * (quaternion.y * quaternion.y + quaternion.z * quaternion.z);

            double yaw = Math.Atan2(value1, value2);

            return yaw * (180.0 / Math.PI);
        }
    }
}
