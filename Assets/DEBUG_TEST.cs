using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class DEBUG_TEST : MonoBehaviour
{
#if UNITY_EDITOR
    public bool try1 = false;
    public bool try2 = false;
    private Stack<GameObject> temp = new Stack<GameObject>();

    private void Update()
    {
        if (try1)
        {
            try1 = false;
            var go = Managers.Resource.Instantiate("Prefabs/Monster/MonsterTest");
            go.transform.position = Vector3.zero;

            temp.Push(go);
        }

        if (try2)
        {
            try2 = false;
            if (temp.Count > 0)
                Managers.Resource.Destroy(temp.Pop());
        }
    }
#endif
}