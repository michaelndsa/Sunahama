using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MazeGenerator_Tilemap : MonoBehaviour
{
    public int width = 21;
    public int height = 21;
    public Tilemap tilemap;
    public Tile wallTile;
    public Tile floorTile;

    private int[,] maze;

    void Start()
    {
        GenerateMaze();
        DrawMaze();
        CenterTilemap();
    }

    void GenerateMaze()
    {
        if (width % 2 == 0)
            width += 1;
        if (height % 2 == 0)
            height += 1;

        maze = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 1;

        Carve(1, 1);
    }

    void Carve(int x, int y)
    {
        maze[x, y] = 0;

        var dirs = new List<Vector2Int> {
            Vector2Int.up * 2,
            Vector2Int.down * 2,
            Vector2Int.left * 2,
            Vector2Int.right * 2
        };
        Shuffle(dirs);

        foreach (var dir in dirs)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if (nx > 0 && ny > 0 && nx < width - 1 && ny < height - 1 && maze[nx, ny] == 1)
            {
                maze[x + dir.x / 2, y + dir.y / 2] = 0;
                Carve(nx, ny);
            }
        }
    }

    void DrawMaze()
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (maze[x, y] == 1)
                    tilemap.SetTile(pos, wallTile);
                else
                    tilemap.SetTile(pos, floorTile);
            }
        }
    }

    void CenterTilemap()
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int centerCell = new Vector3Int(
            Mathf.FloorToInt(bounds.center.x),
            Mathf.FloorToInt(bounds.center.y),
            Mathf.FloorToInt(bounds.center.z)
        );
        Vector3 centerWorld = tilemap.CellToWorld(centerCell);
        tilemap.transform.position = -centerWorld;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}