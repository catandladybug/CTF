using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RedBase : MonoBehaviourPun
{

    //public GameObject redHatStand;
    public GameObject redHat;

    public static RedBase instance;

    void Awake()
    {

        instance = this;

    }

    [PunRPC]
    public void RedHatCaptured()
    {

        redHat.gameObject.SetActive(false);

    }

    [PunRPC]
    public void RedHatReturned()
    {

        redHat.gameObject.SetActive(true);

    }

}
