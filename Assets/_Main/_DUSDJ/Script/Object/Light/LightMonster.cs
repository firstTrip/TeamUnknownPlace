using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LightMonster : MonoBehaviour
{
    public float MoveSpeed;
    public Transform Target;

    [Header("플레이어 추적중인지 대기중인지")] public bool IsHunting;

    private int layerMask;
    private Player player;
    private bool IsActive = false;

    public void Init()
    {
        player = GameManager.Instance.PlayerChara;
        layerMask = 1 << LayerMask.NameToLayer("BlueLight");

        IsActive = true;
    }


    private void Update()
    {
        if (!IsActive)
        {
            return;
        }

        DebugManager.Instance.SetText(DebugManager.Instance.IsHuntingBox, IsHunting.ToString());

        if (IsHunting)
        {
            Hunt();            
        }
        else
        {
            Gaze();
        }
    }



    public void SetTarget(Transform target)
    {
        Target = target;
    }

    private void Hunt()
    {
        if (Target != null)
        {
            // 임시 이동
            Vector3 diretion = (Target.position - transform.position).normalized;
            transform.Translate(diretion * MoveSpeed * Time.deltaTime);
        }
        else
        {
            // 임시
            Target = player.transform;
        }
    }

    private void Gaze()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        DebugManager.Instance.SetText(DebugManager.Instance.DistanceBox, distance.ToString());
       
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, layerMask);
        Debug.DrawRay(transform.position, direction, Color.red, 0.1f);

        if(hit.collider != null)
        {
            DebugManager.Instance.SetText(DebugManager.Instance.IsHitBlueLightBox, hit.collider.gameObject.name);

        }
        else
        {
            DebugManager.Instance.SetText(DebugManager.Instance.IsHitBlueLightBox, "False");
            IsHunting = true;
        }
    }

    public void Bounce(Vector3 source, float power)
    {
        Debug.Log("Bounce!");

        IsActive = false;
        IsHunting = false;

        Vector3 direction = (transform.position - source).normalized;
        direction.z = 0;
        Vector3 endValue = transform.position + direction * power;
        
        Debug.DrawLine(transform.position, endValue, Color.white, 10f);

        transform.DOMove(endValue, 0.3f).SetEase(Ease.OutBack).OnComplete(()=> {
            IsActive = true;
        });

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(IsActive == false)
        {
            return;
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("BlueLight"))
        {
            Bounce(collision.transform.position, 2.0f);
            return;
        }
    }
}
