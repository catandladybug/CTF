using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BlueBase : MonoBehaviourPun
{

    //public GameObject redHatStand;
    public GameObject blueHat;

    public static BlueBase instance;

    void Awake()
    {

        instance = this;

    }

    [PunRPC]
    public void BlueHatCaptured()
    {

        blueHat.gameObject.SetActive(false);

    }

    [PunRPC]
    public void BlueHatReturned()
    {

        blueHat.gameObject.SetActive(true);

    }


}