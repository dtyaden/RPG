using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Camera camera;
	private float zoomSpeed;
	public bool cameraMoving;

	// Use this for initialization
	void Start () {
		camera.orthographicSize = 6;
		zoomSpeed = 2;
	}
	
	// Update is called once per frame
	void Update () {
			this.camera.orthographicSize += Input.GetAxis("Mouse ScrollWheel")*-1*zoomSpeed;
			
		if(Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift)){
			cameraMoving = true;
			Vector3 position = this.camera.transform.position;
			position += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"),0.0f)*-1;
			this.camera.transform.position = position;
		}
	}
}
