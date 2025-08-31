using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawn : MonoBehaviour
{
    public void MoveToMaze(MazeGenerator_Tilemap mazeGenerator)
    {
        int[,] maze = mazeGenerator.GetMaze();
        List<Vector2Int> freeTiles = new List<Vector2Int>();

        for (int x = 1; x < mazeGenerator.Width - 1; x++)
        {
            for (int y = 1; y < mazeGenerator.Height - 1; y++)
            {
                if (maze[x, y] == 0) // 可走路
                    freeTiles.Add(new Vector2Int(x, y));
            }
        }

        if (freeTiles.Count == 0)
        {
            Debug.LogError("迷宮裡沒有可用的位置！");
            return;
        }

        // 隨機選一個可走路的位置
        Vector2Int pos = freeTiles[Random.Range(0, freeTiles.Count)];

        // ✅ 直接移動玩家位置（不是生成新的）
        transform.position = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
    }
}