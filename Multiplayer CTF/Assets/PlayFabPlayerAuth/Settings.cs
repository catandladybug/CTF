using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
   public static bool GameIsPaused = false;

    public GameObject settingsMenuUI;
    public Button backButton;
    public Button leaderboardButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        //bullets.SetActive(true);
    }

    void Pause()
    {
        settingsMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        //bullets.SetActive(false);
    }

    public void OnBackButton()
    {
        settingsMenuUI.SetActive(false);
    }

    public void OnLeaderboardButton()
    {



    }
}
