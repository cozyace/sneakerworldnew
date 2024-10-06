// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

namespace SneakerWorld.Main {

    public class PercentileDictionary<T> where T : Enum {

        public static Dictionary<T, float> dict;
        private T maxEnum;

        // public class SamplePacket<T> {
        //     public T value;
        //     public float percentile;
        // }

        public PercentileDictionary(Dictionary<T, int> wDict, T maxEnum) {
            
            // Calculate the total value of the weights.
            int totalValue = 0;
            foreach (var kv in wDict) {
                items.Add(kv.Key, kv.Value);
                totalValue += kv.Value;
            }

            // Collect the items in a sorted list.
            List<(T, int)> items = new List<(T, int)>();
            items.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            // Add the percentage values to a percent dict.
            dict = new Dictionary<T, float>();
            foreach (var itemSet in items) {
                percentDict.Add(itemSet.Item1, (float)(itemSet.Item2 / total));
            }

            this.maxEnum = maxEnum;
            
        }

        // Sample a percentage dict for its corresponding enum at the given probability value.
        public static (T, float) Sample(float prob) {

            // Convert the dict into arrays based on the order of the enums.
            T[] enums = new T[maxEnum.ToInt()];
            float[] dis = new float[maxEnum.ToInt()];
            float[] cumDis = new float[maxEnum.ToInt()];
            foreach (var kv in dict) {
                enums[kv.Key.ToInt()] = kv.Key;
                dis[kv.Key.ToInt()] = kv.Value;
            }

            // Calculate the cumulative probability distribution.
            cumDis[0] = dis[0];
            for (int i = 1; i < dis.Length; i++) {
                cumDis[i] = dis[i] + cumDis[i-1];
            }

            // Return the corresponding enum.
            for (int i = 0; i < cumDis.Length; i++) {
                if (prob < cumDis[i]) {
                    return (enums[i], cumDis[i]);
                }
            }
            return (enums[enums.Length-1], 1f);

        }

        // Get the minimum probability value that could result in this enum value.
        public static float InvSample(T t) {

            // Convert the dict into arrays based on the order of the enums.
            int index = 0;
            float[] dis = new float[maxEnum.ToInt()];
            float[] cumDis = new float[maxEnum.ToInt()];
            foreach (var kv in dict) {
                if (kv.Key.Equals(t)) {
                    index = kv.Key.ToInt();
                }
                dis[kv.Key.ToInt()] = kv.Value;
            }

            // Get the cumulative distribution.
            cumDis[0] = dis[0];
            for (int i = 1; i < dis.Length; i++) {
                cumDis[i] = dis[i] + cumDis[i-1];
            }

            // Return the value at the corresponding index.
            return cumDis[index];

        }


    }

}
