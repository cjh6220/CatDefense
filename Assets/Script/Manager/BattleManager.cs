using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] Transform[] SpawnPoint;
    [SerializeField] GameObject[] UnitList;
    [SerializeField] Transform EnemyTrans;
    List<GameObject> EnemyList = new List<GameObject>();    
    float _genTime = 1f;
    float _currentTime = 0f;

    private async void Start() 
    {
        // for (int i = 0; i < SpawnPoint.Length; i++)
        // {
        //     var point = SpawnPoint[i];
        //     var ran = Random.Range(1, UnitList.Length + 1);
        //     var newUnit = await ResourcePoolManager.GetAsync<UnitBase>("Unit_Base", true, point);//  Instantiate(UnitList[ran], SpawnPoint[i]);
        //     newUnit.transform.localPosition = Vector3.zero;
        //     newUnit.transform.localRotation = Quaternion.identity;
        //     newUnit.transform.localScale = Vector3.one;
        //     newUnit.SetUnit("Unit_" + ran, i);
        // }
    }

    async void SpawnEnemy()
    {
        var target = await ResourcePoolManager.GetAsync("Monster", true, EnemyTrans);
        target.transform.position = PointManager.Instance.GetRandomPoint();
        EnemyList.Add(target);
    }

    private void Update() 
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _genTime)
        {
            SpawnEnemy();
            _currentTime -= _genTime;
        }
    }

    public GameObject GetCloseEnemy(Vector2 unitPos)
    {
        GameObject targetObj = null;
        float dist = 100f;
        foreach (var item in EnemyList)
        {
            var tempDist = Vector2.Distance(unitPos, item.transform.position);
            if (tempDist < dist)
            {
                dist = tempDist;
                targetObj = item;
            }
        }
        return targetObj;
    }
}
