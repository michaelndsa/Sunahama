using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MendamaSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject menDamaPrefeb;
    public int spawnCount = 1;
    public MazeRenderer MG;
    public MendamaCollectUI uiManager;
    
    private int[,] maze;

    IEnumerator Start()
    {
        while (!MG.mazeGenerated)
            yield return null;
        maze = MG.GetMaze();
        SpawnMendamas();
        Debug.Log("Maze size: " + maze.GetLength(0) + " x " + maze.GetLength(1));
    }

    void SpawnMendamas()
    { 
        List<Vector2Int> floorPositions = new List<Vector2Int>();

        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
            {
                if (maze[x, y] == 0)
                {
                    floorPositions.Add(new Vector2Int(x, y));
                }
            }
        }
        Shuffle(floorPositions);

        for (int i = 0; i < Mathf.Min(spawnCount, floorPositions.Count); i++)
        {
            Vector2Int pos = floorPositions[i];
            Vector3 worldPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
            Instantiate(menDamaPrefeb, worldPos, Quaternion.identity);
        }
        uiManager.SetTotalCount(spawnCount);

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
