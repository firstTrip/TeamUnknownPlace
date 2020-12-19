using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob_Cat : MonoBehaviour
{
    public StructSoundData SoundData;

    [Header("도착지점")]public Transform ArrivalPoint;
    [Header("N번만에 도착함")] public int NumOfMovement = 4;
    [Header("도착 보정")]public float MinDistance = 0.05f;

    [Header("이동속도")] public float MoveSpeed;
    [Header("멈춰서는 시간")] public float MoveDealy = 0.25f;

    private int NumOfActivatedMovement = 0;

    private float stay = 0f;

    private Vector2 startingPoint;
    private Vector2 endingPoint;
    private Vector2 movePoint;
    private bool isArrive = false;
    private bool isActivated = false;


    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        startingPoint = transform.position;
        endingPoint = ArrivalPoint.transform.position;
        movePoint = transform.position;
    }

    private void Update()
    {
        if (!isActivated)
        {
            return;
        }

        Walk();
    }

    public void Load()
    {

    }

    public void ObjectAction()
    {
        if (isActivated)
        {
            return;
        }
        isActivated = true;
    }

    public void MakeSound()
    {
        AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, gameObject);
    }

    public void SetMovePoint()
    {

        if (NumOfActivatedMovement >= NumOfMovement)
        {
            return;
        }

        NumOfActivatedMovement += 1;

        isArrive = false;
        movePoint = Vector2.Lerp(startingPoint, endingPoint, (float)NumOfActivatedMovement / NumOfMovement);        
    }

    public void Walk()
    {
        if (isArrive)
        {
            if (stay >= MoveDealy)
            {
                if (NumOfActivatedMovement >= NumOfMovement)
                {
                    MakeSound();
                    RunAway();
                    return;
                }

                stay = 0;                
                SetMovePoint();
                MakeSound();
                Debug.LogWarning(NumOfActivatedMovement);
            }
            else
            {
                stay += Time.deltaTime;
                return;
            }    
            
            return;
        }

        float distance = Vector2.Distance((Vector2)transform.position, movePoint);
        if (distance <= MinDistance)
        {
            isArrive = true;
            return;
        }

        Vector2 direction = (movePoint - (Vector2)transform.position).normalized;
        transform.Translate(direction * MoveSpeed * Time.deltaTime);
    }

    public void RunAway()
    {
        Debug.Log("고양이 Out");
        gameObject.SetActive(false);
    }
}
