using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class AgentLocationGenerator : MonoBehaviour
{
    private LevelGrid grid;
    private List<GameObject> gameObjects;

    public GameObject Diamond;
    public GameObject Thief;
    public GameObject Troll;
    public int DiamondNumber = 4; 
    public int ThiefNumber = 1;
    public int TrollNumber = 1;
    public float spawnRadius = 10f; 
    public LayerMask detectionLayer; 

    void Start()
    {
        gameObjects = new List<GameObject>();
    }
    public void AgentsGenerate()
    {
        foreach (GameObject go in gameObjects)
        {
            if (go != null)
            {
                Destroy(go);
            }
        }
        gameObjects.Clear();
        grid = GetComponent<LevelGrid>();
        // 生成四个区域的代理
        for (int i = 0; i < 4; i++)
        {
            //diamondscreate
            for (int j = 0; j < DiamondNumber; j++)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition(i);
                GameObject diamond = Instantiate(Diamond, spawnPosition, Quaternion.identity);
                gameObjects.Add(diamond);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            
            for (int j = 0; j < ThiefNumber; j++)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition(i);
                GameObject thief = Instantiate(Thief, spawnPosition, Quaternion.identity);
                gameObjects.Add(thief);
            }

        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < TrollNumber; j++)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition(i);
                GameObject troll = Instantiate(Troll, spawnPosition, Quaternion.identity);
                gameObjects.Add(troll);
            }
        }

    }


    Vector3 GetRandomSpawnPosition(int regionIndex)
    {
        
        Vector3 spawnPosition = Vector3.zero;
        float x = grid.gridSizeX/2;
        float y = grid.gridSizeY/2;
        switch (regionIndex)
        {
            case 0: // lefttop
                spawnPosition = new Vector3(Random.Range(-x, 0f), Random.Range(0f, y), 0f);
                if (ScanEnvironment(spawnPosition))
                {
                    spawnPosition = GetRandomSpawnPosition(0);
                }
                Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(spawnPosition, spawnRadius, detectionLayer);
                foreach (Collider2D nearbyObject in nearbyObjects)
                {
                    if (nearbyObject.CompareTag("Troll") && nearbyObject.CompareTag("Thief") && nearbyObject.CompareTag("Diamond"))
                    {
                        spawnPosition=GetRandomSpawnPosition(0);
                    }
                }
                break;
            case 1: // right top
                spawnPosition = new Vector3(Random.Range(0f, x), Random.Range(0f, y), 0f);
                if (ScanEnvironment(spawnPosition))
                {
                    spawnPosition = GetRandomSpawnPosition(1);
                }
                Collider2D[] nearbyObjects1 = Physics2D.OverlapCircleAll(spawnPosition, spawnRadius, detectionLayer);
                foreach (Collider2D nearbyObject in nearbyObjects1)
                {
                    if (nearbyObject.CompareTag("Troll") && nearbyObject.CompareTag("Thief") && nearbyObject.CompareTag("Diamond"))
                    {
                        spawnPosition = GetRandomSpawnPosition(1);
                    }
                }
                break;
            case 2: // left bottom
                spawnPosition = new Vector3(Random.Range(-x, 0f), Random.Range(-y, 0f), 0f);
                if (ScanEnvironment(spawnPosition))
                {
                    spawnPosition = GetRandomSpawnPosition(2);
                }
                Collider2D[] nearbyObjects2 = Physics2D.OverlapCircleAll(spawnPosition, spawnRadius, detectionLayer);
                foreach (Collider2D nearbyObject in nearbyObjects2)
                {
                    if (nearbyObject.CompareTag("Troll") && nearbyObject.CompareTag("Thief") && nearbyObject.CompareTag("Diamond"))
                    {
                        spawnPosition = GetRandomSpawnPosition(2);
                    }
                }
                break;
            case 3: // right bottom
                spawnPosition = new Vector3(Random.Range(0f, x), Random.Range(-y, 0f), 0f);
                if (ScanEnvironment(spawnPosition))
                {
                    spawnPosition = GetRandomSpawnPosition(3);
                }
                Collider2D[] nearbyObjects3 = Physics2D.OverlapCircleAll(spawnPosition, spawnRadius, detectionLayer);
                foreach (Collider2D nearbyObject in nearbyObjects3)
                {
                    if (nearbyObject.CompareTag("Troll") && nearbyObject.CompareTag("Thief") && nearbyObject.CompareTag("Diamond"))
                    {
                        spawnPosition = GetRandomSpawnPosition(3);
                    }
                }
                break;
        }

        return spawnPosition;
    }
    bool ScanEnvironment(Vector3 Pos)
    {
        for(int i = -1;i<2;i++)
        {
            for(int j = -1; j<2;j++)
            {
                if (grid.initialGrid[(int)(Pos.x+grid.gridSizeX/2+i),(int)(Pos.y+grid.gridSizeY/2+j)]==1)
                {
                    return true;
                }
            }
        }
        //Debug.Log("gezi:"+ grid.initialGrid[(int)(Pos.x + grid.gridSizeX / 2 ), (int)(Pos.y + grid.gridSizeY / 2 )]);
        //Debug.Log("x:" + (int)(Pos.x + grid.gridSizeX / 2) + "    y:" + (int)(Pos.y + grid.gridSizeY / 2));
        return false;
    }
   
}
