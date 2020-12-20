using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadSound : MonoBehaviour
{


    [Header("Sound Data")] public StructSoundData SoundData;
    [Header("루프여부")] public bool SoundLoop = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void ObjectAction()
    {
        AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, gameObject);
        AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, gameObject);
    }

    public void ObjectAction2()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
