using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPlacementHelper
{
    Dictionary<PlacementType, HashSet<Vector2Int>> 
        tileByType = new Dictionary<PlacementType, HashSet<Vector2Int>>();

    HashSet<Vector2Int> validNodeFloorPositions;



    public ItemPlacementHelper(HashSet<Vector2Int> nodeFloorPositions, 
                                HashSet<Vector2Int> pathPositions)
    {
        validNodeFloorPositions = GetPositionsWithoutPath(nodeFloorPositions, pathPositions);
        tileByType = UpdateDictionary();
    }



    public Vector2Int? GetItemPlacementPosition(PlacementType placementType, int maxIterations, Vector2Int size)
    {
        int itemArea = size.x * size.y;
        if (placementType == PlacementType.SurroundingWalkable && tileByType[placementType].Count < itemArea)
            return null;
        else if (placementType == PlacementType.Anywhere)
        {   // if placementType is anywhere, we check all spots to see if there is space
            int totalCount = 0;
            foreach(var kvp in tileByType)
            {
                totalCount += kvp.Value.Count;
            }
            if (totalCount < itemArea)
                return null;
        }
        
        int iteration = 0;
        while (iteration < maxIterations)   // This is just if the random position found is invalid
        {
            iteration++;
            int index = UnityEngine.Random.Range(0, tileByType[placementType].Count);
            Vector2Int position = tileByType[placementType].ElementAt(index);

            if (itemArea > 1)
            {
                
            }
            else
            {
                validNodeFloorPositions.Remove(position);
                tileByType = UpdateDictionary();
            }

            return position;
        }
        return null;
    }



    private Dictionary<PlacementType, HashSet<Vector2Int>> UpdateDictionary()
    {
        NeighbourPositions neighbourPositions = new NeighbourPositions(validNodeFloorPositions);
        Dictionary<PlacementType, HashSet<Vector2Int>> dictionary = new Dictionary<PlacementType, HashSet<Vector2Int>>();
        foreach (var position in validNodeFloorPositions)
        {
            int neighboursCount8Dir = neighbourPositions.Get8DirectionNeighbours(position).Count;

            // Determining the type of item that can be placed on that tile
            PlacementType type = neighboursCount8Dir < 8 ? PlacementType.Anywhere : PlacementType.SurroundingWalkable;
            
            if (!dictionary.ContainsKey(type))
                dictionary[type] = new HashSet<Vector2Int>();
            
            dictionary[type].Add(position);
        }
        return dictionary;
    }



    private HashSet<Vector2Int> GetPositionsWithoutPath(HashSet<Vector2Int> nodeFloorPositions, HashSet<Vector2Int> pathPositions)
    {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>(nodeFloorPositions);
        positions.ExceptWith(pathPositions);
        return positions;
    }
}

public enum PlacementType
{
    Anywhere,
    SurroundingWalkable
}