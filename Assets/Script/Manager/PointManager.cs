using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : Singleton<PointManager>
{
    [SerializeField] Transform[] Points;

    public Transform GetFirstPoint()
    {
        return Points[0];
    }

    public Transform GetNextPoint(int currentPoint)
    {
        if (currentPoint + 1 >= Points.Length)
        {
            return null;   
        }
        else
        {
            return Points[currentPoint + 1];
        }
    }
}
