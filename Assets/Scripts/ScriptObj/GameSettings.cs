using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameSettings", menuName = "Data Objects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public SerializableDictionary<string, SerializableDictionary<string, Value>> GameSettingsDB;
}

[System.Serializable]
public class Value
{
    public float numValue = 0;
    public string stringValue = "";
    public Vector3 vector3Value = Vector3.zero;
    public bool boolValue = false;
}