using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D rb;

    private bool canMove;
    private bool walk;
    private bool dash;

    private PlayerCollision coll;


    [SerializeField]private float JumpForce;
    [SerializeField] private float moveForce;
    [SerializeField] private float moveSpeed;

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


        if (Input.GetButtonUp("Horizontal"))
        {
            rb.velocity = new Vector2(rb.velocity.normalized.x * 0, rb.velocity.y);
        }

        if (coll.OnGround) 
            Jump();
    }

    private void init()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision>();
        canMove = true;
        moveSpeed = 1f;
        moveForce = 1.5f;
        JumpForce = 8f;
    }

    private void InputManager()
    {
        walk = Input.GetKey(KeyCode.LeftControl);
        dash = Input.GetKey(KeyCode.LeftShift);

    }

    private void Move()
    {

        if (!canMove)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);


        if (walk)
            moveForce = 0.5f;
        else if (dash)
            moveForce = 3f;
        else
            moveForce = 1f;

        if (rb.velocity.x > moveForce)
        {
            rb.velocity = new Vector2(moveForce, rb.velocity.y);
            WalkSound();
        }
        else if (rb.velocity.x < moveForce * (-1))
        {
            rb.velocity = new Vector2(moveForce * (-1), rb.velocity.y);
            WalkSound();
        }



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
            StartCoroutine(DisableMovement(1f));
        }
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

}
