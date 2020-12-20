using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    [Header("Layers")]
    [Tooltip("Floor 판별")] public LayerMask groundLayer;
    [Tooltip("Rope 판별")] public LayerMask ropeLayer;
    [Tooltip("Item 판별")] public LayerMask itemLayer;
    [Tooltip("HideItem 판별")] public LayerMask hideItemLayer;
    [Tooltip(" 계단 판별")] public LayerMask stairLayer;

    [Space]

    public bool OnGround;
    public bool OnWall;
    public bool OnRope;
    public bool OnItem;
    [HideInInspector]public Collider2D ItemCollider { get; set; }

    public bool OnHideItem;
    [HideInInspector] public Collider2D HideItemCollider { get; set; }

    public bool OnRightWall;
    public bool OnLeftWall;

    [Space]

    [Header("Collision")]

    public float CollisionRadius = 0.15f;
    public float ItemCollisionRadius = 0.5f;
    public float BoxSize = 0.5f;
    public Vector2 BottomOffset, rightOffset, leftOffset , itemOffset;

    // Update is called once per frame
    void Update()
    {
        OnGround = Physics2D.OverlapCircle((Vector2)transform.position + BottomOffset, CollisionRadius, groundLayer);
        OnWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, CollisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, CollisionRadius, groundLayer);
        OnRope = Physics2D.OverlapCircle((Vector2)transform.position + itemOffset, ItemCollisionRadius, ropeLayer);

        ItemCollider = Physics2D.OverlapCircle((Vector2)transform.position + itemOffset, ItemCollisionRadius, itemLayer);
        if(ItemCollider != null)
        {
            OnItem = true;
        }
        else
        {
            OnItem = false;
        }


        HideItemCollider = Physics2D.OverlapCircle((Vector2)transform.position + itemOffset, ItemCollisionRadius, hideItemLayer);
        if (HideItemCollider != null)
        {
            OnHideItem = true;
        }
        else
        {
            OnHideItem = false;
        }

        OnRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, CollisionRadius, stairLayer);

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { BottomOffset, rightOffset, leftOffset  };

        Gizmos.DrawWireSphere((Vector2)transform.position + BottomOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + itemOffset, ItemCollisionRadius);
    }

}