using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class OtherBoy : MonoBehaviour
{


    [SerializeField] private SkeletonAnimation skeletonAnimation = null;
    [SerializeField] private AnimationReferenceAsset[] AnimClip = null;


    private enum AnimState
    {
        idle, roll
    }
    private string currentAnimation;
    private AnimState _AnimeState;
    public GameObject Boy;
    public Rigidbody2D BoyRb;

    [Header("Sound Data")] public StructSoundData SoundData;
    [Header("루프여부")] public bool SoundLoop = false;


    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        _AnimeState = AnimState.idle;
        SetCurrentAnimation(_AnimeState);

    }

    public void rollAnim()
    {
        _AnimeState = AnimState.roll;
        SetCurrentAnimation(_AnimeState);
        AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, gameObject);
    }

    public void ObjectAction()
    {

        BoyRb = Boy.GetComponent<Player>();
        Boy.GetComponent<OtherBoy>().rollAnim();
        BoyRb.gravityScale = 1;
        BoyRb.velocity = new Vector2((-1) * 2, 1);

    }

    private void Update()
    {
        Debug.Log(rb.velocity.x);
    }

    private void AsncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
    {

        if (animClip.name.Equals(currentAnimation))
            return;

        //해당 애님으로 변경
        skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
        currentAnimation = animClip.name;

    }

    private void SetCurrentAnimation(AnimState _state)
    {

        switch (_state)
        {

            case AnimState.idle:
                AsncAnimation(AnimClip[(int)AnimState.idle], false, 1f);
                break;

            case AnimState.roll:
                AsncAnimation(AnimClip[(int)AnimState.roll], false, 1f);
                break;
        }

    }

}
