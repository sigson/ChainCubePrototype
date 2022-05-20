using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StartGame : MonoBehaviour
{
    public GameObject GameRestartButton;
    public GameObject StatsText;
    public GameManager gameManager;
    public void OnStartGame()
    {
        this.gameObject.SetActive(false);
        GameRestartButton.SetActive(true);
        StatsText.SetActive(true);
        gameManager.GenerateGame();
    }
}
