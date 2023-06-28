using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, sprintspeed, walkspeed, jumpForce, smoothTime;
    [SerializeField] Item[] items;
    [SerializeField] GameObject cam;
    [SerializeField] TMP_Text Text;
    public bool InZone=true;
   // [SerializeField] GameObject killText;


    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    Rigidbody rb;

    PhotonView PV;

    const float maxHealth = 100f;
    public float currentHealth = maxHealth;
    PlayerManager playerManager;
    // GameObject[] scoreboard;
    //  public CameraShaker cameraShake;
    public CameraShaker cameraShake;
    public bool canMov = true;
    //public bool inShip = false;
    //public bool ImCaptain=false;
    Collider ShipColl;
  //  GameObject Ship;
     public float Speed;
    public float turnspeed;
    public Transform FakeParent=null;
    public const string PlayerItem = "itemIndex";
    private Vector3 _positionOffset;
    private Quaternion _rotationOffset;
    Scoreboard scoreboard;
    float damages=0.01f;
    int playerNo=-1;
    public enum PlayerState {Normal,ShipPassenger,ShipCaptain}
    public  PlayerState state;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        cameraShake = GetComponentInChildren<CameraShaker>();
        // cam = GetComponentInChildren<GameObject>();
        scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<Scoreboard>();
        state=PlayerState.Normal;
    }
    private void Start()
    {  
        if (PV.IsMine)
        {
        Text.text = PhotonNetwork.NickName;
        }
        if (PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }
       // Ship = ShipColl.GetComponent<boatmovement>();
    }
    // Update is called once per frame
    void Update()
    {
        //SWITCH CASE UBAYOGICH  HEALTH AFFECT CHEYYIPIKKKAM CHEYYAM
        if (!InZone)
        {   
            PV.RPC("RPC_TakeDamage", RpcTarget.All, damages.ToString(), playerNo.ToString());
        }

        if (!PV.IsMine)
        {
            return;
        }   
        switch (state)
        {
            default:
            case PlayerState.Normal:
                Look();
                Move();
                Jump();

                //CHANGING WEAPONS
                for (int i = 0; i < items.Length; i++)
                {
                    if (Input.GetKeyDown((i + 1).ToString()))
                    {
                        EquipItem(i);
                        break;
                    }
                }

                if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
                {
                    if (itemIndex >= items.Length - 1)
                    {
                        EquipItem(0);
                    }
                    else
                    {
                        EquipItem(itemIndex + 1);
                    }
                }
                else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
                {
                    if (itemIndex <= 0)
                    {
                        EquipItem(items.Length - 1);
                    }
                    else
                    {
                        EquipItem(itemIndex - 1);
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    items[itemIndex].Use();
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    EnterShip();
                }
               // if (transform.position.y < -30f)
               // {
                  //  Debug.Log("down");
                //    Die(-1);
               // }
                break;


            case PlayerState.ShipPassenger:
                if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    ShipColl.GetComponent<boatmovement>().ChangeCaptain(PhotonNetwork.LocalPlayer, this.gameObject);
                }
                Look();
                Move();
                Jump();
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("Input exit");
                    Exit();
                }
                break;


            case PlayerState.ShipCaptain:
                ShipColl.GetComponent<boatmovement>().state = boatmovement.State.Occupied;
                Look();
              transform.position=ShipColl.gameObject.GetComponent<boatmovement>().Captainpos.position;
                
                
                
                float speed = Input.GetAxisRaw("Vertical") * Speed;
               ShipColl.GetComponent<boatmovement>().transform.Translate(Vector3.forward * Time.smoothDeltaTime * ((speed) + 1));
               float horizontal = Input.GetAxisRaw("Horizontal") * turnspeed;
               ShipColl.GetComponent<boatmovement>().transform.Rotate(Vector3.up * Time.smoothDeltaTime * (int)(30 * horizontal));

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    cam.gameObject.SetActive(!cam.gameObject.activeSelf);
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("Input exit");
                    Exit();
                }
                break;

        }


   //     if (FakeParent != null)
   //     {
   //         var targetPos = FakeParent.position - _positionOffset;
   //         var targetRot = FakeParent.localRotation * _rotationOffset;
   //
   //         transform.position = RotatePointAroundPivot(targetPos, FakeParent.position, targetRot);
   //         transform.localRotation = targetRot;
   //     }
    }

        public void SetFakeParent(Transform parent)
        {
          if (parent != null)
          {   //Offset vector
              _positionOffset = parent.position - transform.position;
            //Offset rotation
              _rotationOffset = Quaternion.Inverse(parent.localRotation * transform.localRotation);
             //Our fake parent
              FakeParent = parent;
          }
         if (parent==null)
          {
              FakeParent = null;
          }
        }

        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            //Get a direction from the pivot to the point
            Vector3 dir = point - pivot;
            //Rotate vector around pivot
            dir = rotation * dir;
            //Calc the rotated vector
            point = dir + pivot;
            //Return calculated vector
            return point;
        }
  
    //  if (transform.position.y < -20f)
    //  {
    //      Die(-1);
    //  }

    private void Exit()
    {
        
                    ShipColl.gameObject.GetComponent<boatmovement>().Exit(PhotonNetwork.LocalPlayer,this.gameObject);
                    Debug.Log("input msg forwarded");
                   if (cam.gameObject.activeSelf == false) 
                   {
                      cam.gameObject.SetActive(true);
                   }
                    //canMov = true;s
                
            
        
    }

    void Move()
    {
      //  if (canMov)
      //  {
            Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintspeed : walkspeed), ref smoothMoveVelocity, smoothTime);
      //  }
    }
    void Look()
    {

        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;

    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex) { return; }

        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);
        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }
        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
               hash.Add("itemIndex", itemIndex);
          //  hash[PlayerItem] = itemIndex;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        //Hashtable hash = new Hashtable();
         if (changedProps.ContainsKey("itemIndex"))
         {
            if (!PV.IsMine && targetPlayer == PV.Owner)
            {
                EquipItem((int)changedProps["itemIndex"]);
                Debug.Log("Changing Weapons");
            }
         }
       
    }
    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }
    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        }
    }
    public void TakeDamage(string damage, string player)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, player);
    }
   [PunRPC]
    void RPC_TakeDamage(string damage, string player)
    {
        //  int damage;
        //  int player;
        //  damage=aray[0] ;
      
        if (!PV.IsMine)
        {
            return;
        }

      //  currentHealth -= System.Convert.ToInt32(damage);
        currentHealth -= float.Parse(damage);
        healthbarImage.fillAmount = currentHealth / maxHealth;
        if (InZone)
        {
        Shake();
        }
        if (System.Convert.ToInt32(player)>=0)     
        {
            if (currentHealth <= 0)
            {
                Die(System.Convert.ToInt32(player));
            }
        }
        if(System.Convert.ToInt32(player) <0)
        {
            if (currentHealth <= 0)
            {
                Die(-1);
            }
        }
        //d
        //  Debug.Log("took damage" + damage);
    }
    void Shake()
    {
        //   StartCoroutine(cameraShake.Shake(0.15f, 0.2f));
        cameraShake.Shake();
    }
    void Die(int player)
    {
        if (player >= 0) { 
        FindKiller(player);
        }
        playerManager.Die();
        PhotonNetwork.LocalPlayer.AddDeaths();
    }
    public void FindKiller(int ActorNo)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == ActorNo)
            {
                player.AddKills();
               // scoreboard.InstantiateKillFeed(player, PhotonNetwork.LocalPlayer);
                // Debug.Log(ActorNo);
                Debug.Log("adding kills");
                PV.RPC("RPC_KillFeed", RpcTarget.All,player,PhotonNetwork.LocalPlayer);
            }
        }
    }
    [PunRPC]
    void  RPC_KillFeed(Player player,Player victim)
    {
        scoreboard.InstantiateKillFeed(player,victim);
    }

    
    

    void EnterShip()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 6f);

        if (hitColliders != null)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.CompareTag("Ship"))
                {
                    hitCollider.gameObject.GetComponent<boatmovement>().Run(PhotonNetwork.LocalPlayer,this.gameObject);
                    ShipColl = hitCollider;
                    //scanMov = false;
                }
            }
        }
    }

  //  private void OnDrawGizmosSelected()
  //  {
  //      Gizmos.DrawWireSphere(transform.position, 6);
  //  }
}
