using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] _heightMap)
    {
        int width = _heightMap.GetLength(0);
        int height = _heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);

        int currentVertex = 0;
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                meshData.verts[currentVertex] = new Vector3(topLeftX + x, _heightMap[x, y], topLeftZ - y);
                meshData.uvs[currentVertex] = new Vector2(x / (float)width, y / (float)height);
                // ignore the right and bottom-most vertices of the map 
                if(x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(currentVertex, currentVertex + width + 1, currentVertex + width);
                    meshData.AddTriangle(currentVertex + width + 1, currentVertex, currentVertex + 1);
                }

                currentVertex++;
            }
        }

        return meshData;
    }
   
}

public class MeshData
{
    public Vector3[] verts;
    public int[] tris;
    public Vector2[] uvs;

    int currentTriangle;

    public MeshData(int _width, int _height)
    {
        verts = new Vector3[_width * _height];
        uvs = new Vector2[_width * _height];
        tris = new int[(_width - 1) * (_height - 1) * 6];
    }

    public void AddTriangle(int _a, int _b, int _c)
    {
        tris[currentTriangle] = _a;
        tris[currentTriangle + 1] = _b;
        tris[currentTriangle + 2] = _c;
        currentTriangle += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh
        {
            vertices = verts,
            triangles = tris,
            uv = uvs
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}
