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
    [Space]

    private bool canMove;
    private bool sit;
    private bool dash;
    private bool get; // Z 누른 경우
    private bool use;
    private bool isWall;
    public bool isInvincibility; //  true 일시 대미지 x 


    float x;
    float y;
    float xRaw;
    float yRaw;

    private string currentAnimation;
    private AnimState _AnimState;
    private enum AnimState
    {
        idle, walk, run, slowWalk, jump, get, Throw, clime ,stairUP
    }
    #endregion

    #region Components
    private PlayerCollision coll;
    private Rigidbody2D rb { get; set; }
    //private Animator anim { get; set; }
    [SerializeField] private SkeletonAnimation skeletonAnimation = null;
    [SerializeField] private AnimationReferenceAsset[] AnimClip = null;
    

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

   

    // Update is called once per frame
    void Update()
    {
        Debug.Log(coll.OnRightWall);
        WalkSoundCoolDownCheck();
        InputManager();

        Vector2 dir = new Vector2(x, y);

        if (!coll.OnLope)
            Walk(dir);
            
        if (coll.OnGround || coll.OnLope)
            Jump(dir);

        if (coll.OnRightWall)
            StairUp();

        if (coll.OnGround)
            GetItem();


        if (handsPos.transform.childCount !=0)
                item = handsPos.transform.GetChild(0).gameObject;

        UseItem();

        #region 로프 사용시 중력값 조정
        if (coll.OnLope && !coll.OnGround)
            RopeAction();
        else
            rb.gravityScale = GravityScale;
        #endregion
    }

    #region InputManager
    private void InputManager()
    {

        if (GameManager.Instance.NowState == EnumGameState.Ready)
        {
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
        if (!canMove)
            return;

        if (sit)
        {
            MoveSpeed = 0.0f;
            if (item != null && item.GetComponent<Item>().itemType.ToString() == "Carriable")
            {
                MoveSpeed = 0.5f;
                //_AnimState = AnimState.walk;
                
            }
            StartCoroutine(DisableMovement(0.5f));
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

        if (rb.velocity.x == 0 && !sit)
        {
            _AnimState = AnimState.idle;
            FlipAnim();
            SetCurrentAnimation(_AnimState);
            return;
        }

        WalkSound();
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

            rb.velocity = new Vector2(rb.velocity.x, 0);

            if(rb.velocity == Vector2.zero || dir.y == 0)
            {
                rb.velocity += Vector2.up * JumpForce;
                _AnimState = AnimState.jump;
                SetCurrentAnimation(_AnimState);
            }

        }
    }
    #endregion

    private void StairUp()
    {

        _AnimState = AnimState.stairUP;
        SetCurrentAnimation(_AnimState);
    }

    private void GetItem()
    {
        if (get && coll.OnItem || get && coll.OnHideItem)
        {
            Debug.Log("in GetItem");
            if(item != null)
            {
                Debug.Log("HasItem");
                return;
            }

            StartCoroutine(DisableMovement(0.5f));
            _AnimState = AnimState.get;
            SetCurrentAnimation(_AnimState);
            Debug.Log(handsPos.childCount);

        }

    }
    private void UseItem()
    {
        if (item != null)
        {
            if (get)
            {
                if(item.GetComponent<Item>().itemType.ToString() == "ThrowItem")
                     item.GetComponent<Item>().UseItem();

                _AnimState = AnimState.Throw;
                SetCurrentAnimation(_AnimState);
                handsPos.GetChild(0).gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);
                item = null;
                Debug.Log(item);
            }

            if (use)
            {
                if (item.GetComponent<Item>().itemType.ToString() == "Carriable")
                {
                    item.GetComponent<Item>().UseItem();
                    item = null;

                }

                // handsPos.GetChild(0).gameObject.transform.SetParent(GameObject.Find("Middleground_AP").transform);

            }
        }
        else return;

    }

    public bool GetItemStatus()
    {
        return get;
    }

    // 숨기는 x 
    public bool GetItemUse()
    {
        return use;
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

    #region WalkSound
    private void WalkSound()
    {
        
        if (WalkSoundCoolDownNow <= 0)
        {
            AudioManager.Instance.PlaySound("Footstep", WalkSoundValue, transform.position);

            WalkSoundCoolDownNow = WalkSoundCoolDown;
        }
        
    }
    #endregion

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
                AsncAnimation(AnimClip[(int)AnimState.Throw], false, 1.5f);
                break;

            case AnimState.clime:
                AsncAnimation(AnimClip[(int)AnimState.clime], true, 1f);
                break;

            case AnimState.stairUP:
                AsncAnimation(AnimClip[(int)AnimState.stairUP], true, 1f);
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
