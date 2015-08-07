using UnityEngine;
using System.Collections;
using Leap;

public class tracking : MonoBehaviour {

	public Controller controller;
	public int cursorSize = 25;
	
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

	void OnTriggerEnter(Collider other) {
		print ("collision");
	}

	void trackMouse()
	{	
		// Cursor follow mouse.
		// Don't let cursor go outside screen.
		Vector2 v = Input.mousePosition;
		v.x = Mathf.Clamp (Input.mousePosition.x, cursorSize, UnityEngine.Screen.width - cursorSize);
		v.y = Mathf.Clamp (Input.mousePosition.y, cursorSize, UnityEngine.Screen.height - cursorSize);

		// Transform screen point to world point.
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (v);
		GetComponent<Rigidbody2D> ().position = new Vector2(mousePosition.x, mousePosition.y);
	}

	void trackHand()
	{	
		// Cursor follow LeapMotion hand position.
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands [0];
		Vector3 v = hand.StabilizedPalmPosition.ToUnity();

		// LeapMotion tracking range in mm
		// y 100mm - 250mm
		// x (-)160mm - 160mm
		// z ignored.

		// Limit interaction range (Minimizes RSI).
		v.x = Mathf.Clamp (v.x, -120, 120);
		v.y = Mathf.Clamp (v.y, 100, 250);

		// Transform LeapMotion mm into Unity world point.
		v.x = ((v.x + 120) / 240) * UnityEngine.Screen.width;
		v.y = ((v.y - 100) / 150) * UnityEngine.Screen.height;

		// Limit cursor draw range i.e. Keep cursor inside window.
		v.x = Mathf.Clamp (v.x, cursorSize, UnityEngine.Screen.width - cursorSize);
		v.y = Mathf.Clamp (v.y, cursorSize, UnityEngine.Screen.height - cursorSize);

		Vector3 z = Camera.main.ScreenToWorldPoint (v);

		GetComponent<Rigidbody2D> ().position = new Vector2 (z.x, z.y);
	}



}
