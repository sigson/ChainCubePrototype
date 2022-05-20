using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public GameManager gameManager;
    public List<TextMesh> textArray;
    public string poolName;
    public int value;
    public Color color;
    public bool includeFirstCollise;
    //public int FontSizeStart = 100;
    //public int StarFonttUnscalerValue = 30;
    //public float StepUnscalerValue = 10;

    private Dictionary<int, float> countNumToTextSizeComparsion = new Dictionary<int, float> {
        {1, 1f },
        {2, 0.7f },
        {3, 0.5f },
        {4, 0.4f },
        {5, 0.3f },
        {6, 0.25f },
        {7, 0.2f },
        {8, 0.18f },
        {9, 0.17f },
        {10, 0.15f },
        {11, 0.14f },
        {12, 0.13f },
        {13, 0.12f },
        {14, 0.11f },
        {15, 0.10f },
    };

    private Material cubeMaterial;
    private void Awake()
    {
        cubeMaterial = this.GetComponent<MeshRenderer>().material;
    }

    public void UpdateCube()
    {
        var boxColl = this.GetComponent<BoxCollider>();
        boxColl.enabled = false;
        boxColl.enabled = true;
        cubeMaterial.SetColor("_Color", color);
        textArray.ForEach((text) =>
        {
            text.text = value.ToString();
            text.characterSize = countNumToTextSizeComparsion[value.ToString().Length];
        });
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == this.gameObject.layer)
        {
            var otherScript = other.gameObject.GetComponent<CubeScript>();
            if (otherScript.value == value)
                gameManager.ColliseCubes(this, otherScript);
            if(includeFirstCollise)
            {
                gameManager.Timeout = false;
                includeFirstCollise = false;
            }
        }
    }
}
