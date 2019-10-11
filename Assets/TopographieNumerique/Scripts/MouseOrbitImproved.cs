using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MouseOrbitImproved : MonoBehaviour
{
	public Transform target;
	public Vector4 speed = Vector4.one;
	public float acceleration = 0.99f;
	public float damping = 0.1f;
	
	private Vector3 orbitVelocity = Vector3.zero;
	private Vector2 rotation = Vector2.zero;
	private Vector3 targetOffset = Vector3.zero;
	private float zoom = 0f;
	[HideInInspector] public float distance;
	private Camera cameraComponent;
	private float fov;
	// private bool should;

	private Vector3 startPos;
	private Quaternion startRot;
	private Vector3 startTarget;
	private float startFov;

	void Start () 
	{
		Cursor.visible = false;
		if (target == null) {
			GameObject t = new GameObject("Camera Target");
			target = t.transform;
		}
		distance = Vector3.Distance(transform.position, target.position);
		rotation.x = transform.rotation.eulerAngles.y;
		rotation.y = transform.rotation.eulerAngles.x;
		cameraComponent = GetComponent<Camera>();
		fov = cameraComponent.fieldOfView;
		startFov = fov;
		startPos = transform.position;
		startRot = transform.rotation;
		startTarget = target.position;
		// should = false;
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.R)) {
			fov = startFov;
			transform.position = startPos;
			transform.rotation = startRot;
			target.position = startTarget;
			distance = Vector3.Distance(transform.position, target.position);
			rotation.x = transform.rotation.eulerAngles.y;
			rotation.y = transform.rotation.eulerAngles.x;
			orbitVelocity = Vector3.zero;
			targetOffset = Vector3.zero;
			zoom = 0f;
		}

		bool pad1 = Input.GetKeyDown(KeyCode.Keypad1);
		bool pad3 = Input.GetKeyDown(KeyCode.Keypad3);
		bool pad7 = Input.GetKeyDown(KeyCode.Keypad7);
		if (pad1 || pad7 || pad3) {
			fov = startFov;
			if (pad1) transform.position = Vector3.forward * 80f;
			if (pad3) transform.position = Vector3.right * 80f;
			if (pad7) transform.position = Vector3.up * 80f;
			target.position = Vector3.zero;
			transform.LookAt(target);
			distance = Vector3.Distance(transform.position, target.position);
			rotation.x = transform.rotation.eulerAngles.y;
			rotation.y = transform.rotation.eulerAngles.x;
			orbitVelocity = Vector3.zero;
			targetOffset = Vector3.zero;
			zoom = 0f;
		}

		bool left = Input.GetMouseButton(0);
		bool right = Input.GetMouseButton(1);
		bool middle = Input.GetMouseButton(2);
		// bool space = Input.GetKey(KeyCode.Space);
		float wheel = Input.GetAxis("Mouse ScrollWheel");
		// if (space) should = true;
		// if (Input.anyKey && !left && !right && !middle && wheel == 0f && !space) should = false;

		// if (should) {
			if (left) Rotate();
			if (right) Zoom();
			if (middle) Translate();
			fov = Mathf.Clamp(fov - wheel * 20f, 10f, 180f);
		// }

		ApplyTransformation();

		cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, fov, Time.deltaTime * 5f);

		Shader.SetGlobalFloat("_MouseX", rotation.x / 360f);
		Shader.SetGlobalFloat("_MouseY", rotation.y / 360f);
		
		Shader.SetGlobalVector("_CameraPosition", transform.position);
		Shader.SetGlobalVector("_CameraTarget", target.position);
		Shader.SetGlobalVector("_CameraForward", transform.forward);
		Shader.SetGlobalVector("_CameraUp", transform.up);
		Shader.SetGlobalVector("_CameraRight", transform.right);
	}

	void Rotate ()
	{
		orbitVelocity.x = Mathf.Lerp(orbitVelocity.x, Input.GetAxis("Mouse X") * speed.x, damping);
		orbitVelocity.y = Mathf.Lerp(orbitVelocity.y, Input.GetAxis("Mouse Y") * speed.y, damping);
	}

	void Translate ()
	{
		targetOffset = Vector3.Lerp(targetOffset, Input.GetAxis("Mouse X") * transform.right * speed.w, damping);
		targetOffset = Vector3.Lerp(targetOffset, Input.GetAxis("Mouse Y") * transform.up * speed.w, damping);
	}

	void Zoom ()
	{
		zoom = Mathf.Lerp(zoom, Input.GetAxis("Mouse Y") * speed.z, damping);
	}

	void ApplyTransformation ()
	{
		rotation.x += orbitVelocity.x;
		rotation.y -= orbitVelocity.y;
		transform.rotation = Quaternion.Euler(rotation.y, rotation.x, 0);

		target.position -= targetOffset;

		distance = Mathf.Max(0.001f, distance + zoom);
		transform.position = transform.rotation * (Vector3.back * distance) + target.position;
		
		orbitVelocity *= acceleration;
		zoom *= acceleration;
		targetOffset *= acceleration;
	}

	public void Set (float x, float y, float dist) {
		rotation.x = y;
		rotation.y = x;
		distance = dist;
		target.position = Vector3.zero;
	}

	public void Set (Vector3 position) {
		transform.position = position;
		distance = Vector3.Distance(transform.position, target.position);
		rotation.x = transform.rotation.eulerAngles.y;
		rotation.y = transform.rotation.eulerAngles.x;
	}

	public IEnumerator GoTo (float delay, float x, float y, float dist) {
		float t = 0f;
		float ox = rotation.y;
		float oy = rotation.x;
		float od = distance;
		while (t <= 1f) {	
			Set(Mathf.Lerp(ox, x, t), Mathf.Lerp(oy, oy + y, t), Mathf.Lerp(od, dist, t));
			yield return null;
			t += Time.deltaTime / delay;
		}
	}
}
