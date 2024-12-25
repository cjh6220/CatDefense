using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] Transform[] SpawnPoint;
    [SerializeField] GameObject[] UnitList;
    [SerializeField] NavMeshSurface navMeshSurfaces;
    [SerializeField] Transform StartPoint;
    [SerializeField] GameObject MonsterObj;
    [SerializeField] Transform EnemyTrans;
    List<GameObject> EnemyList = new List<GameObject>();    
    float _genTime = 1f;
    float _currentTime = 0f;

    private void Start() 
    {
        for (int i = 0; i < SpawnPoint.Length; i++)
        {
            var ran = Random.Range(0, UnitList.Length);
            var newUnit = Instantiate(UnitList[ran], SpawnPoint[i]);
        }

        navMeshSurfaces.BuildNavMesh();
    }

    private void Update() 
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _genTime)
        {
            var target = Instantiate(MonsterObj);
            target.transform.position = StartPoint.position;
            EnemyList.Add(target);
            _currentTime -= _genTime;
        }
    }
}
