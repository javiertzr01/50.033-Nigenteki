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
        tileByType = SetupDictionary();
    }



    public Vector2Int? GetItemPlacementPosition(PlacementType placementType, int maxIterations, Vector2Int size, bool addOffset)
    {
        int itemArea = size.x * size.y;

        /// Checking if there are possible tiles ///
        if (placementType == PlacementType.ColliderActive && (!tileByType.ContainsKey(PlacementType.ColliderActive) || tileByType[placementType].Count < itemArea))
            return null;
        else if (placementType == PlacementType.Anywhere)
        {   // if placementType is anywhere, we check all spots to see if there is space
            if (validNodeFloorPositions.Count < itemArea)
                return null;
        }
        
        /// Attemping to place the objects ///
        int iteration = 0;
        while (iteration < maxIterations)   // This is just if the random position found is invalid
        {
            iteration++;
            int index;
            Vector2Int position;

            if (placementType != PlacementType.Anywhere)
            {
                index = UnityEngine.Random.Range(0, tileByType[placementType].Count);
                position = tileByType[placementType].ElementAt(index);
            }
            else
            {
                index = UnityEngine.Random.Range(0, validNodeFloorPositions.Count);
                position = validNodeFloorPositions.ElementAt(index);
            }

            if (itemArea > 1)
            {
                var (result, placementPositions) = PlaceBigItem(position, size, addOffset);

                if (result == false)
                    continue;
                
                validNodeFloorPositions.ExceptWith(placementPositions);
                tileByType[placementType].ExceptWith(placementPositions);
            }
            else
            {
                validNodeFloorPositions.Remove(position);
                tileByType[placementType].Remove(position);
            }

            return position;
        }
        return null;
    }



    private (bool, List<Vector2Int>) PlaceBigItem(          // Placed with originPosition as bottom left tile
        Vector2Int originPosition,
        Vector2Int size,
        bool addOffset)
    {
        List<Vector2Int> positions = new List<Vector2Int>() { originPosition };
        int maxX = addOffset ? size.x + 1 : size.x;
        int maxY = addOffset ? size.y + 1 : size.y;
        int minX = addOffset ? -1 : 0;
        int minY = addOffset ? -1 : 0;

        for (int row = minX; row <= maxX; row++)
        {
            for (int col = minY; col <= maxY; col++)
            {
                if (col == 0 && row == 0)
                    continue;
                Vector2Int newPosToCheck = new Vector2Int(originPosition.x + row, originPosition.y + col);
                if (!validNodeFloorPositions.Contains(newPosToCheck))
                    return (false, positions);
                positions.Add(newPosToCheck);
            }
        }
        return (true, positions);
    }



    private Dictionary<PlacementType, HashSet<Vector2Int>> SetupDictionary()
    {
        NeighbourPositions neighbourPositions = new NeighbourPositions(validNodeFloorPositions);
        Dictionary<PlacementType, HashSet<Vector2Int>> dictionary = new Dictionary<PlacementType, HashSet<Vector2Int>>();
        foreach (var position in validNodeFloorPositions)
        {
            int neighboursCount8Dir = neighbourPositions.Get8DirectionNeighbours(position).Count;

            // Determining the type of item that can be placed on that tile
            PlacementType type;
            if (neighboursCount8Dir == 8)
                type = PlacementType.ColliderActive;
            else
                type = PlacementType.Anywhere;
            
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
    ColliderActive      // Requires surrounding areas to be traversable
}