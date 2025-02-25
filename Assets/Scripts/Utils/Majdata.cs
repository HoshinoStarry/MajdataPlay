﻿#nullable enable
namespace MajdataPlay.Utils
{
    internal static class Majdata<T>
    {
        /// <summary>
        /// Get or set a globally unique instance
        /// </summary>
        public static T? Instance
        {
            get => _instance;
            set
            {
                _instance = value;
            }
        }
        public static bool IsNull => _instance is null;

        static T? _instance = default;

        /// <summary>
        /// Release the instance
        /// </summary>
        public static void Free()
        {
            _instance = default;
        }
    }
}
