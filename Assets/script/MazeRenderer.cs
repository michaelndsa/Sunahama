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

    // === 成員變數化 ===
    private int[,] maze;
    private List<Vector2Int> exitCandidates = new List<Vector2Int>();
    private Vector2Int exitPos;
    private Vector3Int exitWorldPos;
    private int width;
    private int height;
    private int totalWeight;
    private int randomValue;

    void Start()
    {
        if (generator == null)
        {
            Debug.LogError("請在 Inspector 指定 MazeGenerator！");
            return;
        }

        maze = generator.Generate();
        width = maze.GetLength(0);
        height = maze.GetLength(1);

        DrawMaze();
        GenerateExit();

        mazeGenerated = true;

        if (player != null)
            player.MoveToMaze(this);
    }

    public void RegenerateMaze()
    {
        maze = generator.Generate();
        width = maze.GetLength(0);
        height = maze.GetLength(1);

        Tilemap.ClearAllTiles();
        DrawMaze();
        GenerateExit();
    }

    void DrawMaze()
    {
        Tilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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
        exitCandidates.Clear();

        // 上下邊界
        for (int x = 1; x < width - 1; x += 2)
        {
            if (maze[x, 1] == 0) exitCandidates.Add(new Vector2Int(x, 0));
            if (maze[x, height - 2] == 0) exitCandidates.Add(new Vector2Int(x, height - 1));
        }

        // 左右邊界
        for (int y = 1; y < height - 1; y += 2)
        {
            if (maze[1, y] == 0) exitCandidates.Add(new Vector2Int(0, y));
            if (maze[width - 2, y] == 0) exitCandidates.Add(new Vector2Int(width - 1, y));
        }

        if (exitCandidates.Count == 0) return;

        randomValue = Random.Range(0, exitCandidates.Count);
        exitPos = exitCandidates[randomValue];
        maze[exitPos.x, exitPos.y] = 0;

        exitWorldPos = new Vector3Int(exitPos.x, exitPos.y, 0);
        if (ExitTile != null)
            Tilemap.SetTile(exitWorldPos, ExitTile);
        else
            Tilemap.SetTile(exitWorldPos, GetRandomTile(FloorTiles, FloorWeights));
    }

    public void OpenExit()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Tilemap.GetTile(new Vector3Int(x, y, 0)) == ExitTile)
                {
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

        totalWeight = 0;
        for (int i = 0; i < weights.Length; i++)
            totalWeight += weights[i];

        randomValue = Random.Range(0, totalWeight);

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

    // 額外提供出口位置 (方便其他系統排除)
    public Vector2Int GetExitPosition()
    {
        return exitPos;
    }
}
