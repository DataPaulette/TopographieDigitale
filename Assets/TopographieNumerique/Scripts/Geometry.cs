using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour {

	public int count = 1000;
	public Vector2 segment = Vector2.one;
	public int width = 100;
	public int height = 100;

	void Start () {
		Vector3[] positions = new Vector3[count];
		for (int index = 0; index < count; ++index) positions[index] = VectorRange(-1f,1f);
		GetComponent<MeshFilter>().mesh = Particles(positions, null, null, (int)segment.x, (int)segment.y);
	}

	public static Mesh Particles (Vector3[] positions, Color[] colors_ = null, Vector3[] normals_ = null, float sliceX = 1f, float sliceY = 1f) {
		Vector2 faces = new Vector2(sliceX+1f, sliceY+1f);
		int amount = positions.Length;
		int vertexCount = (int)(faces.x * faces.y);
		int totalVertices = amount * vertexCount;
		int mapIndex = 0;
		int count = totalVertices;
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Color> colors = new List<Color>();
		List<Vector2> anchors = new List<Vector2>();
		List<Vector2> quantities = new List<Vector2>();
		List<int> indices = new List<int>();
		int vIndex = 0;
		for (int index = 0; index < count/(faces.x*faces.y); ++index) {
			Vector3 position = positions[mapIndex];
			Vector3 normal = Vector3.up;
			Color color = Color.white;
			if (colors_ != null) color = colors_[mapIndex];
			if (normals_ != null) normal = normals_[mapIndex];
			for (int y = 0; y < faces.y; ++y) {
				for (int x = 0; x < faces.x; ++x) {
					quantities.Add(new Vector2(mapIndex/(float)amount,mapIndex));
					vertices.Add(position);
					if (normals_ != null) normals.Add(normal);
					if (colors_ != null) colors.Add(color);
					anchors.Add(new Vector2(((float)x/(float)sliceX)*2f-1f, ((float)y/(float)sliceY)*2f-1f));
				}
			}
			for (int y = 0; y < sliceY; ++y) {
				for (int x = 0; x < sliceX; ++x) {
					indices.Add(vIndex);
					indices.Add(vIndex+1);
					indices.Add(vIndex+1+(int)sliceX);
					indices.Add(vIndex+1+(int)sliceX);
					indices.Add(vIndex+1);
					indices.Add(vIndex+2+(int)sliceX);
					vIndex += 1;
				}
				vIndex += 1;
			}
			vIndex += (int)faces.x;
			++mapIndex;
		}
		Mesh mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = vertices.ToArray();
		if (normals_ != null) mesh.normals = normals.ToArray();
		if (colors_ != null) mesh.colors = colors.ToArray();
		mesh.uv = anchors.ToArray();
		mesh.uv2 = quantities.ToArray();
		mesh.SetTriangles(indices.ToArray(), 0);
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10f);
		return mesh;
	}

	static public Mesh Grid (int width, int height, bool wireframe = false) {
		Mesh mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		for (int i = 0, y = 0; y <= height; y++) {
			for (int x = 0; x <= width; x++, i++) {
				float xx = (float)x / (float)width;
				float yy = (float)y / (float)height;
				vertices[i] = new Vector3((xx*2f-1f), (yy*2f-1f), 0f);
				uv[i] = new Vector2(xx, yy);
				tangents[i] = tangent;
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;
		int[] triangles = new int[width * height * 6];
		for (int ti = 0, vi = 0, y = 0; y < height; y++, vi++) {
			for (int x = 0; x < width; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + width + 1;
				triangles[ti + 5] = vi + width + 2;
			}
		}
		mesh.triangles = triangles;
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 20f);
		if (wireframe) {
			List<int> lines = new List<int>();
			for (int index = 0; index < triangles.Length-2; index+=3) {
				lines.Add(triangles[index]);
				lines.Add(triangles[index+1]);
				lines.Add(triangles[index+1]);
				lines.Add(triangles[index+2]);
				lines.Add(triangles[index+2]);
				lines.Add(triangles[index]);
			}
			mesh.SetIndices(lines.ToArray(), MeshTopology.Lines, 0);
		}
		return mesh;
	}

	Vector3 VectorRange (float min, float max) {
		return new Vector3(UnityEngine.Random.Range(min,max), UnityEngine.Random.Range(min,max), UnityEngine.Random.Range(min,max));
	}
}
