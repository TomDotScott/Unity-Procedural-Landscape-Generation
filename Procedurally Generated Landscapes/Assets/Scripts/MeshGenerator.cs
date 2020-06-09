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