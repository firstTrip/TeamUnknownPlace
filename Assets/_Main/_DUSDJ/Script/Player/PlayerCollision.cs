using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]

    public bool OnGround;
    public bool OnWall;
    public bool OnRightWall;
    public bool OnLeftWall;
    public int WallSide;

    [Space]

    [Header("Collision")]

    public float CollisionRadius = 0.5f;
    public Vector2 BottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;

    // Update is called once per frame
    void Update()
    {
        OnGround = Physics2D.OverlapCircle((Vector2)transform.position + BottomOffset, CollisionRadius, groundLayer);
        OnWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, CollisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, CollisionRadius, groundLayer);

        OnRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, CollisionRadius, groundLayer);
        OnLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, CollisionRadius, groundLayer);

        WallSide = OnRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { BottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + BottomOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, CollisionRadius);
    }

}