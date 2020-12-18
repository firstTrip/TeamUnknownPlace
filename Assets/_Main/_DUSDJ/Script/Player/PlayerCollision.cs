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

    [Space]

    public bool OnGround;
    public bool OnWall;
    public bool OnLope;
    public bool OnItem;
    public bool OnHideItem;
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
        OnLope = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, CollisionRadius, ropeLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, CollisionRadius, ropeLayer);

        OnItem = Physics2D.OverlapCircle((Vector2)transform.position + itemOffset, ItemCollisionRadius, itemLayer);

        OnHideItem = Physics2D.OverlapCircle((Vector2)transform.position + itemOffset, ItemCollisionRadius, hideItemLayer);

        OnRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, CollisionRadius, groundLayer);
        OnLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, CollisionRadius, groundLayer);

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