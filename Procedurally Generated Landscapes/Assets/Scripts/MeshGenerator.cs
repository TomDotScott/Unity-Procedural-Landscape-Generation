using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] _heightMap, float _heightMultiplier, AnimationCurve _heightCurve, int _levelOfDetail)
    {
        int meshSimplificationIncrement = (_levelOfDetail == 0) ? 1 : _levelOfDetail * 2;

        // Allows each thread to use the animation curve without locking the thread 
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
        int borderedSize = _heightMap.GetLength(0);
        int meshSize = borderedSize - 2 * meshSimplificationIncrement;
        int meshSizeUnsimplified = borderedSize - 2;

        float topLeftX = (meshSizeUnsimplified - 1) / -2f;
        float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

        int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine);

        int[,] vertextIndicesMap = new int[borderedSize, borderedSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;

        for (int y = 0; y < borderedSize; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
            {
                bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

                if (isBorderVertex)
                {
                    vertextIndicesMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                }
                else
                {
                    vertextIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int y = 0; y < borderedSize; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
            {
                int vertexIndex = vertextIndicesMap[x, y];
                
                Vector2 percent = new Vector2((x - meshSimplificationIncrement) / (float)meshSize, (y - meshSimplificationIncrement) / (float)meshSize);
                float height = heightCurve.Evaluate(_heightMap[x, y]) * _heightMultiplier;
                Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);

                meshData.AddVertex(vertexPosition, percent, vertexIndex);

                if (x < borderedSize - 1 && y < borderedSize - 1)
                {
                    int a = vertextIndicesMap[x, y];
                    int b = vertextIndicesMap[x + meshSimplificationIncrement, y];
                    int c = vertextIndicesMap[x, y + meshSimplificationIncrement];
                    int d = vertextIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];

                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);
                }

                vertexIndex++;
            }
        }

        return meshData;

    }
}

public class MeshData
{
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    private Vector3[] borderVertices;
    int[] borderTriangles;

    private int triangleIndex;
    private int borderTriangleIndex;

    public MeshData(int _verticesPerLine)
    {
        vertices = new Vector3[_verticesPerLine * _verticesPerLine];
        uvs = new Vector2[_verticesPerLine * _verticesPerLine];
        triangles = new int[(_verticesPerLine - 1) * (_verticesPerLine - 1) * 6];

        borderVertices = new Vector3[_verticesPerLine * 4 + 4];
        borderTriangles = new int[24 * _verticesPerLine];
    }

    public void AddVertex(Vector3 _vertexPosition, Vector2 _vertexUV, int _vertexIndex)
    {
        if(_vertexIndex < 0)
        {
            borderVertices[-_vertexIndex - 1] = _vertexPosition;
        }
        else
        {
            vertices[_vertexIndex] = _vertexPosition;
            uvs[_vertexIndex] = _vertexUV;
        }
    }

    public void AddTriangle(int _a, int _b, int _c)
    {
        // if any of the triangles are border vertices
        if (_a < 0 || _b < 0 || _c < 0)
        {
            borderTriangles[borderTriangleIndex] = _a;
            borderTriangles[borderTriangleIndex + 1] = _b;
            borderTriangles[borderTriangleIndex + 2] = _c;
            borderTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = _a;
            triangles[triangleIndex + 1] = _b;
            triangles[triangleIndex + 2] = _c;
            triangleIndex += 3;
        }
    }

    private Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            // find the indices in the vertex array that make up the current triangle
            int vertextIndexA = triangles[normalTriangleIndex];
            int vertextIndexB = triangles[normalTriangleIndex + 1];
            int vertextIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertextIndexA, vertextIndexB, vertextIndexC);

            // add the triangle normal to the vertices that are part of the triangle
            vertexNormals[vertextIndexA] += triangleNormal;
            vertexNormals[vertextIndexB] += triangleNormal;
            vertexNormals[vertextIndexC] += triangleNormal;
        }

        // Go through the border as well to get continuous normals
        int borderTriangleCount = borderTriangles.Length / 3;

        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            // find the indices in the vertex array that make up the current triangle
            int vertextIndexA = borderTriangles[normalTriangleIndex];
            int vertextIndexB = borderTriangles[normalTriangleIndex + 1];
            int vertextIndexC = borderTriangles[normalTriangleIndex + 2];

            Vector3 borderTriangleNormal = SurfaceNormalFromIndices(vertextIndexA, vertextIndexB, vertextIndexC);

            if (vertextIndexA >= 0)
            {
                vertexNormals[vertextIndexA] += borderTriangleNormal;
            }
            if (vertextIndexB >= 0)
            {
                vertexNormals[vertextIndexB] += borderTriangleNormal;
            }
            if (vertextIndexC >= 0)
            {
                vertexNormals[vertextIndexC] += borderTriangleNormal;
            }
        }

        // Normalise the values in the array
        foreach (var vector in vertexNormals)
        {
            vector.Normalize();
        }

        return vertexNormals;
    }

    Vector3 SurfaceNormalFromIndices(int _indexA, int _indexB, int _indexC)
    {
        Vector3 pointA = (_indexA < 0) ? borderVertices[- _indexA - 1] : vertices[_indexA];
        Vector3 pointB = (_indexB < 0) ? borderVertices[- _indexB - 1] : vertices[_indexB];
        Vector3 pointC = (_indexC < 0) ? borderVertices[- _indexC - 1] : vertices[_indexC];

        // use the cross product to calculate the normals
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };

        mesh.normals = CalculateNormals();
        return mesh;
    }

}