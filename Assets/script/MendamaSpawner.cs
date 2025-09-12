using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MendamaSpawner : MonoBehaviour
{
    [Header("Prefab 與引用")]
    public GameObject menDamaPrefeb;
    public MazeRenderer MG;
    public CollectMDUI uiManager;
    public MendamaCollectUI uiCount;

    [Header("設定")]
    public int spawnCount = 1;

    [Header("迷宮資訊")]
    public int[,] maze;
    public int mazeWidth;
    public int mazeHeight;
    public Vector2Int exitPos;

    private List<GameObject> spawnedMendamas = new List<GameObject>();
    private List<Vector2Int> allFloorPositions = new List<Vector2Int>();

    IEnumerator Start()
    {
        while (!MG.mazeGenerated)
            yield return null;

        UpdateMazeData();
        SpawnMendamas();
    }

    private void UpdateMazeData()
    {
        maze = MG.GetMaze();
        mazeWidth = maze.GetLength(0);
        mazeHeight = maze.GetLength(1);

        // 直接向 MazeRenderer 拿出口
        exitPos = MG.GetExitPosition();

        CollectAllFloorPositions();
    }

    private void CollectAllFloorPositions()
    {
        allFloorPositions.Clear();
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                // 排除出口
                if (maze[x, y] == 0 && (x != exitPos.x || y != exitPos.y))
                {
                    allFloorPositions.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    public void RespawnMendamas()
    {
        // 1. 刪除舊的
        for (int i = 0; i < spawnedMendamas.Count; i++)
        {
            if (spawnedMendamas[i] != null)
                Destroy(spawnedMendamas[i]);
        }
        spawnedMendamas.Clear();

        // 2. 更新迷宮資料
        UpdateMazeData();

        // 3. 重新生成
        SpawnMendamas();
        Debug.Log("Mendamas respawned!");
    }

    private void SpawnMendamas()
    {
        List<Vector2Int> floorPositionsCopy = new List<Vector2Int>(allFloorPositions);
        Shuffle(floorPositionsCopy);

        List<Vector2Int> chosenPositions = new List<Vector2Int>();

        for (int i = 0; i < floorPositionsCopy.Count; i++)
        {
            Vector2Int pos = floorPositionsCopy[i];
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
                chosenPositions.Add(pos);

            if (chosenPositions.Count >= spawnCount)
                break;
        }

        for (int i = 0; i < chosenPositions.Count; i++)
        {
            Vector2Int pos = chosenPositions[i];
            Vector3 worldPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
            GameObject obj = Instantiate(menDamaPrefeb, worldPos, Quaternion.identity);

            MendamaCollectController controller = obj.GetComponent<MendamaCollectController>();
            if (pos.y == mazeHeight - 2)
                controller.isTopRow = true;

            spawnedMendamas.Add(obj);

            if (uiManager != null)
                uiManager.Show(obj.transform, new Vector3(100, 100, 100));

            if (uiCount != null)
                uiCount.SetTotalCount(chosenPositions.Count);
        }
    }

    public void SpawnSingleMendamaFromList()
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>(allFloorPositions);

        for (int i = 0; i < spawnedMendamas.Count; i++)
        {
            GameObject obj = spawnedMendamas[i];
            if (obj != null)
            {
                Vector2Int occupiedPos = new Vector2Int(
                    Mathf.FloorToInt(obj.transform.position.x),
                    Mathf.FloorToInt(obj.transform.position.y));
                availablePositions.Remove(occupiedPos);
            }
        }

        if (availablePositions.Count == 0)
            return;

        int randIndex = Random.Range(0, availablePositions.Count);
        Vector2Int chosenPos = availablePositions[randIndex];

        Vector3 worldPos = new Vector3(chosenPos.x + 0.5f, chosenPos.y + 0.5f, 0f);
        GameObject newObj = Instantiate(menDamaPrefeb, worldPos, Quaternion.identity);
        MendamaCollectController controller = newObj.GetComponent<MendamaCollectController>();

        if (chosenPos.y == mazeHeight - 2)
            controller.isTopRow = true;

        spawnedMendamas.Add(newObj);

        if (uiManager != null)
            uiManager.Show(newObj.transform, new Vector3(100, 100, 100));
    }

    private void Shuffle<T>(List<T> list)
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