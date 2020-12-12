using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using DG.Tweening;

public class LightArea : MonoBehaviour
{
    /**
     * 아직 미확정.
     * 통째로 지울수도 있는 테스트 클래스임.
     */

    public static LightArea instance;


    public Light2D MainLight;
    public Transform LightPosition;

    public Transform LightEndPosition;
    private Vector3 offSet;


    public bool IsEnter = false;

    private Transform target;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        offSet = MainLight.transform.position - LightEndPosition.transform.position;

        LightManager.Instance.LightCheckSound += LookBySound;
    }

    private void Update()
    {
        if (IsEnter)
        {
            //Look(MainLight.transform, target.transform);


        }
    }

    public void LookBySound(Vector3 source, int value)
    {
        if (!IsEnter)
        {
            return;
        }

        // 사운드 들은것을 Queue로 관리한다.


        MainLight.transform.DOKill();
        MainLight.transform.DOMove(source + offSet, 0.2f).SetEase(Ease.Linear);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsEnter)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Enter");

            //MainLight.transform.position = LightPosition.position;

            //target = collision.transform;

            IsEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsEnter)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            IsEnter = false;
        }
    }

    private void Move(Transform light, Transform target)
    {

    }

    private void Look(Transform light, Transform target)
    {
        Vector3 ShootPoint = target.transform.position;

        float AngleRad = Mathf.Atan2(ShootPoint.y - light.position.y, ShootPoint.x - light.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        light.rotation = Quaternion.Euler(0, 0, AngleDeg + 90);

    }

}
