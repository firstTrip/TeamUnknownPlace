using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat_Sample : MonoBehaviour
{
    // 주기적으로 소리를 발생시키는 클래스로 발전시킬 예정

    private bool IsActive = false;

    [Header("소리발생 쿨다운")] public float ActionCoolDown = 3.0f;
    private float ActionCoolDownNow = 0f;
    [Header("소리 밸류")] public int SoundValue = 5;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        IsActive = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            IsActive = true;
        }

        if (IsActive)
        {
            Action();
        }
    }

    private void Action()
    {
        if (ActionCoolDownNow > 0)
        {
            ActionCoolDownNow -= Time.deltaTime;
        }
        else
        {
            ActionCoolDownNow = ActionCoolDown;

            EffectManager.Instance.SetPool("SoundWave", transform.position, new Vector3(2.0f, 2.0f, 1.0f));
            AudioManager.Instance.PlaySound("Cat", SoundValue, transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsActive = false;
        }
    }



}
