using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    float atkRange = 3f;
    float atkCoolTime = 0.2f;
    float _currentTime = 0f;
    bool isInit = false;
    bool attackReady = false;
    int _order;

    public async void SetUnit(string unitName, int order)
    {
        _order = order;
        var newUnit = await ResourcePoolManager.GetAsync(unitName, true, this.transform);
        newUnit.transform.localPosition = Vector3.zero;
        newUnit.transform.localRotation = Quaternion.identity;
        newUnit.transform.localScale = Vector3.one;

        isInit = true;
    }

    async UniTask Attack()
    {
        var projectile = await ResourcePoolManager.GetAsync<ProjectileBase>("ProjectileBase", true, null);
        projectile.transform.position = this.transform.position;
        projectile.transform.localRotation = Quaternion.identity;
        projectile.transform.localScale = Vector3.one;
        projectile.SetProjectile();
    }

    private async void Update()
    {
        if (!isInit) return;

        _currentTime += Time.deltaTime;
        if (_currentTime >= atkCoolTime)
        {
            _currentTime -= atkCoolTime;
            //attackReady = true;
            await Attack();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRange);
    }
}
