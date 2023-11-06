using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private TileBase floorTile, wallTop, biomeOne, biomeTwo, biomeThree, biomeFour, biomeFive;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    // Take in floorposition and biomes array
    public void PaintBiomeTiles(Dictionary<Vector2Int, HashSet<Vector2Int>> zones, Dictionary<Vector2Int, ProceduralGenerationAlgorithms.RTTNode> nodeDictionary)
    {
        TileBase tile;
        foreach (var kvp in zones)
        {
            ProceduralGenerationAlgorithms.RTTNode node = nodeDictionary[kvp.Key];
            switch(node.biome)
            {
                case Biomes.One:
                    tile = biomeOne;
                    break;
                case Biomes.Two:
                    tile = biomeTwo;
                    break;
                case Biomes.Three:
                    tile = biomeThree;
                    break;
                case Biomes.Four:
                    tile = biomeFour;
                    break;
                case Biomes.Five:
                    tile = biomeFive;
                    break;
                default:
                    tile = floorTile;
                    break;
            }
            foreach(var position in kvp.Value)
            {
                PaintSingleTile(floorTilemap, tile, position);
            }
        }
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach(var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, wallTop, position);
    }
}
