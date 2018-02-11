using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public int Width;
    public int Height;
    public string Seed;
    public bool UseRandomSeed;

    [Range(0, 100)]
    public int RandomFillPercent;

    int[,] Map;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        Map = new int[Width, Height];
        RandomFillMap();

        for(int i = 0; i < 5; i++)
        {
            SmoothMap();
        }
    }

    void RandomFillMap()
    {
        if(UseRandomSeed)
        {
            Seed = Time.time.ToString();
        }

        System.Random PseudoRandom = new System.Random(System.DateTime.Now.GetHashCode());

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if(x == 0 || x == Width-1 || y == 0 || y == Height-1)
                {
                    Map[x, y] = 1;
                }
                else
                {
                    Map[x, y] = (PseudoRandom.Next(0, 100) < RandomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int NeighbourWallTiles = GetSurroundingWallCount(x, y);

                if (NeighbourWallTiles > 4)
                    Map[x, y] = 1;
                else if (NeighbourWallTiles < 4)
                    Map[x, y] = 0;
            }
        }
    }

    int GetSurroundingWallCount(int GridX, int GridY)
    {
        int WallCount = 0;
        for (int NeighbourX = GridX - 1; NeighbourX <= GridX + 1; NeighbourX++)
        {
            for (int NeighbourY = GridY - 1; NeighbourY <= GridY + 1; NeighbourY++)
            {
                if (NeighbourX >= 0 && NeighbourX < Width && NeighbourY >= 0 && NeighbourY < Height)
                {
                    if (NeighbourX != GridX || NeighbourY != GridY)
                    {
                        WallCount += Map[NeighbourX, NeighbourY];
                    }
                }
                else
                {
                    WallCount++;
                }
            }
        }
        return WallCount;
    }

    void OnDrawGizmos()
    {
        if (Map != null)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Gizmos.color = (Map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-Width / 2 + x + 0.5f, 0, -Height / 2 + y + 0.5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}
