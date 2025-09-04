using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MazeRenderer : MonoBehaviour
{
    [Header("迷宮大小")]
    public int Width = 21;
    public int Height = 21;

    [Header("Tilemap 參考")]
    public Tilemap Tilemap;

    [Header("牆壁 Tile & 權重")]
    public Tile[] WallTiles;       // 牆 Tile 陣列
    public int[] WallWeights;      // 權重，對應 WallTiles 索引

    [Header("地板 Tile & 權重")]
    public Tile[] FloorTiles;      // 地板 Tile 陣列
    public int[] FloorWeights;     // 權重，對應 FloorTiles 索引

    [Header("出口 Tile")]
    public Tile ExitTile;

    [Header("玩家參考")]
    public PlayerSpawn player;

    public bool mazeGenerated = false;

    private int[,] maze;

    void Start()
    {
        // 生成迷宮數據
        MazeGenerator generator = new MazeGenerator(Width, Height);
        maze = generator.Generate();

        // 畫迷宮
        DrawMaze();

        // 生成出口
        GenerateExit();

        mazeGenerated = true;

        if (player != null)
            player.MoveToMaze(this);
    }

    void DrawMaze()
    {
        Tilemap.ClearAllTiles();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (maze[x, y] == 1) // 牆
                    Tilemap.SetTile(pos, GetRandomTile(WallTiles, WallWeights));
                else // 地板
                    Tilemap.SetTile(pos, GetRandomTile(FloorTiles, FloorWeights));
            }
        }
    }

    void GenerateExit()
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        // 上下邊界
        for (int x = 1; x < Width - 1; x += 2)
        {
            if (maze[x, 1] == 0) candidates.Add(new Vector2Int(x, 0));
            if (maze[x, Height - 2] == 0) candidates.Add(new Vector2Int(x, Height - 1));
        }

        // 左右邊界
        for (int y = 1; y < Height - 1; y += 2)
        {
            if (maze[1, y] == 0) candidates.Add(new Vector2Int(0, y));
            if (maze[Width - 2, y] == 0) candidates.Add(new Vector2Int(Width - 1, y));
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
