using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleInfo", menuName = "MapGeneration/BiomeInfo", order = 2)]
public class BiomeInfo : ScriptableObject
{
    public Items[] items;
}

[System.Serializable]
public struct Items
{
    public int minCount;
    public int maxCount;
    public ObstacleInfo itemData;
}
