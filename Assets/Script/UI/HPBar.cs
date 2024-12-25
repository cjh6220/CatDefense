using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    Transform target;
    Vector3 _pos;

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    private void Update() 
    {
        if (target == null)
        {
            return;
        }
        else
        {
            // _pos.x = transform.parent.position.x + target.transform.position.x;
            // _pos.y = transform.parent.position.y + (Mathf.Tan(Mathf.Deg2Rad * 15f) * target.transform.position.z) + 1f;
            // _pos.z = transform.position.z;
            transform.position = Camera.main.WorldToScreenPoint(new Vector3(target.position.x, target.position.y + 1f, 0));
        }
    }
}
