using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Sprite img;
    [SerializeField] int currentPoint;
    NavMeshAgent agent;
    bool isInit = false;
    bool firstFrame = true;
    private void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        InitMonster();
        target = PointManager.Instance.GetFirstPoint();
        currentPoint = 0;
        isInit = true;
    }

    void InitMonster()
    {
        sr.sprite = img;
        HPBarManager.Instance.AddNewBar(this.transform);
    }

    private void Update() 
    {
        if (!isInit) return;
        agent.SetDestination(target.position);
        if (firstFrame)
        {
            firstFrame = false;
        }
        else
        {
            if (agent.remainingDistance <= 0.1f)
            {
                SetNextPoint();
                agent.SetDestination(target.position);
            }
        }
    }

    void SetNextPoint()
    {   
        var point = PointManager.Instance.GetNextPoint(currentPoint);
        if (point != null)
        {
            currentPoint++;
            target = point;
        }
    }
}
