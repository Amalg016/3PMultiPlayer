using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class boatmovement :MonoBehaviourPun, IPunObservable
{
  
    public Transform Captainpos;
    public Transform Playerpos;
    public Transform ExitPos;
  //  bool HaveCaptain=false;
    Player Captain;
    GameObject CAptainContr;
  //  [SerializeField] TMP_Text Text;
    public GameObject cam;
    
    public enum State {Free,Occupied}
    public State state;
   
    
    private void Awake()
    {      
        state = State.Free;
        cam.gameObject.SetActive(false);
    }
    private void Update()
    {
      
        if (state==State.Occupied)
        {
          if (Input.GetKeyDown(KeyCode.Tab))
          {
             cam.gameObject.SetActive(!cam.gameObject.activeSelf);
          }
        }    
    }
    public void ChangeCaptain(Player player, GameObject controller)
    {
        if (state==State.Free)
        {
            setCaptain(player,controller);
        }
        else return;
    }
    void setCaptain(Player player, GameObject controller)
    {
        controller.transform.position = Captainpos.position;    
        this.photonView.TransferOwnership(player);
        controller.GetComponent<PlayerController>().state=PlayerController.PlayerState.ShipCaptain;
       // ChangeShipState();
        Captain = player;
        CAptainContr = controller;
    }
    public void ChangeShipState()
    {
        state = State.Occupied;
        Debug.Log("occupied");
      //  CAptainContr.GetComponent<Rigidbody>().isKinematic = true; 
    }
    public void Run(Player player ,GameObject controller)
    {
     
        if (state==State.Free)
        {
            setCaptain(player,controller);           
        }
        if(player!=Captain)
        {
            controller.transform.position = Playerpos.position;
            controller.gameObject.GetComponent<PlayerController>().state=PlayerController.PlayerState.ShipPassenger;
        }
        // controller.gameObject.GetComponent<PlayerController>().inShip = true;
        // controller.transform.parent = this.gameObject.transform;  
        //    controller.gameObject.GetComponent<PlayerController>().SetFakeParent(this.gameObject.transform); 
       
    }    
    public void Exit(Player player, GameObject controller)
    {
   
      
      
        if (player == Captain)
        {
           // controller.gameObject.GetComponent<PlayerController>().canMov = true;
         //   controller.gameObject.GetComponent<PlayerController>().ImCaptain = false;
          //  controller.gameObject.GetComponent<Rigidbody>().isKinematic= false;
            Captain = null;
          //  HaveCaptain = false;
            state = State.Free;
            cam.gameObject.SetActive(false);
        }
        controller.gameObject.GetComponent<PlayerController>().state=PlayerController.PlayerState.Normal;
      //  controller.gameObject.GetComponent<PlayerController>().SetFakeParent(null);
        controller.transform.position = ExitPos.position;
        controller.GetComponent<Rigidbody>().velocity=Vector3.zero;
        Debug.Log("im out");
        //  controller.transform.parent = null;
        //  controller.gameObject.GetComponent<PlayerController>().inShip = false;       
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
       //     if (!photonView.IsMine)
          //  {
          //      return;
          //  }
               stream.SendNext(state);
        }
       if (stream.IsReading)        {
            state = (State)stream.ReceiveNext();
        }
    }


   
  

  
}
