using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleInfo", menuName = "MapGeneration/ObstacleInfo", order = 1)]
public class ObstacleInfo : ScriptableObject
{
    public Vector2Int size;    // Transform Size
    public PlacementType placementType = PlacementType.ColliderActive; // Object placement type
    public Boolean networkObject = false;
    // public int minCount;  // Minimum number of this obstacle to place down
    // public int maxCount;  // Maximum number of this obstacle to place down
}
