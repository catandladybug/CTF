using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPun
{

    public float postGameTime;

    [Header("Players")]
    public string playerPrefabLocation;
    public PlayerControl[] players;
    public int alivePlayers;
    public bool redHatGone = false;
    public bool blueHatGone = false;

    [Header("Team Stuff")]
    public int nextPlayersTeam;
    public Transform[] spawnPoints;

    private int playersInGame;

    // instance
    public static GameManager instance;

    void Awake()
    {
        
        instance = this;

    }

    void Start()
    {
        
        players = new PlayerControl[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);

    }

    [PunRPC]
    void ImInGame ()
    {

        playersInGame++;

        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
            photonView.RPC("SpawnPlayer", RpcTarget.All);

    }

    [PunRPC]
    void SpawnPlayer ()
    {

            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

            // initialize player for all players
            playerObj.GetComponent<PlayerControl>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        
    }

    public PlayerControl GetPlayer (int playerId)
    {

        foreach(PlayerControl player in players)
        {

            if(player != null && player.id == playerId)
                return player;

        }

        return null;

    }

    public PlayerControl GetPlayer (GameObject playerObject)
    {

        foreach (PlayerControl player in players)
        {

            if (player != null && player.gameObject == playerObject)
                return player;

        }

        return null;

    }

    public void CheckWinCondition ()
    {

        if (alivePlayers == 1)
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
        

    }

    [PunRPC]
    public void BlueTeamWins()
    {

        GameUI.instance.SetBlueWinText();

        Invoke("GoBackToMenu", postGameTime);

    }

    [PunRPC]
    public void RedTeamWins()
    {

        GameUI.instance.SetRedWinText();

        Invoke("GoBackToMenu", postGameTime);

    }

    /*[PunRPC]
    public void RedFlagTaken ()
    {
        Debug.Log("message suppose to play1");
        GameUI.instance.SetRedWarning();

    }

    [PunRPC]
    public void BlueFlagTaken()
    {
        Debug.Log("message suppose to play1");
        GameUI.instance.SetBlueWarning();

    }*/

    void GoBackToMenu ()
    {

        NetworkManager.instance.ChangeScene("Menu");

    }

}
