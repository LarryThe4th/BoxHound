using UnityEngine;
using System.Collections;

namespace BoxHound {
    public class Debuger
    {
        /// <summary>
        /// The switch of debuger functions, set to FALSE by default.
        /// </summary>
        public static bool EnanbeDebuger = true;

        /// <summary>
        /// A overload method of Log().
        /// </summary>
        /// <param name="message">The debug message.</param>
        public static void Log(object message) {
            Log(message, null);
        }

        /// <summary>
        /// A wrapper of Unity's log API.
        /// </summary>
        public static void Log(object message, Object context) {
            if (EnanbeDebuger)
                Debug.Log(message, context);
        }

        /// <summary>
        /// A overload method of LogError().
        /// </summary>
        /// <param name="message">The debug message.</param>
        public static void LogError(object message) {
            LogError(message, null);
        }

        /// <summary>
        /// A wrapper of Unity's LogError API.
        /// </summary>
        public static void LogError(object message, Object context) {
            if (EnanbeDebuger)
                Debug.LogError(message, context);
        }

        /// <summary>
        /// A overload method of LogWarning().
        /// </summary>
        /// <param name="message">The debug message.</param>
        public static void LogWarning(object message)
        {
            LogWarning(message, null);
        }

        /// <summary>
        /// A wrapper of Unity's LogWarning API.
        /// </summary>
        public static void LogWarning(object message, Object context)
        {
            if (EnanbeDebuger)
                Debug.LogWarning (message, context);
        }
    }
}