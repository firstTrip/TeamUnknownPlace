using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerLightMovement : MonoBehaviour
{
    public float Range = 1.0f;
    public float MoveTime = 1.0f;
    private Vector3 origin;

    private void Awake()
    {
        origin = Vector3.zero;
    }

    public void StartMovement()
    {
        transform.DOKill();

        Vector2 rand = (Vector2)origin + Random.insideUnitCircle * Range;
        transform.DOLocalMove(rand, MoveTime).OnComplete(()=> {
            StartMovement();
        });
    }

    public void EndMovement()
    {
        transform.DOKill();

        transform.DOLocalMove(origin, MoveTime);
    }
}
