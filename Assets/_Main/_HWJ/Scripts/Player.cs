using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Player : MonoBehaviour, IDamagable
{

    #region Values

    [Header("Stats")]
    [Tooltip("이동속도")] public float MoveSpeed;
    [Tooltip("점프력")] public float JumpForce;
    [Tooltip("덮어씌울 중력배율")] public float GravityScale;
    [Tooltip("로프 이동 속도")] public float RopeUpForce;

    [Space]

    public Transform handsPos = null;
  
    [Space]
    public GameObject item;
    public Animator anim;
    [Space]

    private bool canMove;
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

    private string currentAnimation;
    private AnimState _AnimState;
    private enum AnimState
    {
        idle, walk, run, slowWalk, jump, get, Throw, clime ,stairUP ,wakeUp ,NomalDead ,WaterDead , Down
    }

    public enum PlayerState
    {
        Nomal , Water ,Allive
    }

    public PlayerState playerState;

    #endregion

    #region Components
    private PlayerCollision coll;
    private Rigidbody2D rb { get; set; }

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
        coll = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody2D>();

        _AnimState = AnimState.idle;
        SetCurrentAnimation(_AnimState);
        canMove = true;

    }

    // Update is called once per frame
    void Update()
    {

        WalkSoundCoolDownCheck();
        InputManager();

        dir = new Vector2(x, y);



        if (!coll.OnRope)
            Walk(dir);

        if (coll.OnGround || coll.OnRope)
            Jump(dir);

        if (!canMove)
            return;

        #region 로프 사용시 중력값 조정
        if (coll.OnRope)
            RopeAction();

        else
            rb.gravityScale = GravityScale;
        #endregion

        if (coll.OnGround)
        {
            GetItem();
        }

        if (coll.OnRightWall && yRaw !=0 )
        {
            StairUp();
        }
        else
            useStair = false;

        UseItem();
    }
    private void StartEffect()
    {
        anim = GetComponent<Animator>();

    }

    #region InputManager
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
        else
            canMove = true;


        sit = Input.GetKey(KeyCode.LeftControl);
        dash = Input.GetKey(KeyCode.LeftShift);
        get = Input.GetKeyUp(KeyCode.Z);
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
        if (!canMove)
            return;

        if (sit)
        {
            StartEffect();

            LightManager.Instance.DeadCheck(gameObject);
            MoveSpeed = 0.0f;
            if (isInvincibility)
            {
                MoveSpeed = 0.5f;
                rb.velocity = new Vector2(dir.x * MoveSpeed, rb.velocity.y);
                _AnimState = AnimState.slowWalk;

                FlipAnim();
                SetCurrentAnimation(_AnimState);
            }

            StartCoroutine(DisableMovement(0.5f));
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
            _AnimState = AnimState.idle;
            FlipAnim();
            SetCurrentAnimation(_AnimState);
            return;
        }

       if(!dash &&!sit )
        MovementSound(EnumMovement.Walk);

        /* 아래 3줄을 각각
         * 앉은걸음, 걷기, 달리기 소리가 날 위치에 놔주세요. */
        // MovementSound(EnumMovement.Crouch); // 앉은걸음
        // MovementSound(EnumMovement.Walk); // 걷기
        // MovementSound(EnumMovement.Run); // 달리기
        FlipAnim();
        SetCurrentAnimation(_AnimState);
    }

    #endregion

    #region Jump
    private void Jump(Vector2 dir)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DisableMovement(1f));

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

    private void GetItem()
    {
        if (get && coll.OnItem)
        {
            if(item != null)
            {
                return;
            }

            if (handsPos.transform.childCount != 0)
                item = handsPos.transform.GetChild(0).gameObject;


            _AnimState = AnimState.get;
            SetCurrentAnimation(_AnimState);
            StartCoroutine(DisableMovement(0.5f));

        }


    }
    private void UseItem()
    {
        if (item != null)
        {
            if (get)
            { 
                if (item.GetComponent<Item>().itemType.ToString() == "ThrowItem")
                {
                    item.GetComponent<Item>().UseItem();

                    StartCoroutine(DisableMovement(0.5f));
                    _AnimState = AnimState.Throw;
                    SetCurrentAnimation(_AnimState);
                    item = null;
                }
            }

         
        }

        if (use)
        {
            if (handsPos.transform.childCount != 0)
                item = handsPos.transform.GetChild(0).gameObject;

            if (item != null)
            {
                if (item.GetComponent<Item>().itemType.ToString() == "Carriable")
                {

                    item.GetComponent<Item>().UseItem();
                    item = null;
                }

            }

        }
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
        StartCoroutine(DisableMovement(8.5f));
        _AnimState = AnimState.wakeUp;
        SetCurrentAnimation(_AnimState);

    }

    public void CallDown()
    {
        _AnimState = AnimState.Down;
        SetCurrentAnimation(_AnimState);
    }

    public void CallState(PlayerState deadState )
    {
            
        switch (playerState)
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
            _AnimState = AnimState.idle;
        }
        SetCurrentAnimation(_AnimState);
        FlipAnim();

    }
    #endregion

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

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
                AsncAnimation(AnimClip[(int)AnimState.get], false, 1.5f);
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
                AsncAnimation(AnimClip[(int)AnimState.wakeUp], true, 1f);
                break;

            case AnimState.NomalDead:
                AsncAnimation(AnimClip[(int)AnimState.NomalDead], true, 1f);
                break;

            case AnimState.WaterDead:
                AsncAnimation(AnimClip[(int)AnimState.WaterDead], false, 1f);
                break;
            case AnimState.Down:
                AsncAnimation(AnimClip[(int)AnimState.Down], false, 1f);
                break;
        }

    }

    #endregion

    #region IDamagable
    public void Damage(float value)
    {
        return;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
    #endregion

}
