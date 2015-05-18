using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

	public Popup button;

	// Use this for initialization
	void Start () {

		name = "ui";
		button = GetComponentInChildren<Popup>();
		button.gameObject.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void displayPopup(GameObject target){
		print("display");
		button.gameObject.SetActive(true);
		button.gameObject.GetComponent<RectTransform>().transform.position = Input.mousePosition;
	}
}
