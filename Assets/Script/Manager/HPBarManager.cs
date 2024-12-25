using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarManager : Singleton<HPBarManager>
{
    [SerializeField] HPBar hPBar;
    public void AddNewBar(Transform target)
    {
        var bar = Instantiate(hPBar, this.transform);
        bar.SetTarget(target);
    }
}
