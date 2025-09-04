using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator
{
    private int width;
    private int height;
    private int[,] maze;

    public MazeGenerator(int width, int height)
    {
        // 確保奇數
        if (width % 2 == 0) width++;
        if (height % 2 == 0) height++;

        this.width = width;
        this.height = height;
        maze = new int[width, height];
    }

    public int[,] Generate()
    {
        // 初始化全部牆壁
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 1;

        Carve(1, 1);
        return maze;
    }

    private void Carve(int x, int y)
    {
        maze[x, y] = 0;

        List<Vector2Int> dirs = new List<Vector2Int> {
            Vector2Int.up * 2,
            Vector2Int.down * 2,
            Vector2Int.left * 2,
            Vector2Int.right * 2
        };
        Shuffle(dirs);

        foreach (Vector2Int dir in dirs)
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

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}