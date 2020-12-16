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

    public List<IDamagable> DamageList;

    private void Awake()
    {
        DamageList = new List<IDamagable>();
    }

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

        // Damage
        DamageAll();

        if (IsHunting)
        {
            Hunt();            
        }
        else
        {
            Gaze();
        }
    }

    public void DamageAll()
    {
        for (int i = 0; i < DamageList.Count; i++)
        {
            float distance = Vector2.Distance(transform.position, DamageList[i].GetGameObject().transform.position);
            Debug.Log(string.Format("Damage Distance : {0}", distance));
            DamageList[i].Damage(distance);
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
        // 활동정지시 (순서 아직 고려중)
        if(IsActive == false)
        {
            return;
        }

        // 파랑빛 충돌시 바운스
        if(collision.gameObject.layer == LayerMask.NameToLayer("BlueLight"))
        {
            Bounce(collision.transform.position, 2.0f);
            return;
        }

        // 캐릭터 충돌시 천천히 침식
        if(collision.CompareTag("Player") || collision.CompareTag("Mob"))
        {
            IDamagable id = collision.GetComponent<IDamagable>();
            if (!DamageList.Contains(id))
            {
                Debug.Log("DamageList + " + id.GetGameObject().name);
                DamageList.Add(id);
            }            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 캐릭터 아웃시 침식해제
        if (collision.CompareTag("Player") || collision.CompareTag("Mob"))
        {
            IDamagable id = collision.GetComponent<IDamagable>();
            if (DamageList.Contains(id))
            {
                DamageList.Remove(id);
            }
        }
    }
}
