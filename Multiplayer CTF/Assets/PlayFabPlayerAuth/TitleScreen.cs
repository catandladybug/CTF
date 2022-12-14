using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleScreen;
    public Button startButton;
    public Button exitButton;

    public GameObject loginRegisterScreen;

    void Start()
    {
        titleScreen.SetActive(true);
    }

    public void OnStartButton()
    {
        titleScreen.SetActive(false);
        loginRegisterScreen.SetActive(true);
    }
}
