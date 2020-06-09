using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    private const float viewerMoveThresholdForChunkUpdate = 25f;
    private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public LODInfo[] detailLevels;
    public static float maxViewDistance;

    public Transform viewer;

    private static MapGenerator mapGenerator;

    private Vector2 previousViewerPosition;
    public static Vector2 viewerPosition;
    public Material mapMaterial;


    private int chunkSize;
    private int chunksVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
        UpdateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if ((previousViewerPosition - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            previousViewerPosition = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        foreach(var chunk in terrainChunksVisibleLastUpdate)
        {
            chunk.SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        //find the coordinate of the chunk the player is standing on
        int currentChunkCoordinateX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordinateY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoordinate = new Vector2(currentChunkCoordinateX + xOffset, currentChunkCoordinateY + yOffset);
                if (terrainChunkDict.ContainsKey(viewedChunkCoordinate))
                {
                    //update the terrain chunk
                    terrainChunkDict[viewedChunkCoordinate].UpdateTerrainChunk();
                    if (terrainChunkDict[viewedChunkCoordinate].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDict[viewedChunkCoordinate]);
                    }
                }
                else
                {
                    //instantiate new terrain chunk
                    terrainChunkDict.Add(viewedChunkCoordinate, new TerrainChunk(viewedChunkCoordinate, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }


    public class TerrainChunk
    {
        private GameObject meshObject;
        private Vector2 position;
        private Bounds bounds;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private LODInfo[] detailLevels;
        private LODMesh[] lodMeshes;

        private MapData mapData;
        private bool mapDataReceived;

        private int previousLevelOfDetail = 1;


        public TerrainChunk(Vector2 _coord, int _size, LODInfo[] _detailLevels, Transform _parent, Material _mat)
        {
            detailLevels = _detailLevels;

            position = _coord * _size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            bounds = new Bounds(position, Vector2.one * _size);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = _mat;
            meshFilter = meshObject.AddComponent<MeshFilter>();

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = _parent;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }


            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        private void OnMapDataReceived(MapData _mapData)
        {
            mapData = _mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (mapDataReceived)
            {
                float viewerDistance = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistance <= maxViewDistance;
                // Compare the distance from the viewer to the distance thresholds for the Level of Details
                if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDistance > detailLevels[i].visibleDistanceThreshold)
                        {
                            // level of detail should be lower
                            lodIndex = i = 1;
                        }
                        else
                        {
                            // the correct LOD index
                            break;
                        }
                    }

                    // only update if the LOD has changed
                    if (lodIndex != previousLevelOfDetail)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLevelOfDetail = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                }
                SetVisible(visible);
            }
        }

        public void SetVisible(bool _visibility)
        {
            meshObject.SetActive(_visibility);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

    /// <summary>
    /// To fetch the mesh from the map generator
    /// </summary>
    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;


        public LODMesh(int _lod, System.Action _updateCallback)
        {
            this.lod = _lod;
            this.updateCallback = _updateCallback;
        }

        private void OnMeshDataReceived(MeshData _meshData)
        {
            mesh = _meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(MapData _mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(_mapData, lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDistanceThreshold;
    }
}