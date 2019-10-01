using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(fileName = "Profile", menuName = "Profile", order = 100)]
public class Profile : ScriptableObject
{
	[Header("Geometry")]
	public float _Radius = 1.0f;
	public float _Height = 0.2f;
	public float _HeightVariation = 0.1f;
	public float _WindNoiseScale = 1.0f;
	public float _WindStrength = 0.1f;
	public float _WindSpeed = 1.0f;

	[Header("Inertia")]
	public float _Speed = 1.0f;
	public float _Friction = 1.0f;
	[Range(0,1)] public float _TrailDamping = 0.5f;
	
	[Header("Forces")]
	[Range(0,1)] public float _Curl = 1f;
	[Range(0,1)] public float _Attract = 1f;
	[Range(0,1)] public float _Twirl = 1f;
	[Range(0,1)] public float _Gravity = 1f;
	[Range(0,1)] public float _Expand = 1f;
	[Range(0,1)] public float _Grain = 1f;

	[Header("Distortion")]
	public float _NoiseScale = 1.0f;
}