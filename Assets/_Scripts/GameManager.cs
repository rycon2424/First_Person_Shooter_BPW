using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text enemiesLeft;
    public Text maxEnemies;

    private Enemy[] enemies;
    private int currentEnemies;

    public GameObject victory;
    public GameObject defeat;

    void Awake()
    {
        enemies = FindObjectsOfType<Enemy>();
        currentEnemies = enemies.Length;
        maxEnemies.text = currentEnemies.ToString();
        enemiesLeft.text = currentEnemies.ToString();
    }

    public void UpdateManager()
    {
        currentEnemies -= 1;
        enemiesLeft.text = currentEnemies.ToString();
        if (currentEnemies == 0)
        {
            GameOverWin();
        }
    }

    public void Restart()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        EnterLoading.NextScene(currentScene);
    }

    void GameOverWin()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        victory.SetActive(true);
    }

    public void GameOverLost()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        defeat.SetActive(true);
    }


    public void BackToMainMenu()
    {
        GearInstance.instance.DestroyThis();
        EnterLoading.NextScene("MainMenu");
    }
}
