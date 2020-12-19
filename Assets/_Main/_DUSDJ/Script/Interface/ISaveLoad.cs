using UnityEngine;

public interface ISaveLoad
{
    void ISaveLoadInit();

    void ISave();

    void ILoad();

    void ISaveDelete();

    GameObject GetGameObject();
}