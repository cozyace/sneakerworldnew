// System.
using System;
using System.Threading.Tasks;
using System.Globalization;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    /// <summary>
    /// A simple data structure to store / construct market events with.
    /// </summary>
    [System.Serializable]
    public class FeaturedItem {

        public const string DATE_TIME_FORMAT = "yyyy-MM-dd";

        // The details of this event.
        public string itemId;

        // The date this market event is meant to occur.
        public int day = 01;
        public int month = 01;
        public int year = 2024;
        // public string timeId => GetTimeId();

        public string GetTimeId() {
            return GetTimeId(day, month, year);
        }

        public static string GetTimeId(int day, int month, int year) {
            return new DateTime(year, month, day).ToString(DATE_TIME_FORMAT, DateTimeFormatInfo.InvariantInfo);
        }

    }

}


