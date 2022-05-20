using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LocalisationAnchor : MonoBehaviour
{
    public LocalisationGameObjects thisObject;
    public string dbText;
    public string localisedText;

    public void Awake()
    {
        LocalisationRegistrator.sceneAnchors.Add(this);
    }

    public void OnDestroy()
    {
        LocalisationRegistrator.sceneAnchors.Remove(this);
    }
    public void UpdateObjects()
    {
        if(thisObject == LocalisationGameObjects.Text)
        {
            this.localisedText = LocalisationRegistrator.localisationDB.LocalisationObject.Where((x) => x.TextObject.text == dbText).ToList()[0].TextObject.variants.Where((x) => x.locale == LocalisationRegistrator.nowLocale).ToList()[0].variant;
            this.GetComponent<Text>().text = this.localisedText;
        }
    }
}

public enum LocalisationGameObjects
{
    TextInput,
    Text,
    Text3D
}

public static class LocalisationRegistrator
{
    public static List<LocalisationAnchor> sceneAnchors = new List<LocalisationAnchor>();
    public static string nowLocale = "";
    public static LocalisationDB localisationDB;

    public static void InitDB()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW reader = new WWW(Application.streamingAssetsPath + "/localisation.json");
            while (!reader.isDone) { }

            localisationDB = JsonUtility.FromJson<LocalisationDB>(reader.text);
            reader = new WWW(Application.streamingAssetsPath + "/locale.txt");
            while (!reader.isDone) { }
            nowLocale = reader.text;
        }
        else
        {
            nowLocale = File.ReadAllText(Application.streamingAssetsPath + "/locale.txt");
            var data = File.ReadAllText(Application.streamingAssetsPath + "/localisation.json");
            localisationDB = JsonUtility.FromJson<LocalisationDB>(data);
        }
        
    }

    public static void UpdateAllAnchors()
    {
        sceneAnchors.ForEach((x) => x.UpdateObjects());
    }

    public static void UpdateLocale(string newLocale)
    {
        //File.WriteAllText(Application.streamingAssetsPath + "/locale.txt", newLocale);
        nowLocale = newLocale;
    }
}

#region localisation

[System.Serializable]
public class LocalisationObject
{
    public TextObject TextObject;
}

[System.Serializable]
public class LocalisationDB
{
    public List<LocalisationObject> LocalisationObject;
}

[System.Serializable]
public class TextObject
{
    public string text;
    public List<Variant> variants;
}

[System.Serializable]
public class Variant
{
    public string locale;
    public string variant;
}

#endregion