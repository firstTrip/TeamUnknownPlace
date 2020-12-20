using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class OtherBoy : MonoBehaviour
{


    [SerializeField] private SkeletonAnimation skeletonAnimation = null;
    [SerializeField] private AnimationReferenceAsset[] AnimClip = null;


  
    private string currentAnimation;

    private enum State
    {
        idle, roll
    }
    private State state;

    private Rigidbody2D BoyRb;

    [Header("Sound Data")] public StructSoundData SoundData;
    [Header("루프여부")] public bool SoundLoop = false;

    [Header("Effect Data")] public string EffectKey;
    [Header("이펙트 끝나는 시간")] public float EffectDuration;
    [Header("이펙트 크기")] public Vector3 EffectScale = Vector3.one;

    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        init();

    }

    private void init()
    {
        rb = GetComponent<Rigidbody2D>();
        //_AnimeState = AnimState.idle2;
        //SetCurrentAnimation(_AnimeState);
    }

    public void rollAnim()
    {
        state = State.roll;
        SetCurrentAnimation(state);
        AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, gameObject);
        //EffectManager.Instance.SetPool(EffectKey, transform.position, EffectScale);
    }


    private void AsncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
    {

        if (animClip.name.Equals(currentAnimation))
            return;

        //해당 애님으로 변경
        skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
        currentAnimation = animClip.name;

    }

    private void SetCurrentAnimation(State _state)
    {

        switch (_state)
        {
           
            case State.idle:
                AsncAnimation(AnimClip[(int)State.idle], false, 1f);
                break;
         
            case State.roll:
                AsncAnimation(AnimClip[(int)State.roll], false, 1f);
                break;
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BoyRb = collision.GetComponent<Player>().rb;
            collision.GetComponent<Player>().RollAnimPlayer();
            BoyRb.gravityScale = 1;
            BoyRb.velocity = new Vector2((-1) * 2, 1);
        }
    }
}
