using System;
using Core;
using UnityEngine;

public class DEBUG_TEST : MonoBehaviour
{
    public bool TryNextTick = false;

    private void Update()
    {
        if (TryNextTick)
        {
            TryNextTick = false;
            // var testgo = Managers.Resource.Instantiate("Prefabs/Monster/MonsterTest");
            // var test = Managers.Resource.Load<GameObject>("Prefabs/Monster/MonsterTest");
            Logging.Write($"{Managers.Resource.Load<GameObject>("Prefabs/Monster/MonsterTest")}");
        }
    }
}
