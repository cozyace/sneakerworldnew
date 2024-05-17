using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecorationDatabase : MonoBehaviour
{
    public List<DecorationData> Decorations = new List<DecorationData>();

    public DecorationData GetDecorationByIndex(int index) => Decorations[index];
}

[Serializable]
public struct DecorationData
{
    public string Name;
    public Sprite Icon;
    public DecorationObject Prefab;

    public DecorationData(string name, Sprite icon, DecorationObject prefab)
    {
        Name = name;
        Icon = icon;
        Prefab = prefab;
    }
}
