using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    #region Values


    [Header("Stats")]
    [Tooltip("이동속도")] public float MoveSpeed;
    [Tooltip("점프력")] public float JumpForce;
    [Tooltip("덮어씌울 중력배율")] public float GravityScale;

    [Tooltip("벽 미끄러지는 속도")] public float WallSlideSpeed = 5;
    [Tooltip("삼각점프 보간")] public float WallJumpLerp = 10;
    [Tooltip("대시 스피드")] public float DashSpeed = 20;

    [Space]
    [Header("Booleans")]
    [Tooltip("캐릭터가 기본 왼쪽을 보고 있다면 True")]
    public bool isDefaultLookLeft = true;
    public bool CanMove = true;
    public bool WallGrab;
    public bool WallJumped;
    public bool IsWallSlide;
    public bool IsDashing;

    public int PlayerSide = 1;

    [Space]

    private bool isGroundTouch;
    private bool hasDashed;


    #endregion

    #region Components
    private BetterJump betterJump;
    private PlayerCollision coll;
    private Rigidbody2D rb { get; set; }
    //private Animator anim { get; set; }
    private PlayerAnimation anim;

    #endregion

    #region Init

    private void Awake()
    {
        betterJump = GetComponent<BetterJump>();
        coll = GetComponent<PlayerCollision>();
        anim = GetComponentInChildren<PlayerAnimation>();

        rb = GetComponent<Rigidbody2D>();
        

        Init();
    }

    public void Init()
    {

    }

    #endregion


    private void Update()
    {

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(x, y);

        Walk(dir);
        //anim.SetHorizontalMovement(x, y, rb.velocity.y);

        #region Wall Grab

        if (coll.OnWall && Input.GetKey(KeyCode.LeftShift) && CanMove)
        {
            
            if (PlayerSide != coll.WallSide)
                anim.Flip(PlayerSide);
                
            WallGrab = true;
            IsWallSlide = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || !coll.OnWall || !CanMove)
        {
            WallGrab = false;
            IsWallSlide = false;
        }

        if (coll.OnGround && !IsDashing)
        {
            WallJumped = false;
            betterJump.enabled = true;
        }

        if (WallGrab && !IsDashing)
        {
            rb.gravityScale = 0;
            if (x > .2f || x < -.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (MoveSpeed * speedModifier));
        }
        else
        {
            rb.gravityScale = GravityScale;
        }

        #endregion



        #region Wall

        if (coll.OnWall && !coll.OnGround)
        {
            if (x != 0 && !WallGrab)
            {
                IsWallSlide = true;
                WallSlide();
            }
        }

        if (!coll.OnWall || coll.OnGround)
            IsWallSlide = false;

        #endregion



        #region Jump

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //anim.SetTrigger("jump");

            if (coll.OnGround)
                Jump(Vector2.up, false);

            if (coll.OnWall && !coll.OnGround)
                WallJump();
                
        }

        #endregion

        #region Dash
        if (Input.GetKeyDown(KeyCode.LeftControl) && !hasDashed)
        {
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }
        #endregion


        #region Ground Touch

        if (coll.OnGround && !isGroundTouch)
        {
            GroundTouch();
            isGroundTouch = true;
        }

        if (!coll.OnGround && isGroundTouch)
        {
            isGroundTouch = false;
        }

        #endregion

        // WallParticle(y);

        if (WallGrab || IsWallSlide || !CanMove)
            return;

        if (x > 0)
        {
            PlayerSide = 1;
            anim.Flip(PlayerSide);
        }
        if (x < 0)
        {
            PlayerSide = -1;
            anim.Flip(PlayerSide);
        }

    }


    #region Movements


    private void Walk(Vector2 dir)
    {
        if (!CanMove)
            return;

        if (WallGrab)
            return;

        if (!WallJumped)
        {
            rb.velocity = new Vector2(dir.x * MoveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * MoveSpeed, rb.velocity.y)), WallJumpLerp * Time.deltaTime);
        }
    }

    private void WallSlide()
    {
        
        if (coll.WallSide != PlayerSide)
            anim.Flip(PlayerSide);
        
        if (!CanMove)
            return;

        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.OnRightWall) || (rb.velocity.x < 0 && coll.OnLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -WallSlideSpeed);
    }

    private void WallJump()
    {
        if ((PlayerSide == 1 && coll.OnRightWall) || PlayerSide == -1 && !coll.OnRightWall)
        {
            PlayerSide *= -1;
            anim.Flip(PlayerSide);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.OnRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        WallJumped = true;
    }

    IEnumerator DisableMovement(float time)
    {
        CanMove = false;
        yield return new WaitForSeconds(time);
        CanMove = true;
    }

    private void Jump(Vector2 dir, bool wall)
    {
        //slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        //ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * JumpForce;

        //particle.Play();
    }

    private void GroundTouch()
    {
        hasDashed = false;
        IsDashing = false;

        if (isDefaultLookLeft)
        {
            PlayerSide = anim.transform.localScale.x <= 0 ? -1 : 1;
        }
        else
        {
            PlayerSide = anim.transform.localScale.x <= 0 ? 1 : -1;
        }

        //jumpParticle.Play();
    }


    #endregion

    #region Dash

    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        //FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        hasDashed = true;

        //anim.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * DashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        //FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        //dashParticle.Play();
        rb.gravityScale = 0;
        betterJump.enabled = false;
        WallJumped = true;
        IsDashing = true;

        yield return new WaitForSeconds(.3f);

        //dashParticle.Stop();
        rb.gravityScale = GravityScale;
        betterJump.enabled = true;
        WallJumped = false;
        IsDashing = false;
    }

    

    private IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.OnGround)
            hasDashed = false;
    }

    private void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    #endregion

    #region Actions



    #endregion


}
