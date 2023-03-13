using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    private Button[] btns;
    private String btnName;
    
    private void Start()
    {
        btns = GetComponentsInChildren<Button>();

        for (int i = 0; i <= PlayerPrefs.GetInt("LevelCompleted"); i++)
        {
            btns[i].interactable = true;
        }

        for (int i = 0; i < btns.Length - 1; i++)
        {
            btns[i].onClick.AddListener(StartLevel);
        }
    }

    public void StartLevel()
    {
        btnName = EventSystem.current.currentSelectedGameObject.name;
        PlayerPrefs.SetInt("CurrentLevel", Int32.Parse(btnName[^1].ToString()));
        SceneManager.LoadScene(btnName);
    }
}
