
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
	public Material material, materialInit, materialPosition, materialVelocity, materialTrail;
	public Profile profile;
	public Texture heightMap;

	private string[] uniformsProfile;
	private FieldInfo[] profileFields;
	private int dimension;
	private FrameBuffer framePosition, frameVelocity, frameTrail;

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

		dimension = (int)Mathf.Pow(2f, Mathf.Ceil(Mathf.Log(Mathf.Sqrt(particleCount)) / Mathf.Log(2f)));

		frameVelocity.Blit(materialInit);
		framePosition.Blit(materialInit);
		frameTrail.Blit(materialInit);
		frameVelocity.Blit(materialInit);
		framePosition.Blit(materialInit);
		frameTrail.Blit(materialInit);
	}

	void Update ()
	{
		for (int i = 0; i < profileFields.Length; ++i)
			SetFloat(uniformsProfile[i], (float)profileFields[i].GetValue(profile));
		SetFloat("_Count", particleCount);
		SetFloat("_TrailSegment", trailSegment);
		SetFloat("_TimeElapsed", Time.time);
		SetFloat("_Dimension", dimension);
		SetFloat("_TimeDelta", Time.deltaTime);
		SetMatrix("_MatrixWorld", transform.localToWorldMatrix);
		SetMatrix("_MatrixLocal", transform.worldToLocalMatrix);
		SetTexture("_Velocity", frameVelocity.GetTexture());
		SetTexture("_Position", framePosition.GetTexture());
		SetTexture("_Trail", frameTrail.GetTexture());
		SetTexture("_HeightMap", heightMap);

		frameVelocity.Blit(materialVelocity);
		framePosition.Blit(materialPosition);
		frameTrail.Blit(materialTrail);
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

	Vector3 VectorRange (float min, float max) {
		return new Vector3(UnityEngine.Random.Range(min,max), UnityEngine.Random.Range(min,max), UnityEngine.Random.Range(min,max));
	}
}
