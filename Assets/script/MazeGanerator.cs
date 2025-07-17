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
    public Tile exitTile;
    public bool IsMazeReady() => maze != null;
    private int[,] maze;

    void Start()
    {
        GenerateMaze();
        DrawMaze();
        GenerateExit();
        GetMaze();
        //CenterTilemap();
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
    public int[,] GetMaze()
    {
        return maze;
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

    void GenerateExit()
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        // 上下邊界
        for (int x = 1; x < width - 1; x += 2)
        {
            if (maze[x, 1] == 0)
            {
                candidates.Add(new Vector2Int(x, 0)); // 下邊
            }
            if (maze[x, height - 2] == 0)
            {
                candidates.Add(new Vector2Int(x, height - 1)); // 上邊
            }
        }

        // 左右邊界
        for (int y = 1; y < height - 1; y += 2)
        {
            if (maze[1, y] == 0)
            {
                candidates.Add(new Vector2Int(0, y)); // 左邊
            }
            if (maze[width - 2, y] == 0)
            {
                candidates.Add(new Vector2Int(width - 1, y)); // 右邊
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("出口候補が見つかりませんでした");
            return;
        }

        // 隨機選一個候選點
        int index = Random.Range(0, candidates.Count);
        Vector2Int exitPos = candidates[index];

        maze[exitPos.x, exitPos.y] = 0;

        Vector3Int pos = new Vector3Int(exitPos.x, exitPos.y, 0);

        if (exitTile != null)
        {
            tilemap.SetTile(pos, exitTile);
        }
        else
        {
            tilemap.SetTile(pos, floorTile);
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