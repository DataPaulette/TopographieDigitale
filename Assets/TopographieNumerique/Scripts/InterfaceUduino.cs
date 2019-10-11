using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class Sensor {
	public Vector2 position;
	public float min;
	public float max;
	public float treshold;

	public Sensor (Vector2 p, float min_, float max_, float treshold_) {
		position = p;
		min = min_;
		max = max_;
		treshold = treshold_;
	}
}

public class InterfaceUduino : MonoBehaviour
{
	private float[] values;
	private Sensor[] sensors;
	private Vector2 cursor;
	private float intensity;
	private int currentID;
	private CameraAnimation cameraAnimation;

	void Start () 
	{
		currentID = -1;
		intensity = 0f;
		cursor = new Vector2();
		values = new float[12];
		cameraAnimation = GameObject.FindObjectOfType<CameraAnimation>();
		sensors = new Sensor[] {
							// position			//min   //max   //threshold
			new Sensor(new Vector2(0.5f,0.5f), 	35f, 	120f, 	60f),
			new Sensor(new Vector2(-0.3f,0.5f), 25f, 	95f, 	60f),
			new Sensor(new Vector2(0f,-0.4f), 	15f, 	90f, 	60f),
			new Sensor(new Vector2(0.8f,-0.8f), 80f, 	15f, 	60f),
			new Sensor(new Vector2(-0.8f,-0.9f),25f, 	100f, 	60f),
			new Sensor(new Vector2(-0.4f,-0.3f),20f, 	90f, 	50f),
			new Sensor(new Vector2(-0.2f,-0.8f), 0f, 	60f, 	25f),
			new Sensor(new Vector2(-0.7f,0.2f), 35f, 	110f, 	70f),
			new Sensor(new Vector2(-0.3f,-0.1f),10f, 	70f, 	30f),
			new Sensor(new Vector2(-0.7f,0.7f), 20f, 	75f, 	50f),
			new Sensor(new Vector2(0f,0f), 		20f, 	80f, 	50f),
			new Sensor(new Vector2(0.8f,0f), 	10f, 	90f, 	50f)
		};
	}

	public void ValueReceived(string value, UduinoDevice device) {
		string[] datas = value.Split(',');
		for (int i = 0; i < datas.Length && i < values.Length; ++i) {
			int n;
			bool isNumeric = int.TryParse("123", out n);
			if (isNumeric) {
				values[i] = (float)Int32.Parse(datas[i]);
			}
		}
	}

	void Update () {
		float valueMin = 100000f;
		currentID = -1;
		for (int i = 0; i < values.Length; ++i) {
			float value = Mathf.InverseLerp(sensors[i].min, sensors[i].max, values[i]);
			if (value < valueMin && values[i] < sensors[i].treshold) {
				valueMin = value;
				currentID = i;
			}
		}
		if (currentID != -1) {
			cursor = Vector2.Lerp(cursor, sensors[currentID].position, Time.deltaTime);
			intensity = Mathf.Clamp01(intensity + Time.deltaTime);
			cameraAnimation.interaction = true;
		} else {
			intensity = Mathf.Clamp01(intensity - Time.deltaTime);
			cameraAnimation.interaction = false;
		}
		Shader.SetGlobalVector("_Cursor", cursor);
		Shader.SetGlobalFloat("_Intensity", intensity);
	}
}
