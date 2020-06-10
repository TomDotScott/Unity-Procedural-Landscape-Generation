using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
	public static MeshData GenerateTerrainMesh(float[,] _heightMap, float _heightMultiplier, AnimationCurve _heightCurve, int _levelOfDetail)
	{
		// Allows each thread to use the animation curve without locking the thread 
		AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
		int width = _heightMap.GetLength(0);
		int height = _heightMap.GetLength(1);
		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		int meshSimplificationIncrement = (_levelOfDetail == 0) ? 1 : _levelOfDetail * 2;
		int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

		MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
		int currentVertex = 0;

		for (int y = 0; y < height; y+= meshSimplificationIncrement)
		{
			for (int x = 0; x < width; x+= meshSimplificationIncrement)
			{

				meshData.vertices[currentVertex] = new Vector3(topLeftX + x, heightCurve.Evaluate(_heightMap[x, y]) * _heightMultiplier, topLeftZ - y);
				meshData.uvs[currentVertex] = new Vector2(x / (float)width, y / (float)height);

				if (x < width - 1 && y < height - 1)
				{
					meshData.AddTriangle(currentVertex, currentVertex + verticesPerLine+ 1, currentVertex + verticesPerLine);
					meshData.AddTriangle(currentVertex + verticesPerLine + 1, currentVertex, currentVertex + 1);
				}

				currentVertex++;
			}
		}

		return meshData;

	}
}

public class MeshData
{
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;

	int currentTriangle;

	public MeshData(int _width, int _height)
	{
		vertices = new Vector3[_width * _height];
		uvs = new Vector2[_width * _height];
		triangles = new int[(_width - 1) * (_height - 1) * 6];
	}

	public void AddTriangle(int _a, int _b, int _c)
	{
		triangles[currentTriangle] = _a;
		triangles[currentTriangle + 1] = _b;
		triangles[currentTriangle + 2] = _c;
		currentTriangle += 3;
	}

	private Vector3[] CalculateNormals() {
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

		// Normalise the values in the array
		foreach(var vector in vertexNormals)
        {
			vector.Normalize();
        }

		return vertexNormals;
	}

	Vector3 SurfaceNormalFromIndices(int _indexA, int _indexB, int _indexC)
    {
		Vector3 pointA = vertices[_indexA];
		Vector3 pointB = vertices[_indexB];
		Vector3 pointC = vertices[_indexC];

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