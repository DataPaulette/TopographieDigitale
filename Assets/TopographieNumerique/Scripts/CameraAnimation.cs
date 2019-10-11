using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
	private Vector3 origin;
	public bool interaction;
	private bool interacted;
	private float interactionStart;
	private float idleDelay = 10f;
	private float speed = 1f;
	private float speedTraveling = 0.1f;
	private float speedRotation = 1f;
	private float heightMax = 8f;
	private float heightMin = 2f;
	private float radiusMin = 6f;
	private float radiusMax = 8f;

	void Start () {
		origin = Camera.main.transform.position;
		interaction = false;
		interactionStart = 0f;
	}

	void Update () {
		if (interaction && !interacted) {
			interacted = true;
		}
		if (!interaction && interactionStart + idleDelay < Time.time) {
			interacted = false;
			transform.Rotate(Vector3.up * Time.deltaTime * speedRotation);
			Vector3 pos = Camera.main.transform.position;
			pos.y = Mathf.Lerp(pos.y, Mathf.Lerp(heightMin, heightMax, 0.5f+0.5f*Mathf.Sin(Time.time*speedTraveling)), Time.deltaTime * speed);
			float len = pos.magnitude;
			pos = Vector3.Normalize(pos) * Mathf.Lerp(len, Mathf.Lerp(radiusMin, radiusMax, 0.5f+0.5f*Mathf.Sin(Time.time*speedTraveling)), Time.deltaTime * speed);
			Camera.main.transform.position = pos;
		}

		if (interaction) {
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, origin, Time.deltaTime * speed);
			interactionStart = Time.time;
		}

		Camera.main.transform.LookAt(Vector3.zero);
	}

}
