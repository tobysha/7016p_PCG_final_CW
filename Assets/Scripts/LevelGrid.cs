using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public int gridSizeX = 50;
    public int gridSizeY = 50;
    public float cellSize = 1f;
    public int randomFillPercent = 35;
    public int smoothIterations = 50;
    public int borderSize = 4;
    public int wallThresholdSize = 50;
    public int roomThresholdSize = 50;

    public int[,] initialGrid;
    private int[,] finalLevel;

    private AgentLocationGenerator ag;
    void Awake()
    {
        
    }
    private void Start()
    {
        ag = GetComponent<AgentLocationGenerator>();
    }
    void builtGrid()
    {
        GenerateInitialGrid();
        SmoothInitialGrid();
        ProcessMap();
        PostProcessGrid();
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(finalLevel, 1);
        ag.AgentsGenerate();
    }
    void GenerateInitialGrid()
    {
        initialGrid = new int[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (x == 0 || x == gridSizeX - 1 || y == 0 || y == gridSizeY - 1)
                {
                    initialGrid[x, y] = 1; 
                }
                else
                {
                    float distanceToCenter = Vector2.Distance(new Vector2(x, y), new Vector2(gridSizeX / 2, gridSizeY / 2));
                    if (distanceToCenter < gridSizeX / 8) 
                    {
                        initialGrid[x, y] = 1;
                    }
                    else
                    {
                        initialGrid[x, y] = Random.Range(0, 100) < randomFillPercent ? 1 : 0;
                    }
                }
            }
        }
    }

    void SmoothInitialGrid()
    {
        for (int i = 0; i < smoothIterations; i++)
        {
            for (int x = 1; x < gridSizeX - 1; x++)
            {
                for (int y = 1; y < gridSizeY - 1; y++)
                {
                    int surroundingWalls = GetSurroundingWallCount(x, y);
                    if (surroundingWalls > 4)
                    {
                        initialGrid[x, y] = 1;
                    }
                    else if (surroundingWalls < 4)
                    {
                        initialGrid[x, y] = 0;
                    }
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < gridSizeX && neighborY >= 0 && neighborY < gridSizeY)
                {
                    if (neighborX != gridX || neighborY != gridY)
                    {
                        wallCount += initialGrid[neighborX, neighborY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    void PostProcessGrid()
    {
        
        finalLevel = new int[gridSizeX + borderSize * 2, gridSizeY + borderSize * 2];
        for (int x = 0; x < finalLevel.GetLength(0); x++)
        {
            for (int y = 0; y < finalLevel.GetLength(1); y++)
            {
                if (x >= borderSize && x < gridSizeX + borderSize && y >= borderSize && y < gridSizeY + borderSize)
                {
                    finalLevel[x, y] = initialGrid[x - borderSize, y - borderSize];
                }
                else
                {
                    finalLevel[x, y] = 1;
                }
            }
        }
    }
    bool isInMapRange(int x, int y)
    {
        return (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY);
    }
    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);
        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    initialGrid[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);
        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    initialGrid[tile.tileX, tile.tileY] = 1;
                }
            }
        }
    }
    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (mapFlags[x, y] == 0 && initialGrid[x, y] == tileType)
                {
                    List<Coord> newRegion = getRegion(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }
    List<Coord> getRegion(int startX, int startY)
    {
        List<Coord> region = new List<Coord>();
        int[,] mapflags = new int[gridSizeX, gridSizeY];
        int tileType = initialGrid[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapflags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord coord = queue.Dequeue();
            region.Add(coord);
            for (int x = coord.tileX - 1; x <= coord.tileX + 1; x++)
            {
                for (int y = coord.tileY - 1; y <= coord.tileY + 1; y++)
                {
                    if(isInMapRange(x,y) && (x==coord.tileX||y==coord.tileY))
                    {
                        if (mapflags[x, y] == 0 && initialGrid[x, y] == tileType)
                        {
                            mapflags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return region;
    }
    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }
}
