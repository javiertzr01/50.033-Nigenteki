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
    private TileBase floorTile, wallTop, biomeOne, biomeTwo, biomeThree, biomeFour, biomeFive, path;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    // Take in floorposition and biomes array for Client side
    public void PaintBiomeTiles(Vector2Int[] floorPositions, Sprites[] biomes)
    {
        TileBase tile;
        for (int i = 0; i < floorPositions.Length; i++)
        {
            switch(biomes[i])
            {
                case Sprites.One:
                    tile = biomeOne;
                    break;
                case Sprites.Two:
                    tile = biomeTwo;
                    break;
                case Sprites.Three:
                    tile = biomeThree;
                    break;
                case Sprites.Four:
                    tile = biomeFour;
                    break;
                case Sprites.Five:
                    tile = biomeFive;
                    break;
                case Sprites.Path:
                    tile = path;
                    break;
                default:
                    tile = floorTile;
                    break;
            }

            PaintSingleTile(floorTilemap, tile, floorPositions[i]);
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
