using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class PlayerController1: MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity=2f, sprintspeed, walkspeed, jumpForce, smoothTime;
    [SerializeField] Item[] items;
    [SerializeField] GameObject cam;
    [SerializeField] TMP_Text Text;
    public bool InZone=true;
  
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
    public enum PlayerState {Normal,Combat,ShipPassenger,ShipCaptain}
    public  PlayerState state;
    public bool m_Jump;





    [SerializeField] float m_MovingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;
    [SerializeField] float m_JumpPower = 12f;
    [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
    [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    [SerializeField] float m_MoveSpeedMultiplier = 1f;
    [SerializeField] float m_AnimSpeedMultiplier = 1f;
    [SerializeField] float m_GroundCheckDistance = 0.5f;
    [SerializeField] Transform RayOrg;
    Rigidbody m_Rigidbody;
    Animator m_Animator;
    bool m_IsGrounded;
    float m_OrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float m_TurnAmount;
    float m_ForwardAmount;
    Vector3 m_GroundNormal;
    float m_CapsuleHeight;
    Vector3 m_CapsuleCenter;
    CapsuleCollider m_Capsule;
    bool m_Crouching;
   // public float mouseSensitivity = 2f;
    float m_moveX;
    float m_moveY;
    //public bool COMBATMODE = false;
    float e_TurnX;
    float e_TurnY;
    // CharacterController controller;//s

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        cameraShake = GetComponentInChildren<CameraShaker>();
        // cam = GetComponentInChildren<GameObject>();
        scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<Scoreboard>();
        if (PV.IsMine)
        {
        state=PlayerState.Normal;
        }
    }
    private void Start()
    {  
        if (PV.IsMine)
        {
            Text.text = PhotonNetwork.NickName;
            EquipItem(0);
           // m_Character = GetComponent<ThirdPersonCharacter1>();
            //  controller = GetComponent<CharacterController>();
            m_Animator = this.GetComponent<Animator>();
            m_Rigidbody = this.GetComponent<Rigidbody>();
            m_Capsule = this.GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;
            //characterController = GetComponent<CharacterController>();
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }
        // Ship = ShipColl.GetComponent<boatmovement>();
    }
    public void Aim()
    {
        e_TurnX+= Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        e_TurnX = Mathf.Clamp(e_TurnX, -90f, 90f);//
        
        e_TurnY += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        e_TurnY = Mathf.Clamp(e_TurnY, -90f, 90f);//
        m_Animator.SetFloat("TurnX", e_TurnX);
        m_Animator.SetFloat("TurnY", e_TurnY);
    }

    public void Move(Vector3 move, bool crouch, bool jump)
    {
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        //move = transform.InverseTransformDirection(move);

        //move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        //m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_TurnAmount = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        m_ForwardAmount = move.z;
        m_moveX = move.x;
        // m_moveY = move.z;
        ApplyExtraTurnRotation();
        CheckGroundStatus();

        // control and velocity handling is different when grounded and airborne:
        if (m_IsGrounded)
        {
            HandleGroundedMovement(crouch, jump);
        }
        else
        {
            HandleAirborneMovement();
        }

        //	ScaleCapsuleForCrouching(crouch);
        //	PreventStandingInLowHeadroom();

        // send input and other state parameters to the animator
      //  if (!COMBATMODE)
        if(state==PlayerState.Normal)
        {
            m_Animator.SetBool("COMBATMODE", false);
            UpdateAnimator(move);
        }
        if (state==PlayerState.Combat)
        {
            m_Animator.SetBool("COMBATMODE", true);
            m_Animator.SetFloat("NoTurn", m_moveX);
            m_Animator.SetFloat("Back", m_ForwardAmount);
        }
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
     //   float h = Input.GetAxis("Horizontal");
       // float v = Input.GetAxis("Vertical");
      
        
        //CHANGING STATES TO ADVENTURE AND COMBAT
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //if (COMBATMODE == true)
            //{ COMBATMODE = false; }
            if (state == PlayerState.Normal)
            {
                state = PlayerState.Combat;
            }   
            else if(state==PlayerState.Combat)
               {
                state = PlayerState.Normal;
                   //COMBATMODE = true;
               }
        }
       
        switch (state)
        {
            default:
            case PlayerState.Normal:
                Look();
                EquipItem(0);
                Jump();
                if (m_Animator.GetLayerWeight(1) == 1f)
                {  m_Animator.SetLayerWeight(1,0f); }

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

                  case PlayerState.Combat:
                Look();
                Jump();
                if (m_Animator.GetLayerWeight(1) == 0)
                {
                 m_Animator.SetLayerWeight(1, 1f);
                }
                Aim();
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
               // Move();
              //  Jump();
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
    void Look()
    {

       // transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;

    }

    void Jump()
    {
        if (!m_Jump)
        {
            m_Jump = Input.GetKeyDown(KeyCode.Space);
          //  Debug.Log(m_Jump);
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
    bool crouch = false;
  
    
    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            if (state == PlayerState.Combat || state==PlayerState.Normal)
            {
                Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
                moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintspeed : walkspeed), ref smoothMoveVelocity, smoothTime);

                crouch = Input.GetKey(KeyCode.C);
                //  rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
                Move(moveAmount, crouch, m_Jump);
                m_Jump = false;
            }
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
        healthbarImage.fillAmount = currentHealth / maxHealth;//s
      //  if (InZone)
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
    void UpdateAnimator(Vector3 move)
    {
        // update the animator parameters
        m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
        m_Animator.SetBool("Crouch", m_Crouching);
        m_Animator.SetBool("OnGround", m_IsGrounded);
        if (!m_IsGrounded)
        {
            m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
        }

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
        if (m_IsGrounded)
        {
            m_Animator.SetFloat("JumpLeg", jumpLeg);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_IsGrounded && move.magnitude > 0)
        {
            m_Animator.speed = m_AnimSpeedMultiplier;
        }
        else
        {
            // don't use that while airborne
            m_Animator.speed = 1;
        }
    }


    void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        m_Rigidbody.AddForce(extraGravityForce);

        m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
    }


    void HandleGroundedMovement(bool crouch, bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // jump!
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
            m_IsGrounded = false;
            m_Animator.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }


    public void OnAnimatorMove()
    {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (m_IsGrounded && Time.deltaTime > 0)
        {
            Vector3 v = m_Animator.deltaPosition * m_MoveSpeedMultiplier / Time.deltaTime;
            // we preserve the existing y part of the current velocity.
            v.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = v;
        }
    }


    void CheckGroundStatus()
    {
        RaycastHit hitInfo;

        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character

        if (Physics.Raycast(RayOrg.position, Vector3.down, out hitInfo, m_GroundCheckDistance))
        //if(characterController.isGrounded)
        {
            //m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            m_Animator.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            //m_GroundNormal = Vector3.up;
            m_Animator.applyRootMotion = false;
        }
    }
}
