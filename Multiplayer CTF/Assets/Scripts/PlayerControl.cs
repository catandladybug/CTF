using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerControl : MonoBehaviourPun
{

    [Header("Info")]
    public int id;
    private int curAttackerId;
  

    [Header("Stats")]
    public float moveSpeed;
    public float jumpForce;
    public int curHp;
    public int maxHp;
    public int kills;
    public bool dead;
    public int playerTeam;

    private bool flashingDamage;

    [Header("Components")]
    public Rigidbody rig;
    public Player photonPlayer;
    public MeshRenderer mr;
    public PlayerWeapon weapon;
   

    [Header("CTF")]
    public bool hasBlueFlag = false;
    public bool hasRedFlag = false;
    public bool blueFlagGone = false;
    public bool redFlagGone = false;
    public GameObject blueHatObject;
    public GameObject redHatObject;
   


    [PunRPC]
    public void Initialize (Player player)
    {

        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        if (id == 1 || id == 3 || id == 5 || id == 7)
        {

            playerTeam = 1;
            mr.material.color = Color.blue;

        }
        else
        {

            playerTeam = 2;
            mr.material.color = Color.red;

        }

        if (!photonView.IsMine)
        {

            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;

        }
        else
        {

            GameUI.instance.Initialize(this);

        }

        if( playerTeam == 1 )
        {

            transform.position = new Vector3(114, -27, 112);

        }
        else if( playerTeam == 2 )
        {

            transform.position = new Vector3(-125, -26, -79);

        }

    }

    void Update()
    {

        if (!photonView.IsMine || dead)
            return;

        Move();

        if (Input.GetKeyDown(KeyCode.Space)) 
            TryJump();

        if(Input.GetMouseButtonDown(0))
            weapon.TryShoot();

        //Leaderboard.instance.SetLeaderboardEntry(Mathf.RoundToInt(kills));

    }

    void Move()
    {

        // get input axis
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // calculate a direction relative to where we're facing
        Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
        dir.y = rig.velocity.y;

        // set that as velocity
        rig.velocity = dir;

    }

    void TryJump()
    {

        // create ray facing down
        Ray ray = new Ray(transform.position, Vector3.down);

        // shoot raycast
        if (Physics.Raycast(ray, 1.5f))
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    }

    [PunRPC]
    public void TakeDamage (int attackerId, int damage)
    {

        if (dead)
            return;

        curHp -= damage;
        curAttackerId = attackerId;

        // flash player red
        photonView.RPC("DamageFlash", RpcTarget.Others);

        // update the health bar
        GameUI.instance.UpdateHealthBar();

        // die if no health left
        if (curHp <= 0)
            photonView.RPC("Die", RpcTarget.All);
    }

    [PunRPC]
    void DamageFlash ()
    {

        if (flashingDamage)
            return;

        StartCoroutine(DamageFlashCoRoutine());

        IEnumerator DamageFlashCoRoutine ()
        {

            flashingDamage = true;

            Color defaultColor = mr.material.color;
            mr.material.color = Color.yellow;

            yield return new WaitForSeconds(0.05f);

            mr.material.color = defaultColor;
            flashingDamage = false;

        }


    }

    [PunRPC]
    void Die ()
    {

        curHp = 0;
        dead = true;

        GameManager.instance.alivePlayers--;

        // is this our local player?
        if(photonView.IsMine)
        {

            if (curAttackerId != 0)
                GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);

            if (playerTeam == 1)
            {

                transform.position = new Vector3(114, -27, 112);
                curHp = 30;
                GameUI.instance.UpdateHealthBar();
                dead = false;

                if (hasRedFlag == true)
                {

                    RedBase.instance.photonView.RPC("RedHatReturned", RpcTarget.All);

                    photonView.RPC("LoseRedHat", RpcTarget.All);

                    hasRedFlag = false;

                }

            }
            else if (playerTeam == 2)
            {

                transform.position = new Vector3(-125, -26, -79);
                curHp = 100;
                GameUI.instance.UpdateHealthBar();
                dead = false;

                if (hasBlueFlag == true)
                {

                    BlueBase.instance.photonView.RPC("BlueHatReturned", RpcTarget.All);

                    photonView.RPC("LoseBlueHat", RpcTarget.All);

                    hasBlueFlag = false;

                }

            }

        }

    }

    [PunRPC]
    public void AddKill ()
    {

        kills++;

       // Leaderboard.instance.SetLeaderboardEntry(Mathf.RoundToInt(kills));

        // update UI
        GameUI.instance.UpdatePlayerInfoText();

    }

    [PunRPC]
    public void Heal (int amountToHeal)
    {

        curHp = Mathf.Clamp(curHp + amountToHeal, 0, maxHp);

        // update health bar UI
        GameUI.instance.UpdateHealthBar();

    }

    [PunRPC]
    public void GetBlueHat()
    {

        blueHatObject.gameObject.SetActive(true);

    }

    [PunRPC]
    public void GetRedHat()
    {

        redHatObject.gameObject.SetActive(true);

    }

    [PunRPC]
    public void LoseBlueHat()
    {

        blueHatObject.gameObject.SetActive(false);

    }

    [PunRPC]
    public void LoseRedHat()
    {

        redHatObject.gameObject.SetActive(false);

    }

    void OnCollisionEnter(Collision collision)
    {

        if (!photonView.IsMine)
            return;

        if (collision.gameObject.name == "RedBody")
        {

            if (playerTeam == 1)
            {

                hasRedFlag = true;

                RedBase.instance.photonView.RPC("RedHatCaptured", RpcTarget.All);

                photonView.RPC("GetRedHat", RpcTarget.All);

                //GameManager.instance.photonView.RPC("RedFlagTaken", RpcTarget.All);

            }

        }

        if (collision.gameObject.name == "BlueCaptureZone")
        {

            if (playerTeam == 1 && hasRedFlag == true)
            {

                Debug.Log("You did it i guess");
                GameManager.instance.photonView.RPC("BlueTeamWins", RpcTarget.All);

            }

        }

        if (collision.gameObject.name == "BlueBody")
        {

            GameManager.instance.photonView.RPC("CheckIfBlueHatGone", RpcTarget.All);

            if (playerTeam == 2)
            {

                hasBlueFlag = true;

                BlueBase.instance.photonView.RPC("BlueHatCaptured", RpcTarget.All);

                photonView.RPC("GetBlueHat", RpcTarget.All);

                GameManager.instance.photonView.RPC("BlueHatGone", RpcTarget.All);

            }

        }

        if (collision.gameObject.name == "RedCaptureZone")
        {

            if (playerTeam == 2 && hasBlueFlag == true)
            {

                Debug.Log("You did it i guess");
                GameManager.instance.photonView.RPC("RedTeamWins", RpcTarget.All);

            }

        }

    }

}
