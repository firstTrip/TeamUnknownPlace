using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class BadBoy : MonoBehaviour
{


    [SerializeField] private SkeletonAnimation skeletonAnimation = null;
    [SerializeField] private AnimationReferenceAsset[] AnimClip = null;


  
    private string currentAnimation;
    private AnimState _AnimeState;

    private Rigidbody2D otherRb;
    public GameObject OtherBoy;

    public float pushForce;
    private enum AnimState
    {
        push
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void ObjectAction()
    {

        _AnimeState = AnimState.push;
        SetCurrentAnimation(_AnimeState);

        otherRb= OtherBoy.GetComponent<OtherBoy>().rb;
        OtherBoy.GetComponent<OtherBoy>().rollAnim();
        otherRb.gravityScale = 1;
        otherRb.velocity = new Vector2 ((-1) * pushForce, 1);

    }

    #region 

    private void AsncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
    {

        if (animClip.name.Equals(currentAnimation))
            return;

        //해당 애님으로 변경
        skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
        currentAnimation = animClip.name;

    }
    #endregion

    #region SetCurrentAnimation
    private void SetCurrentAnimation(AnimState _state)
    {

        switch (_state)
        {

            case AnimState.push:
                AsncAnimation(AnimClip[(int)AnimState.push], false, 1f);
                break;
        }

    }

    #endregion
}
