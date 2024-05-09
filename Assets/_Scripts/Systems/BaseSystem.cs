using System.Collections.Generic;
using UnityEngine;

public class BaseSystem : SingletonPersistent<BaseSystem>
{
    // Set up target franerate
    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }
}
