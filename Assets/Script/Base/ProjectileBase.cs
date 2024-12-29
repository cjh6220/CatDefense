using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] TrailRenderer trailRenderer;
    GameObject _target;
    float _speed = 5f;
    Vector2 _lastPos = new Vector2();

    public void SetProjectile(GameObject target)
    {
        trailRenderer.Clear();
        _target = target;
    }

    void Despawn()
    {
        ResourcePoolManager.Despawn(this.gameObject);
        _target = null;
    }

    private void FixedUpdate() 
    {
        if (_target == null)
        {
            Despawn();
            return;
        }
        else
        {
            _lastPos = _target.transform.position;
            var dir = (_target.transform.position - this.transform.position).normalized;
            transform.position += transform.up * _speed * Time.deltaTime;
            transform.up = Vector2.Lerp(transform.up, dir, 0.25f);
        }

        var dist = Vector2.Distance(this.transform.position, _target.transform.position);
        if (dist <= 0.5f)
        {
            Debug.LogError("Hit!");
            Despawn();
        }
    }
}
