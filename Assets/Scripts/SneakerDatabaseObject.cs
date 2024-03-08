using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "Sneaker Database", menuName = "New Sneaker Database", order = 1)]
public class SneakerDatabaseObject : ScriptableObject
{
        [FormerlySerializedAs("SneakerDatabase")]
        public List<SneakerInformation> Database = new List<SneakerInformation>();
}

[Serializable]
public struct SneakerInformation
{
        public string Name;
        public string Description;
        public int Value;
        public Sprite Icon;
        public SneakerRarity Rarity;
}

