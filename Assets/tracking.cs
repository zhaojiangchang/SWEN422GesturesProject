using UnityEngine;
using System.Collections;
using Leap;

public class tracking : MonoBehaviour {
	
	Controller controller;
	public float sensitivity = 1.5f; // Adjusts speed of LeapMotion tracking.
	
	// Use this for initialization
	void Start () {
		controller = new Controller();
	}
	
	// Update is called once per frame
	void Update () {

		if (controller.IsConnected)
			trackHand();
		else
			trackMouse();

	}

	void trackMouse()
	{
		// Cursor follow mouse.
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		GetComponent<Rigidbody2D> ().position = Vector2.Lerp(transform.position, mousePosition, 1);
		print (transform.position);
		print (Input.mousePosition);
	}

	void trackHand()
	{
		// Cursor follow LeapMotion hand position.
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands [0];
		Vector3 v = hand.StabilizedPalmPosition.ToUnityScaled ();
		
		v.Scale (new Vector3(sensitivity, sensitivity, 0.0f));
		Vector3 z = new Vector3(v.x * 4.0f, v.y * 2.5f, 0.0f);
		z.y = z.y - 12.0f;
		GetComponent<Rigidbody2D> ().position = Vector2.Lerp(transform.position, z, 1);
	}

}
