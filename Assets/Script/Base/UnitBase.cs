using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    float atkRange = 3f;
    float atkCoolTime = 2f;
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
        var target = BattleManager.Instance.GetCloseEnemy(this.transform.position);
        if (target != null)
        {
            var dist = Vector2.Distance(this.transform.position, target.transform.position);
            if (dist > atkRange)
            {
                
            }
            else
            {
                attackReady = false;
             
                var projectile = await ResourcePoolManager.GetAsync<ProjectileBase>("ProjectileBase", true, this.transform);
                projectile.transform.localPosition = Vector3.zero;
                projectile.transform.localRotation = Quaternion.identity;
                projectile.transform.localScale = Vector3.one;
                projectile.SetProjectile(target);
                Debug.LogError("Shot = " + _order);

            }
        }
    }

    private async void Update() 
    {
        if (!isInit) return;
        if (attackReady)
        {
            await Attack();
            return;
        }

        _currentTime += Time.deltaTime;
        if (_currentTime >= atkCoolTime)
        {
            _currentTime -= atkCoolTime;
            attackReady = true;
            await Attack();
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRange);
    }
}
