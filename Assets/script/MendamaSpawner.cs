using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MendamaSpawner : MonoBehaviour
{
    public GameObject menDamaPrefeb;
    public int spawnCount = 1;
    public MazeRenderer MG;
    public MendamaCollectUI uiManager;

    public int[,] maze;

    private List<GameObject> spawnedMendamas = new List<GameObject>();

    IEnumerator Start()
    {
        while (!MG.mazeGenerated)
            yield return null;

        maze = MG.GetMaze();
        SpawnMendamas();
        Debug.Log("Maze size: " + maze.GetLength(0) + " x " + maze.GetLength(1));
    }

    public void RespawnMendamas()
    {
        // 1. 刪除舊的
        for (int i = 0; i < spawnedMendamas.Count; i++)
        {
            GameObject obj = spawnedMendamas[i];
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedMendamas.Clear();

        // 2. 更新迷宮資料
        maze = MG.GetMaze();

        // 3. 重新生成
        SpawnMendamas();
        Debug.Log("Mendamas respawned!");
    }

    void SpawnMendamas()
    {
        List<Vector2Int> floorPositions = new List<Vector2Int>();

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                if (maze[x, y] == 0)
                {
                    floorPositions.Add(new Vector2Int(x, y));
                }
            }
        }
        Shuffle(floorPositions);

        List<Vector2Int> chosenPositions = new List<Vector2Int>();

        for (int i = 0; i < floorPositions.Count; i++)
        {
            Vector2Int pos = floorPositions[i];

            bool tooClose = false;
            for (int j = 0; j < chosenPositions.Count; j++)
            {
                Vector2Int chosen = chosenPositions[j];
                if (Vector2Int.Distance(pos, chosen) <= 1.0f)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                chosenPositions.Add(pos);
            }

            if (chosenPositions.Count >= spawnCount)
                break;
        }

        int mazeHeight = maze.GetLength(1);

        for (int i = 0; i < chosenPositions.Count; i++)
        {
            Vector2Int pos = chosenPositions[i];
            Vector3 worldPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
            GameObject obj = Instantiate(menDamaPrefeb, worldPos, Quaternion.identity);

            MendamaCollectController controller = obj.GetComponent<MendamaCollectController>();
            if (pos.y == mazeHeight - 2)
            {
                controller.isTopRow = true;
            }

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = -Mathf.RoundToInt(pos.y * 100);
            }

            // 記錄下來方便清理
            spawnedMendamas.Add(obj);
        }

        uiManager.SetTotalCount(chosenPositions.Count);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}