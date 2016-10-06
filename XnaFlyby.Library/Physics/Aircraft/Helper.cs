// <copyright file="Helper.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Library.Physics.Aircraft
{
    using System;

    /// <summary>
    /// Helper class for basic aircraft physics.
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Calculates the lift coefficient for cambered airfoil with a plain trailing edge flap.
        /// </summary>
        /// <param name="angle">The attack angle.</param>
        /// <param name="flaps">The status of the flaps.</param>
        /// <returns>The lift coefficient for the given attack angle and status of the flaps.</returns>
        public static float CalculateFlapLiftCoefficient(float angle, int flaps)
        {
            // lift coefficients for neutral flap position
            float[] flapNeutralLiftCoefficients = { -0.54f, -0.2f, 0.2f, 0.57f, 0.92f, 1.21f, 1.43f, 1.4f, 1.0f };

            // lift coefficients for flap down position
            float[] flapDownLiftCoefficients = { 0.0f, 0.45f, 0.85f, 1.02f, 1.39f, 1.65f, 1.75f, 1.38f, 1.17f };

            // lift coefficients for flap up position
            float[] flapUpLiftCoefficients = { -0.74f, -0.4f, 0.0f, 0.27f, 0.63f, 0.92f, 1.03f, 1.1f, 0.78f };

            // angles which are determining the angle range of a lift coefficient
            float[] angles = { -8.0f, -4.0f, 0.0f, 4.0f, 8.0f, 12.0f, 16.0f, 20.0f, 24.0f };

            return Helper.CalculateCoefficientForFlaps(
                flapNeutralLiftCoefficients,
                flapDownLiftCoefficients,
                flapUpLiftCoefficients,
                angles,
                angle,
                flaps);
        }

        /// <summary>
        /// Calculates the drag coefficient for cambered airfoil with a plain trailing edge flap.
        /// </summary>
        /// <param name="angle">The attack angle.</param>
        /// <param name="flaps">The status of the flaps.</param>
        /// <returns>The drag coefficient for the given attack angle and status of the flaps.</returns>
        public static float CalculateFlapDragCoefficient(float angle, int flaps)
        {
            // drag coefficients for neutral flap position
            float[] flapNeutralDragCoefficients = { 0.01f, 0.0074f, 0.004f, 0.009f, 0.013f, 0.023f, 0.05f, 0.12f, 0.21f };

            // drag coefficients for flap down position
            float[] flapDownDragCoefficients = { 0.0065f, 0.0043f, 0.0055f, 0.0153f, 0.0221f, 0.0391f, 0.1f, 0.195f, 0.3f };

            // drag coefficients for flap up position
            float[] flapUpDragCoefficients = { 0.005f, 0.0043f, 0.0055f, 0.0261f, 0.03757f, 0.06647f, 0.13f, 0.18f, 0.25f };

            // angles which are determining the angle range of a drag coefficient
            float[] angles = { -8.0f, -4.0f, 0.0f, 4.0f, 8.0f, 12.0f, 16.0f, 20.0f, 24.0f };

            return Helper.CalculateCoefficientForFlaps(
                flapNeutralDragCoefficients,
                flapDownDragCoefficients,
                flapUpDragCoefficients,
                angles,
                angle,
                flaps);
        }

        /// <summary>
        /// Calculates the lift coefficient for symmetric, non-cambered airfoil without flaps (e.g. rudder).
        /// </summary>
        /// <param name="angle">The attack angle.</param>
        /// <returns>The lift coefficient for the given attack angle.</returns>
        public static float CalculateRudderLiftCoefficient(float angle)
        {
            float[] rudderLiftCoefficients = { 0.16f, 0.456f, 0.736f, 0.968f, 1.144f, 1.12f, 0.8f };
            float[] angles = { 0.0f, 4.0f, 8.0f, 12.0f, 16.0f, 20.0f, 24.0f };

            float absoluteAngle = Math.Abs(angle);
            float coefficient = 0.0f;

            coefficient = Helper.CalculateCoefficientForRudder(
                rudderLiftCoefficients,
                angles,
                absoluteAngle,
                coefficient);

            // inverts the coefficient's value if we have negative attack angle
            if (angle < 0.0f)
            {
                coefficient = -coefficient;
            }

            return coefficient;
        }

        /// <summary>
        /// Calculates the drag coefficient for symmetric, non-cambered airfoil without flaps (e.g. rudder).
        /// </summary>
        /// <param name="angle">The attack angle.</param>
        /// <returns>The drag coefficient for the given attack angle.</returns>
        public static float CalculateRudderDragCoefficient(float angle)
        {
            float[] rudderDragCoefficients = { 0.0032f, 0.0072f, 0.0104f, 0.0184f, 0.04f, 0.096f, 0.168f };
            float[] angles = { 0.0f, 4.0f, 8.0f, 12.0f, 16.0f, 20.0f, 24.0f };

            float absoluteAngle = Math.Abs(angle);
            float coefficient = 0.5f;

            return Helper.CalculateCoefficientForRudder(
                rudderDragCoefficients,
                angles,
                absoluteAngle,
                coefficient);
        }

        /// <summary>
        /// Calculates the lift coefficient for the specified flap position.
        /// </summary>
        /// <param name="flapNeutralCoefficients">The flap neutral coefficients.</param>
        /// <param name="flapDownCoefficients">The flap down coefficients.</param>
        /// <param name="flapUpCoefficients">The flap up coefficients.</param>
        /// <param name="angles">The range of angles.</param>
        /// <param name="angle">The attack angle.</param>
        /// <param name="flaps">The status of the flaps.</param>
        /// <returns>The drag coefficient for the given coefficients, angle ranges, attack angle and status of the flaps.</returns>
        private static float CalculateCoefficientForFlaps(
            float[] flapNeutralCoefficients,
            float[] flapDownCoefficients,
            float[] flapUpCoefficients,
            float[] angles,
            float angle,
            int flaps)
        {
            float liftCoefficient = 0.0f;
            for (int i = 0; i < 8; i++)
            {
                if (angles[i] <= angle && angles[i + 1] > angle)
                {
                    float[] currentCoefficients = flapNeutralCoefficients;
                    switch (flaps)
                    {
                        case 0:
                            currentCoefficients = flapNeutralCoefficients;
                            break;
                        case -1:
                            currentCoefficients = flapDownCoefficients;
                            break;
                        case 1:
                            currentCoefficients = flapUpCoefficients;
                            break;
                    }

                    liftCoefficient = currentCoefficients[i]
                        - ((angles[i] - angle)
                        * (currentCoefficients[i] - currentCoefficients[i + 1])
                        / (angles[i] - angles[i + 1]));
                }
            }

            return liftCoefficient;
        }

        /// <summary>
        /// Calculates the coefficient for rudder.
        /// </summary>
        /// <param name="rudderCoefficients">The rudder coefficients.</param>
        /// <param name="angles">The angles.</param>
        /// <param name="absoluteAngle">The absolute angle.</param>
        /// <param name="coefficientStartingValue">The coefficient starting value.</param>
        /// <returns></returns>
        private static float CalculateCoefficientForRudder(
            float[] rudderCoefficients,
            float[] angles,
            float absoluteAngle,
            float coefficientStartingValue)
        {
            float coefficient = coefficientStartingValue;
            for (int i = 0; i < angles.Length; i++)
            {
                if ((angles[i] <= absoluteAngle) && (angles[i + 1] > absoluteAngle))
                {
                    coefficient = rudderCoefficients[i]
                        - ((angles[i] - absoluteAngle)
                        * (rudderCoefficients[i] - rudderCoefficients[i + 1])
                        / (angles[i] - angles[i + 1]));
                    break;
                }
            }

            return coefficient;
        }
    }
}
