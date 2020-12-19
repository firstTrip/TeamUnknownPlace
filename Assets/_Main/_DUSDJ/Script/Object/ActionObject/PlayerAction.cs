using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerActoin : ActionObject
{
    public EnumPlayerAction TargetAction;

    protected override void AfterCheckAction()
    {
        switch (TargetAction)
        {
            case EnumPlayerAction.Dead:
                GameManager.Instance.PlayerChara.Dead();
                break;

            case EnumPlayerAction.DeadByWater:
                GameManager.Instance.PlayerChara.DeadByWater();
                break;
        }
    }
}