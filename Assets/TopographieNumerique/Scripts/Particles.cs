
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Particles : MonoBehaviour
{
	public int particleCount = 100000;
	public int trailSegment = 10;
	public Material material, materialPosition, materialVelocity, materialTrail;
	public Profile profile;

	private string[] uniformsProfile;
	private FieldInfo[] profileFields;

	private FrameBuffer framePosition, frameVelocity, frameTrail;
	// private struct PointData { public Vector3 position, velocity, info; }
	// private struct TrailData { public Vector3 position, info; }
	// private PointData[] particles;
	// private TrailData[] trails;
	// private ComputeBuffer particleBuffer, trailBuffer;

	void Start ()
	{
		profileFields = profile.GetType().GetFields();
		uniformsProfile = new string[profileFields.Length];
		for (int i = 0; i < profileFields.Length; ++i) uniformsProfile[i] = profileFields[i].Name;

		Vector3[] positions = new Vector3[particleCount];
		for (int index = 0; index < particleCount; ++index) positions[index] = VectorRange(-1f,1f);
		gameObject.GetComponent<MeshFilter>().mesh = Geometry.Particles(positions, null, null, 1, trailSegment);

		framePosition = new FrameBuffer(2, particleCount, 1, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		frameVelocity = new FrameBuffer(2, particleCount, 1, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		frameTrail = new FrameBuffer(2, particleCount, trailSegment, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

		// particles = new PointData[particleCount];
		// for (int i = 0; i < particleCount; ++i) {
		// 	particles[i].position = VectorRange(-1f,1f);
		// 	particles[i].velocity = Vector3.zero;
		// 	particles[i].info = Vector3.zero;
		// }
		// trails = new TrailData[particleCount*trailSegment];
		// for (int i = 0; i < particleCount; ++i) {
		// 	for (int t = 0; t < trailSegment; ++t) {
		// 		trails[i*trailSegment+t].position = particles[i].position;
		// 		trails[i*trailSegment+t].info = Vector3.zero;
		// 	}
		// }

		// particleBuffer = new ComputeBuffer(particles.Length, Marshal.SizeOf(typeof(PointData)));
		// particleBuffer.SetData(particles);
		// SetBuffer("_Particles", particleBuffer);

		// trailBuffer = new ComputeBuffer(trails.Length, Marshal.SizeOf(typeof(TrailData)));
		// trailBuffer.SetData(trails);
		// SetBuffer("_Trails", trailBuffer);
	}

	void Update ()
	{
		// if (Input.GetKeyDown(KeyCode.R)) {
		// 	Vector3[] vertices = gameObject.GetComponent<MeshFilter>().mesh.vertices;
		// 	for (int i = 0; i < particleCount; ++i) {
		// 		particles[i].position = vertices[i];
		// 		particles[i].velocity = Vector3.zero;
		// 	}
		// 	particleBuffer.SetData(particles);
		// }

		SetFloat("_Count", particleCount);
		SetFloat("_TrailSegment", trailSegment);
		SetFloat("_TimeElapsed", Time.time);
		SetFloat("_TimeDelta", Time.deltaTime);
		SetMatrix("_MatrixWorld", transform.localToWorldMatrix);
		SetMatrix("_MatrixLocal", transform.worldToLocalMatrix);


		// #if UNITY_EDITOR
		// SetBuffer("_Particles", particleBuffer);
		// SetBuffer("_Trails", trailBuffer);
		// #endif

		for (int i = 0; i < profileFields.Length; ++i)
			SetFloat(uniformsProfile[i], (float)profileFields[i].GetValue(profile));

		frameVelocity.Blit(materialVelocity);
		framePosition.Blit(materialPosition);
		frameTrail.Blit(materialTrail);

		SetTexture("_Velocity", frameVelocity.GetTexture());
		SetTexture("_Position", framePosition.GetTexture());
		SetTexture("_Trail", frameTrail.GetTexture());

		// compute.Dispatch(0, particles.Length/8, 1, 1);
		// compute.Dispatch(1, trails.Length/8, 1, 1);
	}

	void SetVector(string name, Vector3 v) {
		material.SetVector(name, v);
		materialPosition.SetVector(name, v);
		materialVelocity.SetVector(name, v);
		materialTrail.SetVector(name, v);
	}

	void SetFloat(string name, float v) {
		material.SetFloat(name, v);
		materialPosition.SetFloat(name, v);
		materialVelocity.SetFloat(name, v);
		materialTrail.SetFloat(name, v);
	}

	void SetTexture(string name, Texture v) {
		material.SetTexture(name, v);
		if (v != null) materialPosition.SetTexture(name, v);
		if (v != null) materialVelocity.SetTexture(name, v);
		if (v != null) materialTrail.SetTexture(name, v);
	}

	void SetMatrix(string name, Matrix4x4 v) {
		material.SetMatrix(name, v);
		materialPosition.SetMatrix(name, v);
		materialVelocity.SetMatrix(name, v);
		materialTrail.SetMatrix(name, v);
	}

	// void SetBuffer(string name, ComputeBuffer v) {
	// 	material.SetBuffer(name, v);
	// 	if (v != null) compute.SetBuffer(0, name, v);
	// 	if (v != null) compute.SetBuffer(1, name, v);
	// }

	Vector3 VectorRange (float min, float max) {
		return new Vector3(UnityEngine.Random.Range(min,max), UnityEngine.Random.Range(min,max), UnityEngine.Random.Range(min,max));
	}

	// void OnDestroy () {
	// 	if (particleBuffer != null) particleBuffer.Dispose();
	// 	if (trailBuffer != null) trailBuffer.Dispose();
	// }
}
