﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask ropeLayer;
    public LayerMask itemLayer;

    [Space]

    public bool OnGround;
    public bool OnWall;
    public bool OnLope;
    public bool OnItem;
    public bool OnRightWall;
    public bool OnLeftWall;
    public bool IsRightWall;
    public bool IsLeftWall;
    public int WallSide;

    [Space]

    [Header("Collision")]

    public float CollisionRadius = 0.15f;
    public float BoxSize = 0.5f;
    public Vector2 BottomOffset, rightOffset, leftOffset , leftWallOffset , rightWallOffset;
    private Color debugCollisionColor = Color.red;

    // Update is called once per frame
    void Update()
    {
        OnGround = Physics2D.OverlapCircle((Vector2)transform.position + BottomOffset, CollisionRadius, groundLayer);
        OnWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, CollisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, CollisionRadius, groundLayer);
        OnLope = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, CollisionRadius, ropeLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, CollisionRadius, ropeLayer);
        OnItem = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, CollisionRadius, itemLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, CollisionRadius, itemLayer);

        OnRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, CollisionRadius, groundLayer);
        OnLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, CollisionRadius, groundLayer);
        IsRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightWallOffset, CollisionRadius, groundLayer);
        IsLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftWallOffset, CollisionRadius, groundLayer);

        WallSide = OnRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { BottomOffset, rightOffset, leftOffset , leftWallOffset, rightWallOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + BottomOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftWallOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightWallOffset, CollisionRadius);
    }

}