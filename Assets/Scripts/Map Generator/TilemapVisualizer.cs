using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : NetworkBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private TileBase floorTile, wallTop, Green1, Green2, Green3, Autumn, Sakura, path;

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
                case Sprites.Green1:
                    tile = Green1;
                    break;
                case Sprites.Green2:
                    tile = Green2;
                    break;
                case Sprites.Green3:
                    tile = Green3;
                    break;
                case Sprites.Autumn:
                    tile = Autumn;
                    break;
                case Sprites.Sakura:
                    tile = Sakura;
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

    public void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        // GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        // foreach (GameObject obj in obstacles)
        // {
        //     DestroyImmediate(obj);
        // }
    }

    internal void PaintSingleBasicWall(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, wallTop, position);
    }
}
