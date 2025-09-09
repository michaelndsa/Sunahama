using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MazeRenderer : MonoBehaviour
{
    [Header("�g�c�ͦ��Ѧ�")]
    public MazeGenerator generator;

    [Header("Tilemap �Ѧ�")]
    public Tilemap Tilemap;

    [Header("��� Tile & �v��")]
    public Tile[] WallTiles;
    public int[] WallWeights;

    [Header("�a�O Tile & �v��")]
    public Tile[] FloorTiles;
    public int[] FloorWeights;

    [Header("�X�f Tile")]
    public Tile ExitTile;

    [Header("���a�Ѧ�")]
    public PlayerSpawn player;

    public bool mazeGenerated = false;

    private int[,] maze;

    void Start()
    {
        if (generator == null)
        {
            Debug.LogError("�Цb Inspector ���w MazeGenerator�I");
            return;
        }

        // �ͦ��g�c���
        maze = generator.Generate();

        // �e�g�c
        DrawMaze();

        // �ͦ��X�f
        GenerateExit();

        mazeGenerated = true;

        if (player != null)
            player.MoveToMaze(this);
    }

    public void RegenerateMaze()
    {
        // ���s�ͦ��g�c���
        maze = generator.Generate();

        // �M�� Tilemap
        Tilemap.ClearAllTiles();

        // ���e�g�c
        DrawMaze();

        // ���s���ͥX�f
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

        // �W�U���
        for (int x = 1; x < width - 1; x += 2)
        {
            if (maze[x, 1] == 0) candidates.Add(new Vector2Int(x, 0));
            if (maze[x, height - 2] == 0) candidates.Add(new Vector2Int(x, height - 1));
        }

        // ���k���
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
                    // �����S���I�����a�O tile
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
