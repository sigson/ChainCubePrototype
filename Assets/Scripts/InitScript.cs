using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScript : MonoBehaviour
{
    private void Awake()
    {
        LocalisationRegistrator.InitDB();
    }
    void Start()
    {
        LocalisationRegistrator.UpdateAllAnchors();
    }
}
