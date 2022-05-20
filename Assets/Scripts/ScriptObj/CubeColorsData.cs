using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AllColors", menuName = "Data Objects/CubeColorsData", order = 1)]
public class CubeColorsData : ScriptableObject
{
    public SerializableDictionary<string, SerializableDictionary<int, Color>> ColorsSetup;
}
