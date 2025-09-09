using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MazeRenderer : MonoBehaviour
{
    [Header("迷宮生成參考")]
    public MazeGenerator generator;

    [Header("Tilemap 參考")]
    public Tilemap Tilemap;

    [Header("牆壁 Tile & 權重")]
    public Tile[] WallTiles;
    public int[] WallWeights;

    [Header("地板 Tile & 權重")]
    public Tile[] FloorTiles;
    public int[] FloorWeights;

    [Header("出口 Tile")]
    public Tile ExitTile;

    [Header("玩家參考")]
    public PlayerSpawn player;

    public bool mazeGenerated = false;

    private int[,] maze;

    void Start()
    {
        if (generator == null)
        {
            Debug.LogError("請在 Inspector 指定 MazeGenerator！");
            return;
        }

        // 生成迷宮資料
        maze = generator.Generate();

        // 畫迷宮
        DrawMaze();

        // 生成出口
        GenerateExit();

        mazeGenerated = true;

        if (player != null)
            player.MoveToMaze(this);
    }

    public void RegenerateMaze()
    {
        // 重新生成迷宮資料
        maze = generator.Generate();

        // 清空 Tilemap
        Tilemap.ClearAllTiles();

        // 重畫迷宮
        DrawMaze();

        // 重新產生出口
        GenerateExit();
    }

    void DrawMaze()
    {
        Tilemap.ClearAllTiles();

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (maze[x, y] == 1)
                    Tilemap.SetTile(pos, GetRandomTile(WallTiles, WallWeights));
                else
                    Tilemap.SetTile(pos, GetRandomTile(FloorTiles, FloorWeights));
            }
        }
    }

    void GenerateExit()
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        // 上下邊界
        for (int x = 1; x < width - 1; x += 2)
        {
            if (maze[x, 1] == 0) candidates.Add(new Vector2Int(x, 0));
            if (maze[x, height - 2] == 0) candidates.Add(new Vector2Int(x, height - 1));
        }

        // 左右邊界
        for (int y = 1; y < height - 1; y += 2)
        {
            if (maze[1, y] == 0) candidates.Add(new Vector2Int(0, y));
            if (maze[width - 2, y] == 0) candidates.Add(new Vector2Int(width - 1, y));
        }

        if (candidates.Count == 0) return;

        int index = Random.Range(0, candidates.Count);
        Vector2Int exitPos = candidates[index];
        maze[exitPos.x, exitPos.y] = 0;

        Vector3Int pos = new Vector3Int(exitPos.x, exitPos.y, 0);
        if (ExitTile != null)
            Tilemap.SetTile(pos, ExitTile);
        else
            Tilemap.SetTile(pos, GetRandomTile(FloorTiles, FloorWeights));
    }

    public void OpenExit()
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Tilemap.GetTile(new Vector3Int(x, y, 0)) == ExitTile)
                {
                    // 換成沒有碰撞的地板 tile
                    Tilemap.SetTile(new Vector3Int(x, y, 0), GetRandomTile(FloorTiles, FloorWeights));
                    return;
                }
            }
        }
    }
    Tile GetRandomTile(Tile[] tiles, int[] weights)
    {
        if (tiles == null || tiles.Length == 0 || weights == null || tiles.Length != weights.Length)
            return null;

        int totalWeight = 0;
        for (int i = 0; i < weights.Length; i++)
            totalWeight += weights[i];

        int randomValue = Random.Range(0, totalWeight);

        for (int i = 0; i < tiles.Length; i++)
        {
            if (randomValue < weights[i])
                return tiles[i];
            randomValue -= weights[i];
        }

        return tiles[0];
    }

    public int[,] GetMaze()
    {
        return maze;
    }
}
