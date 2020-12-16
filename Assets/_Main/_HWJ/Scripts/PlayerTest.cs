using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PlayerTest : MonoBehaviour
{

    #region Values


    [Header("Stats")]
    [Tooltip("이동속도")] public float MoveSpeed;
    [Tooltip("점프력")] public float JumpForce;
    [Tooltip("덮어씌울 중력배율")] public float GravityScale;

    [Tooltip("벽 미끄러지는 속도")] public float WallSlideSpeed = 5;
    [Tooltip("삼각점프 보간")] public float WallJumpLerp = 10;
    [Tooltip("대시 스피드")] public float DashSpeed = 20;
    [Tooltip("로프 이동 속도")] public float RopeUpForce;

    public Transform handsPos = null;

    [Space]
    [Header("Booleans")]
    [Tooltip("캐릭터가 기본 왼쪽을 보고 있다면 True")]
    public bool isDefaultLookLeft = true;
    public bool WallJumped;

    public int PlayerSide = 1;
    [Space]
    public GameObject item;

    private bool isGroundTouch;
    private bool hasDashed;

    private bool canMove;
    private bool walk;
    private bool dash;
    private bool get; // Z 누른 경우
    private bool use; // x 누른 경우
    private bool isWall;

    float x;
    float y;
    float xRaw;
    float yRaw;

    private string currentAnimation;
    private AnimState _AnimState;
    private enum AnimState
    {
        idle, walk, run, slowWalk, jump, get, overWall, clime
    }
    #endregion
    #region Components
    private PlayerCollision coll;
    private Rigidbody2D rb { get; set; }
    //private Animator anim { get; set; }
    [SerializeField] private SkeletonAnimation skeletonAnimation = null;
    [SerializeField] private AnimationReferenceAsset[] AnimClip = null;
    [SerializeField] private GameManager gameManager;

    #endregion

    #region WalkSound

    [Header("발소리 쿨다운")] public float WalkSoundCoolDown = 0.2f;
    private float WalkSoundCoolDownNow = 0f;
    [Header("발소리 밸류")] public int WalkSoundValue = 2;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody2D>();

        canMove = true;

        _AnimState = AnimState.idle;
        SetCurrentAnimation(_AnimState);

   
    }

    #region InputManager
    private void InputManager()
    {
        /*
        if (GameManager.Instance.NowState == EnumGameState.Ready)
            return;
        */
        walk = Input.GetKey(KeyCode.LeftControl);
        dash = Input.GetKey(KeyCode.LeftShift);
        get = Input.GetKeyDown(KeyCode.Z);
        use = Input.GetKeyDown(KeyCode.X);

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        WalkSoundCoolDownCheck();
        InputManager();

        Vector2 dir = new Vector2(x, y);
        if(!coll.OnLope)
            Walk(dir);


        if (coll.OnGround || coll.OnLope)
            Jump();

        if (coll.OnGround)
            GetItem();

        UseItem();

        if (coll.OnLope && !coll.OnGround)
            RopeAction();
        else
            rb.gravityScale = GravityScale;
    }

    #region Walk
    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (walk)
        {
            MoveSpeed = 0.5f;
            _AnimState = AnimState.slowWalk;
        }

        else if (dash)
        {
            MoveSpeed = 5f;
            _AnimState = AnimState.run;
        }
        else
        {
            MoveSpeed = 3f;
            _AnimState = AnimState.walk;
        }

        rb.velocity = new Vector2(dir.x * MoveSpeed, rb.velocity.y);
        WalkSound();

        if(rb.velocity.x == 0)
        {
            _AnimState = AnimState.idle;
        }

        FlipAnim();
        SetCurrentAnimation(_AnimState);
    }

    #endregion

    #region Jump
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DisableMovement(0.5f));
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += Vector2.up * JumpForce;
            
            _AnimState = AnimState.jump;
            SetCurrentAnimation(_AnimState);
            //StartCoroutine(DisableMovement(1f));

        }
    }
    #endregion

    
    private void GetItem()
    {
        if (get && coll.OnItem)
        {
            Debug.Log("asdqwe");
            StartCoroutine(DisableMovement(1f));
            _AnimState = AnimState.get;
            SetCurrentAnimation(_AnimState);
        }

        if(handsPos.GetChildCount() !=0)
        {
            item = handsPos.GetChild(0).gameObject;
            if (item.GetComponent<Item>().itemType.ToString() == "UnCarriable")
            {
                item.GetComponent<Item>().UseItem();
            }


            //handsPos.GetChild(0).gameObject.SetActive(false);
        }
    }
    private void UseItem()
    {

        if (item != null)
        {
        
            if (get)
            {
                item.GetComponent<Item>().UseItem();
                item = null;
                //item.GetComponent<Item>().rb.velocity = handsPos.transform.right * 5f;
                //item.SetActive(false);
            }
            else if (item.GetComponent<Item>().itemType.ToString() == "Carriable")
            {
                if (walk)
                {
                    item.GetComponent<Item>().UseItem();
                    item.transform.position += new Vector3(0, 1, 0);

                }
            }

        }
        else return;

    }
    public bool GetItemStatus()
    {
        return get;
    }

    #region RopeAction
    private void RopeAction()
    {

        Debug.Log("rope");
        rb.AddForce(Vector2.up * yRaw * 0.1f, ForceMode2D.Impulse);
        Debug.Log(yRaw);
        rb.gravityScale = 0f;

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * RopeUpForce);

        _AnimState = AnimState.clime;
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
    }

    private void WalkSound()
    {
        #region WalkSound
        if (WalkSoundCoolDownNow <= 0)
        {
            EffectManager.Instance.SetPool("SoundWave", transform.position, new Vector3(0.5f, 0.5f, 1f));
            AudioManager.Instance.PlaySound("Footstep", WalkSoundValue, transform.position);

            WalkSoundCoolDownNow = WalkSoundCoolDown;
        }
        #endregion
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
                AsncAnimation(AnimClip[(int)AnimState.idle], true, 0.5f);
                break;

            case AnimState.walk:
                AsncAnimation(AnimClip[(int)AnimState.walk], true, 0.5f);
                break;

            case AnimState.run:
                AsncAnimation(AnimClip[(int)AnimState.run], true, 0.5f);
                break;

            case AnimState.slowWalk:
                AsncAnimation(AnimClip[(int)AnimState.slowWalk], true, 0.5f);
                break;

            case AnimState.jump:
                AsncAnimation(AnimClip[(int)AnimState.jump], false, 0.5f);
                break;

            case AnimState.get:
                AsncAnimation(AnimClip[(int)AnimState.get], false, 0.5f);
                break;

            case AnimState.overWall:
                AsncAnimation(AnimClip[(int)AnimState.overWall], false, 0.5f);
                break;

            case AnimState.clime:
                AsncAnimation(AnimClip[(int)AnimState.clime], true, 0.5f);
                break;
        }


    }
    #endregion

}
