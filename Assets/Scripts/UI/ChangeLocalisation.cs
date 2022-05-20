using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ChangeLocalisation : MonoBehaviour
{
    public GameSettings gameSettings;
    public void OnChangeLocalisation()
    {
        LocalisationRegistrator.UpdateLocale(gameSettings.GameSettingsDB["lang"].Where((x) => x.Key != LocalisationRegistrator.nowLocale).ToList()[0].Key);
        LocalisationRegistrator.UpdateAllAnchors();
    }
}
