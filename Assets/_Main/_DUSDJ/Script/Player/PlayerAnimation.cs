using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController controller;
    private PlayerCollision coll;

    private Animator anim;

    private Vector3 originScale;

    void Start()
    {
        coll = GetComponentInParent<PlayerCollision>();
        controller = GetComponentInParent<PlayerController>();

        anim = GetComponent<Animator>();

        originScale = transform.localScale;
    }

    void Update()
    {
        anim.SetBool("onGround", coll.OnGround);
        anim.SetBool("onWall", coll.OnWall);
        anim.SetBool("onRightWall", coll.OnRightWall);

        anim.SetBool("wallGrab", controller.WallGrab);
        anim.SetBool("wallSlide", controller.IsWallSlide);
        anim.SetBool("canMove", controller.CanMove);
        anim.SetBool("isDashing", controller.IsDashing);
    }

    public void SetHorizontalMovement(float x, float y, float yVel)
    {
        anim.SetFloat("HorizontalAxis", x);
        anim.SetFloat("VerticalAxis", y);
        anim.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public void Flip(int side)
    {

        if (controller.WallGrab || controller.IsWallSlide)
        {
            if (side == -1 && IsLookLeft())
                return;

            if (side == 1 && !IsLookLeft())
            {
                return;
            }
        }

        // Look Right
        if (side == 1)
        {
            if (controller.isDefaultLookLeft)
            {
                transform.localScale = new Vector3(originScale.x * -1, originScale.y, originScale.z);
            }
            else
            {
                transform.localScale = originScale;
            }
        }
        // Look Left
        else
        {
            if (controller.isDefaultLookLeft)
            {
                transform.localScale = originScale;
            }
            else
            {
                transform.localScale = new Vector3(originScale.x * -1, originScale.y, originScale.z);
            }
            
        }
        
    }

    public bool IsLookLeft()
    {
        if (controller.isDefaultLookLeft && transform.localScale.x > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
