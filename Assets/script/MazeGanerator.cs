using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MazeGenerator_Tilemap : MonoBehaviour
{
    public int Width = 21;
    public int Height = 21;
    public Tilemap Tilemap;
    public Tile WallTile;
    public Tile FloorTile;
    public Tile ExitTile;
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
        if (Width % 2 == 0)
            Width += 1;
        if (Height % 2 == 0)
            Height += 1;

        maze = new int[Width, Height];

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
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

            if (nx > 0 && ny > 0 && nx < Width - 1 && ny < Height - 1 && maze[nx, ny] == 1)
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
        for (int x = 1; x < Width - 1; x += 2)
        {
            if (maze[x, 1] == 0)
            {
                candidates.Add(new Vector2Int(x, 0)); // 下邊
            }
            if (maze[x, Height - 2] == 0)
            {
                candidates.Add(new Vector2Int(x, Height - 1)); // 上邊
            }
        }

        // 左右邊界
        for (int y = 1; y < Height - 1; y += 2)
        {
            if (maze[1, y] == 0)
            {
                candidates.Add(new Vector2Int(0, y)); // 左邊
            }
            if (maze[Width - 2, y] == 0)
            {
                candidates.Add(new Vector2Int(Width - 1, y)); // 右邊
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

        if (ExitTile != null)
        {
            Tilemap.SetTile(pos, ExitTile);
        }
        else
        {
            Tilemap.SetTile(pos, FloorTile);
        }
    }

    void DrawMaze()
    {
        Tilemap.ClearAllTiles();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (maze[x, y] == 1)
                    Tilemap.SetTile(pos, WallTile);
                else
                    Tilemap.SetTile(pos, FloorTile);
            }
        }
    }

    void CenterTilemap()
    {
        BoundsInt bounds = Tilemap.cellBounds;
        Vector3Int centerCell = new Vector3Int(
            Mathf.FloorToInt(bounds.center.x),
            Mathf.FloorToInt(bounds.center.y),
            Mathf.FloorToInt(bounds.center.z)
        );
        Vector3 centerWorld = Tilemap.CellToWorld(centerCell);
        Tilemap.transform.position = -centerWorld;
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