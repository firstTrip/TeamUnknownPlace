using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D rb;

    private bool canMove;
    [SerializeField]private float JumpForce;
    [SerializeField] private float moveForce;


    [Header("발소리 쿨다운")] public float WalkSoundCoolDown = 0.25f;
    private float WalkSoundCoolDonwNow = 0f;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        if (WalkSoundCoolDonwNow > 0)
        {
            WalkSoundCoolDonwNow -= Time.deltaTime;
        }

        Move();
        Jump();
    }

    private void init()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        canMove = true;

        moveForce = 12f;
        JumpForce = 10f;
    }

    private void Move()
    {

        float h = Input.GetAxisRaw("Horizontal");

        if (!canMove)
            return;

        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rb.velocity.x > moveForce)
            rb.velocity = new Vector2(moveForce, rb.velocity.y);        
        else if (rb.velocity.x < moveForce * (-1))
            rb.velocity = new Vector2(moveForce*(-1), rb.velocity.y);

        WalkSound();
    }

    private void WalkSound()
    {
        #region WalkSound
        if (WalkSoundCoolDonwNow <= 0)
        {
            AudioManager.Instance.PlayOneShot("Footstep");
            EffectManager.Instance.SetPool("SoundWave", transform.position, new Vector3(0.5f, 0.5f, 1f));
            LightManager.Instance.LightCheckSound(transform.position, 2);
            WalkSoundCoolDonwNow = WalkSoundCoolDown;
        }
        #endregion
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("asd");
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }
    }
}
