using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarManager : Singleton<HPBarManager>
{
    [SerializeField] HPBar hPBar;
    public async void AddNewBar(Transform target)
    {
        var bar = await ResourcePoolManager.GetAsync<HPBar>("HPBar", true, this.transform);
        bar.SetTarget(target);
    }
}
