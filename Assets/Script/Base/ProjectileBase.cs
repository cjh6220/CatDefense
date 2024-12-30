using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] TrailRenderer trailRenderer;
    float _speed = 20f;
    Vector2 _lastPos = new Vector2();

    public void SetProjectile()
    {
        trailRenderer.Clear();
    }

    void Despawn()
    {
        ResourcePoolManager.Despawn(this.gameObject);
    }

    private void FixedUpdate()
    {
        transform.position += transform.up * _speed * Time.deltaTime;
    }
}
