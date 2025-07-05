using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using CharacterController = Content.Character.CharacterController;

public class DEBUG_TEST : MonoBehaviour
{
#if UNITY_EDITOR
    public bool try1 = false;
    public CharacterController character = null;
    
    private void Update()
    {
        if (try1)
        {
            try1 = false;
            character?.Init();
        }
    }
#endif
}