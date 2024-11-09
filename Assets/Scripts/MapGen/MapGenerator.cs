// src: https://www.youtube.com/watch?v=v7yyZZjF1z4

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour // mudar tudo isso dps
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] [Range(0, 100)] private int fillPercentage;

    private MeshGenerator meshGenerator;

    private int seed = -1;
    public int Seed { get { return this.seed; } }

    private int[,] mapWallData;

    private void Start()
    {
        this.meshGenerator = GetComponent<MeshGenerator>();
    }

    public void SetSeed(int seed)
    {
        if (this.seed != -1) return;
        this.seed = seed;
    }

    private int GetVizinhosWallCount(int x, int y)
    {
        int count = 0;
        for (int vizinX = x - 1; vizinX <= x + 1; vizinX++)
        {
            for (int vizinY = y - 1; vizinY <= y + 1; vizinY++)
            {
                if ((vizinX < 0 || vizinX > this.width - 1 || vizinY < 0 || vizinY > this.height - 1))
                {
                    count++;
                    continue;
                }
                if ((vizinX == x && vizinY == y)) continue;
                count += mapWallData[vizinX, vizinY];
            }
        }
        return count;
    }

    private void SmoothMap()
    {
        for (int xIndex = 0; xIndex < this.width; xIndex++)
        {
            for (int yIndex = 0; yIndex < this.height; yIndex++)
            {
                int vizinWallTiles = this.GetVizinhosWallCount(xIndex, yIndex);

                // regra arbitrarias
                if (vizinWallTiles == 4) continue;
                if (vizinWallTiles > 4)
                {
                    this.mapWallData[xIndex, yIndex] = 1;
                    continue;
                }
                this.mapWallData[xIndex, yIndex] = 0;
            }
        }
    }

    public void GenerateMap()
    {
        if (this.seed == -1) { Debug.Log("deu kakinha"); return; }

        // Init
        this.mapWallData = new int[this.width, this.height];
        
        // RandomFillMap
        Random.InitState(this.seed);
        for (int xIndex = 0; xIndex < this.width; xIndex++)
        {
            for (int yIndex = 0; yIndex < this.height; yIndex++)
            {
                if (xIndex == 0 || xIndex == this.width - 1 || yIndex == 0 || yIndex == this.height - 1)
                {
                    this.mapWallData[xIndex, yIndex] = 1;
                    continue;
                }

                this.mapWallData[xIndex, yIndex] = (Random.Range(0f, 100f) < this.fillPercentage) ? 1 : 0;
            }
        }

        // Smoothing
        for (int index = 0; index < 5; index++) { this.SmoothMap(); }

        // Boarder
        int boarderSize = 5;
        int[,] boardedMap = new int[this.width + boarderSize * 2, this.height + boarderSize * 2];

        int boardedMapXIndex = boardedMap.GetLength(0);
        int boardedMapYIndex = boardedMap.GetLength(1);

        for (int xIndex = 0; xIndex < boardedMapXIndex; xIndex++)
        {
            for (int yIndex = 0; yIndex < boardedMapYIndex; yIndex++)
            {
                if ((xIndex > (boardedMapXIndex / 2) - 10 && xIndex < (boardedMapXIndex / 2) + 10) && (yIndex > (boardedMapYIndex / 2) - 7 && yIndex < (boardedMapYIndex / 2) + 7))
                {
                    boardedMap[xIndex, yIndex] = 0;
                    continue;
                }

                if (xIndex >= boarderSize && xIndex < this.width + boarderSize && yIndex >= boarderSize && yIndex < this.height + boarderSize)
                {
                    boardedMap[xIndex, yIndex] = this.mapWallData[xIndex - boarderSize, yIndex - boarderSize];
                    continue;
                }

                boardedMap[xIndex, yIndex] = 1;
            }
        }

        // MeshGen
        this.meshGenerator.GenerateMesh(boardedMap, 1);
    }
}
