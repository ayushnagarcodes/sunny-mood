using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    private int currentLevel;
    private int levelCompleted;

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        levelCompleted = PlayerPrefs.GetInt("LevelCompleted");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (levelCompleted < 3)
            {
                if (currentLevel < 3)
                {
                    PlayerPrefs.SetInt("LevelCompleted", currentLevel);
                    currentLevel++;
                }
                else
                {
                    PlayerPrefs.SetInt("LevelCompleted", currentLevel);
                    currentLevel = 1;
                }
            }
            else
            {
                if (currentLevel < 3) {currentLevel++;}
                else {currentLevel = 1;}
            }
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            SceneManager.LoadScene($"Level {currentLevel}");
        }
    }
}
