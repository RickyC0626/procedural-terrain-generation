﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh };
    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMap()
    {
        // Generate the noise map with the necessary parameters
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);

        // Create an array to hold all the colors on the map
        Color[] colorMap = new Color[mapWidth * mapHeight];

        // Iterate through each coordinate on the noiseMap
        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                // Get the height value at the coordinates
                float currentHeight = noiseMap[x, y];

                // Iterate through the regions to find which color corresponds to the height
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        // Converting the index of two-dimensional array to one-dimensional
                        // y * width will give us the row, and adding x will give us the column
                        
                        // Assign the region color associated with the height
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        
        if(drawMode == DrawMode.NoiseMap) display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if(drawMode == DrawMode.ColorMap) display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        else if(drawMode == DrawMode.Mesh) display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
    }

    // Called whenever a public variable is modified in the inspector
    void OnValidate()
    {
        if(mapWidth < 1) mapWidth = 1;
        if(mapHeight < 1) mapHeight = 1;
        if(octaves < 0) octaves = 0;
        if(lacunarity < 1) lacunarity = 1;
    }
}

// Make serializable to show in inspector
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
