using System.Collections;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public float Duration;  // 지속시간
    private float BaseDuration;

    public Transform FollowTarget;
    public Vector3 Adder;

    private Renderer objectToFollow;

    private SpriteRenderer spr;
    private Animator anim;
    private ParticleSystem ps;

    IEnumerator coroutine;

    private void Awake()
    {
        FollowTarget = null;

        BaseDuration = Duration;
        Adder = Vector3.zero;

        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        ps = GetComponent<ParticleSystem>();
        if(ps == null)
        {
            ps = GetComponentInChildren<ParticleSystem>();
        }

    }

    private void Update()
    {
        if (objectToFollow != null)
        {
            Reposition();
        }
    }

    #region Set Effect


    public void Reposition()
    {
        
    }

    public void SetEffect(Renderer objectToFollow)
    {
        this.objectToFollow = objectToFollow;

        Reposition();

        Action();
    }

    public void SetEffect(Vector3 position)
    {
        Duration = BaseDuration;

        transform.position = position;

        Action();
    }

    public void SetEffect(Vector3 position, Vector3 scale)
    {
        Duration = BaseDuration;
        transform.position = position;
        transform.localScale = scale;
        Action();
    }

    #endregion



    public void Action()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        if (ps != null)
        {
            coroutine = CheckIfParticleAlive();
        }
        else
        {
            coroutine = UpdateCoroutine();
        }

        if (anim != null)
        {
            anim.SetTrigger("Action");
        }

        StartCoroutine(coroutine);
    }

    IEnumerator CheckIfParticleAlive()
    {

        while (true && ps != null)
        {
            yield return new WaitForSeconds(0.5f);
            if (!ps.IsAlive(true))
            {
                Clean();
            }
        }
    }

    IEnumerator UpdateCoroutine()
    {
        float t = 0;

        while (t < Duration)
        {
            if (FollowTarget != null)
            {
                if (FollowTarget.gameObject.activeSelf == false)
                {
                    Clean();
                    yield break;
                }
                else
                {
                    transform.position = FollowTarget.position + Adder;
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        Clean();
    }

    public void Clean()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        objectToFollow = null;
        FollowTarget = null;
        Adder = Vector3.zero;

        gameObject.SetActive(false);
    }
}