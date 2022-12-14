using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{

    public Slider healthBar;
    public TextMeshProUGUI playerInfoText;
    public TextMeshProUGUI ammoText;
    public Image blueWinBackground;
    public Image redWinBackground;

    public GameObject leaderboardCanvas;
    public bool leaderboardActive = false;

    public Image crosshair;


    private PlayerControl player;

    //instance
    public static GameUI instance;

    void Awake()
    {

        instance = this;
        
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {

            if (leaderboardActive == false)
            {
                leaderboardCanvas.SetActive(true);
                Leaderboard.instance.OpenLeaderboard();
                leaderboardActive = true;
            }
            else
            {

                leaderboardCanvas.SetActive(false);
                leaderboardActive = false;

            }

        }


    }

    public void Initialize (PlayerControl localPlayer)
    {
        
        player = localPlayer;
        healthBar.maxValue = player.maxHp;
        healthBar.value = player.curHp;

        UpdatePlayerInfoText();
        UpdateAmmoText();

    }

    public void UpdateHealthBar ()
    {

        healthBar.value = player.curHp;

    }

    public void UpdatePlayerInfoText ()
    {

        playerInfoText.text = /*"<b>Alive:</b> " + GameManager.instance.alivePlayers + */ "\n<b>Kills:</b> " + player.kills;

    }

    public void UpdateAmmoText ()
    {

        ammoText.text = player.weapon.curAmmo + " / " + player.weapon.maxAmmo;

    }

    public void SetBlueWinText ()
    {

        crosshair.gameObject.SetActive(false);
        blueWinBackground.gameObject.SetActive(true);

    }

    public void SetRedWinText ()
    {

        crosshair.gameObject.SetActive(false);
        redWinBackground.gameObject.SetActive(true);

    }
}
