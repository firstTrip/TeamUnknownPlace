using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MainLightAction : ActionObject
{

    public EnumMainLightAction TargetAction;

    protected override void AfterCheckAction()
    {
        switch (TargetAction)
        {
            case EnumMainLightAction.DeadLock:
                LightManager.Instance.MainLight.DeadLock();
                break;

            case EnumMainLightAction.Banish:
                LightManager.Instance.MainLight.Banish();
                break;

            case EnumMainLightAction.Reveal:
                LightManager.Instance.MainLight.Reveal();
                break;

            case EnumMainLightAction.Bounce:
                Debug.LogError("MLA Bounce 미구현");
                break;
        }
    }

}
