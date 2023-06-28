using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
//using Photon.Realtime;
public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    GameObject controller;
   
    public string CharacterController;
    GameObject[] scoreboard;
    [SerializeField] GameObject killText;
    void Awake()
    {
        PV = GetComponent<PhotonView>();      
    }
   
    // Start is called before the first frame update

    void Start()
    {
      
        if (PV.IsMine)
        {
            CreateController();
        }

    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
      //  base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (changedProps.ContainsKey("kills"))
        {
            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                int kills=PhotonNetwork.LocalPlayer.GetKills();

                if (kills != 0)
                {
                SetKillText();
                }
            }
        }
    }
    // Update is called once per frame
   void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
      //  CharacterController = PlayerPrefs.GetString("Character");
        //controller= PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position,spawnpoint.rotation,0,new object[] { PV.ViewID });
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", PlayerPrefs.GetString("Character")), spawnpoint.position,spawnpoint.rotation,0,new object[] { PV.ViewID });
    }
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
     
       
        CreateController();
    }
    private void SetKillText()
    {
        killText.gameObject.SetActive(true);
        Invoke("DisEngageKillText", 1f);
        Debug.Log("killText");
    }
    void DisEngageKillText()
    {
        killText.gameObject.SetActive(false);
    }
}
