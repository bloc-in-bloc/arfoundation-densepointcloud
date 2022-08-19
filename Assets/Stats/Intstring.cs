using UnityEngine;

namespace BlocInBloc.Stats {
    public static class IntString {
        /// <summary>
        /// List of negative ints casted to strings.
        /// </summary>
        private static string[] m_negativeBuffer = new string[0];

        /// <summary>
        /// List of positive ints casted to strings.
        /// </summary>
        private static string[] m_positiveBuffer = new string[0];

        /// <summary>
        /// The lowest int value of the existing number buffer.
        /// </summary>
        public static int MinValue => -(m_negativeBuffer.Length - 1);

        /// <summary>
        /// The highest int value of the existing number buffer.
        /// </summary>
        public static int MaxValue => m_positiveBuffer.Length;

        /// <summary>
        /// Initialize the buffers.
        /// </summary>
        /// <param name="minNegativeValue">
        /// Lowest negative value allowed.
        /// </param>
        /// <param name="maxPositiveValue">
        /// Highest positive value allowed.
        /// </param>
        public static void Init (int minNegativeValue, int maxPositiveValue) {
            if (MinValue > minNegativeValue && minNegativeValue <= 0) {
                int length = Mathf.Abs (minNegativeValue);

                m_negativeBuffer = new string[length];

                for (int i = 0; i < length; i++) {
                    m_negativeBuffer[i] = (-i - 1).ToString ();
                }
            }

            if (MaxValue < maxPositiveValue && maxPositiveValue >= 0) {
                m_positiveBuffer = new string[maxPositiveValue + 1];

                for (int i = 0; i < maxPositiveValue + 1; i++) {
                    m_positiveBuffer[i] = i.ToString ();
                }
            }
        }

        public static void Dispose () {
            m_negativeBuffer = new string[0];
            m_positiveBuffer = new string[0];
        }

        /// <summary>
        /// Returns this int as a cached string.
        /// </summary>
        /// <param name="value">
        /// The required int.
        /// </param>
        /// <returns>
        /// A cached number string if within the buffer ranges.
        /// </returns>
        public static string ToStringNonAlloc (this int value) {
            if (value < 0 && -value <= m_negativeBuffer.Length) {
                return m_negativeBuffer[-value - 1];
            }

            if (value >= 0 && value < m_positiveBuffer.Length) {
                return m_positiveBuffer[value];
            }

            // If the value is not within the buffer ranges, just do a normal .ToString()
            return value.ToString ();
        }
    }
}