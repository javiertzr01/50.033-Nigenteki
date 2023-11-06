using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighbourPositions
{
    private static List<Vector2Int> fourDirections = new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private static List<Vector2Int> eightDirections = new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(1,1),    // Top right
        new Vector2Int(1,-1),    // Bottom right
        new Vector2Int(-1,-1),    // Bottom left
        new Vector2Int(-1,1)    // Top left
    };

    
    
    List<Vector2Int> positions;

    
    
    public NeighbourPositions(IEnumerable<Vector2Int> allValidPositions)
    {
        this.positions = new List<Vector2Int>(allValidPositions);
    }



    public List<Vector2Int> Get4DirectionNeighbours(Vector2Int position)
    {
        return GetNeighbours(position, fourDirections);
    }



    public List<Vector2Int> Get8DirectionNeighbours(Vector2Int position)
    {
        return GetNeighbours(position, eightDirections);
    }



    private List<Vector2Int> GetNeighbours(Vector2Int position, List<Vector2Int> neighbourOffsetList)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        foreach (var direction in neighbourOffsetList)
        {
            Vector2Int potentialNeighbour = position + direction;
            if (positions.Contains(potentialNeighbour))
                neighbours.Add(potentialNeighbour);
        }
        return neighbours;
    }
}
