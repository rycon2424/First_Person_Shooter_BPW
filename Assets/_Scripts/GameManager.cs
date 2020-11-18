using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text enemiesLeft;
    public Text maxEnemies;

    private Enemy[] enemies;
    private int currentEnemies;

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
    }

    public void GameOver()
    {

    }
}
