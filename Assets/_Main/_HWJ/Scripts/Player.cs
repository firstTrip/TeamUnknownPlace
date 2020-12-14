using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Player : MonoBehaviour
{

    private Rigidbody2D rb;

    private bool canMove;
    private bool walk;
    private bool dash;
    private bool get; // Z 누른 경우
    private bool use; // x 누른 경우
    private bool isWall;


    private PlayerCollision coll;
    [SerializeField] private SkeletonAnimation skeletonAnimation = null;
    [SerializeField] private AnimationReferenceAsset[] AnimClip = null;

    [SerializeField]private float JumpForce;
    [SerializeField] private float OverWallJumpForce;
    [SerializeField] private float moveSpeed;

    [SerializeField] private Transform handsPos = null;
    private float moveForce;
    private float h;

    private string currentAnimation;

    private AnimState _AnimState;
    private enum AnimState
    {
        idle , walk , run , slowWalk , jump , get , overWall
    }


    #region WalkSound

    [Header("발소리 쿨다운")]public float WalkSoundCoolDown = 0.2f;
    private float WalkSoundCoolDownNow = 0f;
    [Header("발소리 밸류")]public int WalkSoundValue = 2;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        WalkSoundCoolDownCheck();
        InputManager();
        Move();


        GetItem();
        UseItem();

        if (Input.GetButtonUp("Horizontal"))
        {
            rb.velocity = new Vector2(rb.velocity.normalized.x * 0, rb.velocity.y);
        }

        if (coll.OnGround)
        {
            isWall = true;
            Jump();

        }

        if (!coll.OnGround && coll.OnWall)
             SeizeWall();

        if (!isWall && Input.GetButtonUp("Horizontal"))
        {
            OverWall();
        }

    }

    private void init()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision>();
        canMove = true;
        isWall = true;
        moveSpeed = 1f;
        moveForce = 1.5f;
        JumpForce = 8f;
    }

    private void InputManager()
    {
        walk = Input.GetKey(KeyCode.LeftControl);
        dash = Input.GetKey(KeyCode.LeftShift);
        get = Input.GetKeyDown(KeyCode.Z);
        use = Input.GetKeyDown(KeyCode.X);
        h = Input.GetAxisRaw("Horizontal");
    }

    private void Move()
    {

        if (!canMove)
            return;

 
        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);


        if (walk)
        {
            moveSpeed = 0.5f;
            _AnimState = AnimState.slowWalk;
        }
           
        else if (dash)
        {
            moveSpeed = 3f;
            _AnimState = AnimState.run;
        }
        else
        {
            moveSpeed = 1f;
            _AnimState = AnimState.walk;
        }

        if (rb.velocity.x > moveForce)
        {
            rb.velocity = new Vector2(moveForce * moveSpeed , rb.velocity.y);
            transform.localScale = new Vector2(-h * 1f, 1f);
            WalkSound();
        }
        else if (rb.velocity.x < moveForce * (-1))
        {
            rb.velocity = new Vector2(moveForce * (-1) * moveSpeed , rb.velocity.y);
            transform.localScale = new Vector2(-h * 1f, 1f);
            WalkSound();
        }

        SetCurrentAnimation(_AnimState);

    }

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

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

            _AnimState = AnimState.jump;
            SetCurrentAnimation(_AnimState);
            //StartCoroutine(DisableMovement(1f));

        }
    }

    private void SeizeWall()
    {

        Debug.Log("asd");
        StartCoroutine(DisableMovement(0.5f));
        _AnimState = AnimState.overWall;
        SetCurrentAnimation(_AnimState);
        isWall = false;

        rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
        //StartCoroutine(FreezeRotaison(0.5f));
        

    }

    private void OverWall()
    {
        if(!coll.IsRightWall || !coll.IsLeftWall)
        {
            isWall = true;

            transform.Translate(new Vector2(1, 1.5f));
          
        }

    }

    private void GetItem()
    {
        if (get)
        {
            Debug.Log("getItem");

            StartCoroutine(DisableMovement(1f));
            _AnimState = AnimState.get;
            SetCurrentAnimation(_AnimState);

        }
    }

    private void UseItem()
    {

        if (handsPos.GetChildCount() ==1)
        {
            if (use)
            {
                Destroy(handsPos.GetChild(0).gameObject);
            }

            if (get)
            {
                Destroy(handsPos.GetChild(0).gameObject);
            }
        }
        else return;
       
    }

    public bool GetItemGet()
    {
        return get;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator FreezeRotaison(float time)
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(time);
        rb.constraints = RigidbodyConstraints2D.None;
    }

    IEnumerator AnimeDuraiton()
    {
        
        yield return new WaitForSeconds(skeletonAnimation.skeleton.Data.FindAnimation(currentAnimation).Duration);
        Debug.Log(skeletonAnimation.skeleton.Data.FindAnimation(currentAnimation).Duration);
        
    }

    #region AsncAnimation
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
                AsncAnimation(AnimClip[(int)AnimState.get], false, 1f);
                break;

            case AnimState.overWall:
                AsncAnimation(AnimClip[(int)AnimState.overWall], false, 1f);
                break;

        }


    }
    #endregion



}
