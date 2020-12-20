using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Player : MonoBehaviour, IDamagable, ISaveLoad
{

    #region Values

    [Header("Stats")]
    [Tooltip("이동속도")] public float MoveSpeed;
    [Tooltip("점프력")] public float JumpForce;
    [Tooltip("덮어씌울 중력배율")] public float GravityScale;
    [Tooltip("로프 이동 속도")] public float RopeUpForce;

    #region HP Property & Dead()
    [Header("최대체력")]public float MaxHP = 10f;
    [SerializeField][Header("현재체력")]
    private float hp;
    public float HP
    {
        get
        {
            return hp;
        }
        set
        {
            if(value > MaxHP)
            {
                value = MaxHP;
            }

            if(value <= 0)
            {
                hp = 0;
                Dead();

                return;
            }

            hp = value;            
        }
    }

    private bool isAlive = true;

    public void Dead()
    {
        if (!isAlive)
        {
            return;
        }

        Debug.LogWarning("Player Dead");

        CallState(PlayerState.Nomal);
        anim.SetTrigger("Die");

        DeadEffect(4.3f);
    }

    public void DeadByWater()
    {
        Debug.LogWarning("Player Dead By Water");

        CallState(PlayerState.Water);
        DeadEffect(3.0f);
    }

    public void DeadEffect(float time)
    {
        // 추적 중지
        LightManager.Instance.DeadCheck(gameObject);

        // 상태처리, 입력 제거
        isAlive = false;

        StartCoroutine(DUSDJUtil.ActionAfterSecondCoroutine(time, () =>{
            AfterDead();
        }));
    }

    public void AfterDead()
    {
        GameManager.Instance.GameOver(true);
    }

    public void InitHP()
    {
        hp = MaxHP;
    }

    public void Heal(float value)
    {
        HP += value;
    }

    #endregion

    [Space]

    public Transform handsPos = null;
  
    [Space]
    public ThrowItem ItemInHand;
    public Animator anim;
    [Space]

    public bool canMove;
    private bool sit;
    private bool dash;
    private bool use;
    public bool get; // Z 누른 경우
    private bool useStair;

    public bool isInvincibility; //  true 일시 대미지 x 


    private float x;
    private float y;
    private float yRaw;
    public float xRaw;

    private int currentState;
    private string currentAnimation;
    private AnimState _AnimState;
    private enum AnimState
    {
        idle, walk, run, slowWalk, jump, get, Throw, clime ,stairUP ,wakeUp ,NomalDead ,WaterDead , Down ,Roll
    }

    public enum PlayerState
    {
        Nomal , Water ,Allive
    }

    public PlayerState playerState;

    #endregion

    #region Components
    private PlayerCollision coll;
    public Rigidbody2D rb { get; set; }

    [SerializeField] private SkeletonAnimation skeletonAnimation = null;
    [SerializeField] private AnimationReferenceAsset[] AnimClip = null;

    private Vector2 dir;

    #endregion

    #region Crouch, Walk, Run Sound
    [Header("앉은걸음 쿨다운")] public float CrouchSoundCoolDown = 0.4f;
    private float CrouchSoundCoolDownNow = 0f;
    [Header("앉은걸음 밸류")] public int CrouchSoundValue = 10;

    [Header("발걸음 쿨다운")] public float WalkSoundCoolDown = 0.4f;
    private float WalkSoundCoolDownNow = 0f;
    [Header("발걸음 밸류")] public int WalkSoundValue = 20;

    [Header("뜀걸음 쿨다운")] public float RunSoundCoolDown = 0.2f;
    private float RunSoundCoolDownNow = 0f;
    [Header("뜀걸음 밸류")] public int RunSoundValue = 40;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //ISaveLoad
        ISaveLoadInit();
        // Hp
        InitHP();


        coll = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        _AnimState = AnimState.NomalDead;
        SetCurrentAnimation(_AnimState);
        canMove = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAnimation == "Hero_die01" || currentAnimation == "Hero_drown01")
        {
            rb.velocity = Vector2.zero;
            canMove = false;
            return;
        }


        WalkSoundCoolDownCheck();
        InputManager();

        dir = new Vector2(x, y);


        if (!canMove)
            return;

        if (!coll.OnRope)
            Walk(dir);

        if (coll.OnGround || coll.OnRope)
            Jump(dir);
       

        #region 로프 사용시 중력값 조정
        if (coll.OnRope)
            RopeAction();

        else
            rb.gravityScale = GravityScale;
        #endregion

        bool getItem = false;

        if (coll.OnGround)
        {
            getItem = GetItem();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StairUp();
        }
        else
            useStair = false;

        if (!getItem)
        {
            UseItem();
        }               
    }

    #region InputManager
    public void ForceCanMove(bool value)
    {
        canMove = value;
    }

    private void InputManager()
    {

        if (GameManager.Instance.NowState == EnumGameState.Ready)
        {
            x = 0;
            y = 0;
            canMove = false;
            rb.velocity = Vector2.zero;
            return;
        }

        sit = Input.GetKey(KeyCode.LeftControl);
        dash = Input.GetKey(KeyCode.LeftShift);
        get = Input.GetKeyDown(KeyCode.Z);
        use = Input.GetKeyDown(KeyCode.X); // 숨기

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");

    }
    #endregion

    #region Walk
    private void Walk(Vector2 dir)
    {
        if (sit)
        {
            MoveSpeed = 0.0f;
            if (isInvincibility)
            {
                MoveSpeed = 0.5f;
                rb.velocity = new Vector2(dir.x * MoveSpeed, rb.velocity.y);
                _AnimState = AnimState.slowWalk;

                FlipAnim();
                SetCurrentAnimation(_AnimState);
            }
            _AnimState = AnimState.slowWalk;
        }
        else if (dash)
        {
            MoveSpeed = 5f;
            _AnimState = AnimState.run;
            MovementSound(EnumMovement.Run);
        }
        else
        {
            MoveSpeed = 3f;
            _AnimState = AnimState.walk;
        }

        rb.velocity = new Vector2(dir.x * MoveSpeed, rb.velocity.y);

        if (rb.velocity.x == 0 && !sit)
        {
            if (!canMove)
            return;
            _AnimState = AnimState.idle;
            FlipAnim();
            SetCurrentAnimation(_AnimState);
            return;
        }


        if(!dash &&!sit)
        MovementSound(EnumMovement.Walk);

        FlipAnim();
        SetCurrentAnimation(_AnimState);
    }

    #endregion

    #region Jump
    private void Jump(Vector2 dir)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartDisable(0.25f);

            if (coll.OnRope)
            {
                rb.velocity = new Vector2(JumpForce * 0.5f, JumpForce * 0.2f);
            }
            rb.velocity = new Vector2(rb.velocity.x, 0);

            if(rb.velocity == Vector2.zero || dir.y == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, JumpForce);
                _AnimState = AnimState.jump;
                SetCurrentAnimation(_AnimState);
            }

        }
     
    }
    #endregion

    private void StairUp()
    {
        useStair = true;
        _AnimState = AnimState.stairUP;
        SetCurrentAnimation(_AnimState);
    }

    #region Disable Coroutine

    private IEnumerator disableCoroutine;
    private void StartDisable(float time)
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = DisableMovement(time);
        StartCoroutine(disableCoroutine);
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSecondsRealtime(time);
        canMove = true;
    }

    #endregion



    private bool GetItem()
    {
        // Z 키 입력
        if (get && coll.OnItem)
        {
            // 손에 든 ThrowItem 없고
            if (ItemInHand != null)
            {
                return false;
            }

            // 바닥의 ThrowItem이 범위 내에 있다.
            // = GetItem
            ThrowItem checkItem = coll.ItemCollider.GetComponent<ThrowItem>();
            if (checkItem != null)
            {
                checkItem.GetItemByPlayer(this, handsPos);

                _AnimState = AnimState.get;
                SetCurrentAnimation(_AnimState);
                StartDisable(0.5f);

                return true;
            }
        }

        return false;
    }
    private void UseItem()
    {
        // 손에 ThrowItem이 있다.
        if (ItemInHand != null)
        {
            // Z 키 입력
            if (get)
            {
                ItemInHand.UseItem();
                ItemInHand = null;

                StartDisable(0.5f);
                _AnimState = AnimState.Throw;
                SetCurrentAnimation(_AnimState);                
            }
        }

        // X 키 입력
        if (use)
        {
            /* Hide */

            // 사용중인 HI가 있음
            if (myHideItem != null)
            {
                HideExit();
                return;
            }

            // 새 HI 찾기
            if (coll.HideItemCollider != null)
            {
                HideItem hi = coll.HideItemCollider.GetComponent<HideItem>();
                HideEnter(hi);
            }

        }
    }

    #region Hide By HideItem

    private HideItem myHideItem;

    public void HideEnter(HideItem hi)
    {
        if (hi.isUse)
        {
            return;
        }

        myHideItem = hi;
        myHideItem.isUse = true;
        myHideItem.spr.sortingLayerName = "Middleground_AP";

        isInvincibility = true;

        LightManager.Instance.DeadCheck(gameObject);
    }

    public void HideExit()
    {
        if(myHideItem == null)
        {
            return;
        }

        myHideItem.isUse = false;
        myHideItem.spr.sortingLayerName = "Middleground_BP";

        isInvincibility = false;

        myHideItem = null;
    }

    #endregion




    public void RemoveSprite()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public bool GetItemStatus()
    {
        return get;
    }

    public bool GetUseStair()
    {
        return useStair;
    }

    // 숨기는 x 
    public bool GetItemUse()
    {
        return use;
    }

    public void CallWakeUp()
    {
        StartDisable(8.5f);
        _AnimState = AnimState.wakeUp;
        SetCurrentAnimation(_AnimState);

    }

    public void CallDown()
    {
        _AnimState = AnimState.Down;
        SetCurrentAnimation(_AnimState);
    }

    public void RollAnimPlayer()
    {
        StartDisable(8.5f);
        _AnimState = AnimState.Roll;
        SetCurrentAnimation(_AnimState);

    }

    public void CallState(PlayerState deadState)
    {
        switch (deadState)
        {
            case PlayerState.Nomal:

                rb.velocity = Vector2.zero;
                x = 0;
                y = 0;
                canMove = false;
                _AnimState = AnimState.NomalDead;
                SetCurrentAnimation(_AnimState);

                break;

            case PlayerState.Water:

                rb.velocity = Vector2.zero;
                x = 0;
                y = 0;
                canMove = false;
                _AnimState = AnimState.WaterDead;
                SetCurrentAnimation(_AnimState);

                break;

            case PlayerState.Allive:
                canMove = true;
                _AnimState = AnimState.idle;
                SetCurrentAnimation(_AnimState);

                // 메시 복구
                GetComponentInChildren<RemoveSprite>().MakeSprite();
                break;
        }
    }

    #region RopeAction
    private void RopeAction()
    {

        rb.AddForce(Vector2.up * yRaw * 0.25f, ForceMode2D.Impulse);

        rb.gravityScale = 0f;
        //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * RopeUpForce);
        _AnimState = AnimState.clime;

        if(yRaw == 0)
        {
            Debug.Log("??");
            _AnimState = AnimState.idle;
        }
        SetCurrentAnimation(_AnimState);
        FlipAnim();

    }
    #endregion

    

    #region 사운드
    private void WalkSoundCoolDownCheck()
    {
        if (WalkSoundCoolDownNow > 0)
        {
            WalkSoundCoolDownNow -= Time.deltaTime;
        }

        if (CrouchSoundCoolDownNow > 0)
        {
            CrouchSoundCoolDownNow -= Time.deltaTime;
        }

        if (RunSoundCoolDownNow > 0)
        {
            RunSoundCoolDownNow -= Time.deltaTime;
        }
    }

    private void MovementSound(EnumMovement m)
    {
        // 숨어있는 상태면 소리나지 않는다.
        if(myHideItem != null)
        {
            return;
        }

        switch (m)
        {
            case EnumMovement.Crouch:
                if (CrouchSoundCoolDownNow <= 0)
                {
                    AudioManager.Instance.PlaySound("Footstep", CrouchSoundValue, transform.position, gameObject);
                    CrouchSoundCoolDownNow = CrouchSoundCoolDown;
                }

                break;


            case EnumMovement.Walk:
                if (WalkSoundCoolDownNow <= 0)
                {
                    AudioManager.Instance.PlaySound("Footstep", WalkSoundValue, transform.position, gameObject);
                    WalkSoundCoolDownNow = WalkSoundCoolDown;
                }

                break;


            case EnumMovement.Run:
                if (RunSoundCoolDownNow <= 0)
                {
                    AudioManager.Instance.PlaySound("Footstep", RunSoundValue, transform.position, gameObject);
                    RunSoundCoolDownNow = RunSoundCoolDown;
                }

                break;
        }
        
    }


    #endregion

    #region FlipAnim
    private void FlipAnim()
    {
        if (xRaw != 0)
            transform.localScale = new Vector2(-xRaw * 1f, 1f);
    }
    #endregion

    #region 

    private void AsncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
    {

        if (animClip.name.Equals(currentAnimation))
            return;

        //해당 애님으로 변경
        skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
        currentAnimation = animClip.name;

    }
    #endregion

    #region SetCurrentAnimation
    private void SetCurrentAnimation(AnimState _state)
    {

        switch (_state)
        {

            case AnimState.idle:
                AsncAnimation(AnimClip[(int)AnimState.idle], true, 1f);
                break;

            case AnimState.walk:
                AsncAnimation(AnimClip[(int)AnimState.walk], true, 0.5f);
                break;

            case AnimState.run:
                AsncAnimation(AnimClip[(int)AnimState.run], true, 0.5f);
                break;

            case AnimState.slowWalk:
                AsncAnimation(AnimClip[(int)AnimState.slowWalk], false, 2f);
                break;

            case AnimState.jump:
                AsncAnimation(AnimClip[(int)AnimState.jump], false, 1.5f);
                break;

            case AnimState.get:
                AsncAnimation(AnimClip[(int)AnimState.get], false, 3f);
                break;

            case AnimState.Throw:
                AsncAnimation(AnimClip[(int)AnimState.Throw], false, 2.5f);
                break;

            case AnimState.clime:
                AsncAnimation(AnimClip[(int)AnimState.clime], true, 1f);
                break;

            case AnimState.stairUP:
                AsncAnimation(AnimClip[(int)AnimState.stairUP], true, 1f);
                break;

            case AnimState.wakeUp:
                AsncAnimation(AnimClip[(int)AnimState.wakeUp], false, 1f);
                break;

            case AnimState.NomalDead:
                AsncAnimation(AnimClip[(int)AnimState.NomalDead], false, 1f);
                break;

            case AnimState.WaterDead:
                AsncAnimation(AnimClip[(int)AnimState.WaterDead], false, 1f);
                break;
            case AnimState.Down:
                AsncAnimation(AnimClip[(int)AnimState.Down], false, 1f);
                break;

            case AnimState.Roll:
                AsncAnimation(AnimClip[(int)AnimState.Roll], false, 1f);
                break;
        }

    }

    #endregion

    #region IDamagable
    public void Damage(float value)
    {
        if (isInvincibility)
        {
            return;
        }

        HP -= value;
        return;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
    #endregion

    #region ISaveLoad

    public struct StructSaveData {
        public Vector3 SavePosition;
        public float HP;
        public bool isAlive;
    }    
    public StructSaveData SaveData;

    public void ISaveLoadInit()
    {
        SaveManager.Instance.AddSaveObject(this);
    }

    public void ISave()
    {
        Debug.Log(string.Format("ISave : {0}", gameObject.name));
        SaveData.SavePosition = transform.position;
        SaveData.HP = hp;
        SaveData.isAlive = isAlive;
    }

    public void ILoad()
    {
        Debug.Log(string.Format("ILoad : {0}", gameObject.name));
        transform.position = SaveData.SavePosition;        
        isAlive = SaveData.isAlive;

        if (isAlive)
        {
            HP = SaveData.HP;
        }

        CallState(PlayerState.Allive);
    }

    public void ISaveDelete()
    {
        SaveManager.Instance.DeleteSaveObject(this);
    }

    #endregion

}
