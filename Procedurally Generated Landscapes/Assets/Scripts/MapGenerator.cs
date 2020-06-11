using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, FallOffMap, Mesh };
    public DrawMode drawMode;

    public TerrainData terrainData;
    public NoiseData noiseData;

    [Range(0, 6)]
    public int previewLevelOfDetail;

    public bool autoUpdate;

    private float[,] fallOffMap;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    public int mapChunkSize
    {
        get
        {
            if (terrainData.useFlatShading)
            {
                return 95;
            }
            else
            {
                return 239;
            }
        }
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.heightMultiplier, terrainData.meshHeightCurve, previewLevelOfDetail, terrainData.useFlatShading));
        }else if(drawMode == DrawMode.FallOffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOffGenerator.GenerateFallOffMap(mapChunkSize)));
        }
    }

    public void RequestMapData(Vector2 _centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(_centre, callback);
        };

        new Thread(threadStart).Start();
    }

    private void MapDataThread(Vector2 _centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(_centre);
        // add the mapdata and the callback to a queue
        // make sure the queue can't be accessed at the same time by different threads
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int _levelOfDetail, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, _levelOfDetail, callback);
        };
        new Thread(threadStart).Start();
    }

    private void MeshDataThread(MapData _mapData, int _levelOfDetail, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(_mapData.heightMap, terrainData.heightMultiplier, terrainData.meshHeightCurve, _levelOfDetail, terrainData.useFlatShading);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

    }

    private MapData GenerateMapData(Vector2 _centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, _centre + noiseData.offset, noiseData.normaliseMode);

        if (terrainData.applyFallOffMap)
        {
            if (fallOffMap == null) {
                fallOffMap = FallOffGenerator.GenerateFallOffMap(mapChunkSize + 2);
            }
            for (int y = 0; y < mapChunkSize + 2; y++)
            {
                for (int x = 0; x < mapChunkSize + 2; x++)
                {
                    if (terrainData.applyFallOffMap)
                    {
                        noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - fallOffMap[x, y]);
                    }
                }
            }
        }

        return new MapData(noiseMap);
    }


    /// <summary>
    /// Clamps the values of the variables whenever one of them is changed
    /// </summary>
    void OnValidate()
    {
        if(terrainData != null)
        {
            // Ensure the subscription count stays at one
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if(noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

public struct MapData
{
    public float[,] heightMap;

    public MapData(float[,] _heightMap)
    {
        this.heightMap = _heightMap;
    }
}