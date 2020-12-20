using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : Item, ISaveLoad
{

    public float xThrow;
    public float yThrow;

    private bool isActive;
    private float x;

    Transform parent;
    Transform middleGround;


    protected override void Start()
    {
        base.Start();

        isActive = true;
        OnGround = false;

        UseMaterial();

        ISaveLoadInit();
        middleGround = GameObject.Find("Middleground_AP").transform;
        parent = middleGround;
    }


    private void Update()
    {
        OnGround = Physics2D.OverlapCircle((Vector2)transform.position + BottomOffset, CollisionRadius, groundLayer);

        if (OnGround)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
    }

    public void GetItemByPlayer(Player p, Transform hand)
    {
        if (isGet)
        {
            return;
        }

        p.ItemInHand = this;

        parent = hand;
        transform.SetParent(parent, true);
        transform.localPosition = Vector3.zero;        

        isGet = true;
        isActive = true;
    }

    public override void UseItem()
    {        
        x = parent.GetComponentInParent<Player>().transform.localScale.x;

        gameObject.transform.SetParent(middleGround, true);

        parent = null;
        isActive = false;
        StartCoroutine(DisableToGet(0.5f));

        rb.velocity = new Vector2(x * xThrow * (-1) , yThrow);
        rb.gravityScale = 1;
    }

    public override void UseMaterial()
    {
        base.UseMaterial();
    }


    IEnumerator DisableToGet(float time)
    {
        isGet = true;
        yield return new WaitForSecondsRealtime(time);
        isGet = false;
    }

    #region ISaveLoad

    public struct StructSaveData
    {
        public Transform parent;
        public Vector3 Position;
        public bool isGet;
        public bool isActive;
    }
    public StructSaveData SaveData;

    public void ISaveLoadInit()
    {
        SaveManager.Instance.AddSaveObject(this);
    }

    public void ISave()
    {
        SaveData.Position = transform.position;
        SaveData.isGet = isGet;
        SaveData.isActive = isActive;
    }

    public void ILoad()
    {
        transform.position = SaveData.Position;
        isGet = SaveData.isGet;
        isActive = SaveData.isActive;
    }

    public void ISaveDelete()
    {
        SaveManager.Instance.DeleteSaveObject(this);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #endregion

}
