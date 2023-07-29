using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum PointType
{
    Default, Wall, Floor, Door, Window, Character, Roof
}


public class DecoratorPoint
{
   

    public GameObject pointObject;
    public Vector3 point;
    public PointType pointType;
    public bool occupied;
    public int levelNumber;

    public DecoratorPoint(GameObject pointObject_, Vector3 point_, PointType pointType_, bool occupied_, int levelNumber_)
    {
        pointObject = pointObject_;
        point = point_;
        pointType = pointType_;
        occupied = occupied_;
        levelNumber = levelNumber_;
    }

    
}
