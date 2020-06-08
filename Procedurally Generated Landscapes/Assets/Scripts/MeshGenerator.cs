using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
	public static MeshData GenerateTerrainMesh(float[,] _heightMap, float _heightMultiplier, AnimationCurve _heightCurve)
	{
		int width = _heightMap.GetLength(0);
		int height = _heightMap.GetLength(1);
		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		MeshData meshData = new MeshData(width, height);
		int currentVertex = 0;

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{

				meshData.vertices[currentVertex] = new Vector3(topLeftX + x, _heightCurve.Evaluate(_heightMap[x, y]) * _heightMultiplier, topLeftZ - y);
				meshData.uvs[currentVertex] = new Vector2(x / (float)width, y / (float)height);

				if (x < width - 1 && y < height - 1)
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

	public Mesh CreateMesh()
	{
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        mesh.RecalculateNormals();
		return mesh;
	}

}