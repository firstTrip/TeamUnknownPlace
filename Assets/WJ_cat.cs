using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class WJ_cat : MonoBehaviour
{

    [Header("Sound Data")] public StructSoundData SoundData;
    [Header("루프여부")] public bool SoundLoop = false;


    private enum CatAnim { Walk, Touch }
    private CatAnim _catAnim;

    private string currentAnimation;

    [SerializeField] private SkeletonAnimation skeletonAnimation = null;
    [SerializeField] private AnimationReferenceAsset[] AnimClip = null;

    public GameObject Phon;
    public GameObject DropPlace;

    // Start is called before the first frame update


    private void Awake()
    {
        _catAnim = CatAnim.Walk;
        SetCurrentAnimation(CatAnim.Walk);
    }

    public void ObjectAction()
    {
        LightManager.Instance.Reveal();
        _catAnim = CatAnim.Touch;
        SetCurrentAnimation(CatAnim.Touch);
        Phon.transform.position = Vector3.MoveTowards(Phon.transform.position, DropPlace.transform.position, 1f);
        AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, gameObject);
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

    private void SetCurrentAnimation(CatAnim _catAnim)
    {

        switch (_catAnim)
        {
            // ( 애님 이름  , 루프 여부 , 재생시간)
            case CatAnim.Walk:
                AsncAnimation(AnimClip[(int)CatAnim.Walk], true, 1f);
                break;

            case CatAnim.Touch:
                AsncAnimation(AnimClip[(int)CatAnim.Touch], true, 0.5f);
                break;

        }

    }
}
