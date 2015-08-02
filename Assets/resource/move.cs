using UnityEngine;
using System.Collections;
using Leap;

public class move : MonoBehaviour {

	Controller controller;
	public float sensitivity = 1.5f;
	

	// Use this for initialization
	void Start () {
		controller = new Controller();
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands [0];
		Vector3 v = hand.StabilizedPalmPosition.ToUnityScaled();

		v.Scale (new Vector3(sensitivity, sensitivity, 0.0f));
		Vector3 z = new Vector3(v.x * 4.0f, v.y * 2.5f, 0.0f);
		z.y = z.y - 12.0f;
		print (v);
		//Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		GetComponent<Rigidbody2D> ().position = Vector2.Lerp(transform.position, z, 1);
	}
}
