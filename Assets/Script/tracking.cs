using UnityEngine;
using System.Collections;
using Leap;

public class tracking : MonoBehaviour {

	public static Vector3 handPosition;
	public Controller controller;
	public int cursorSize = 25;
	
	// Use this for initialization
	void Start () {
		controller = new Controller();
	}
	
	// Update is called once per frame
	void Update () {

		if (controller.IsConnected) {
			trackHand (0);
			//trackHand (1);
		}
		else
			trackMouse();
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

	void trackHand(int handIndex)
	{	
		// Cursor follow LeapMotion hand position.
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands [handIndex];

		// Hand doesn't exist do nothing.
		if (hand.Direction.x == 0.0f)
			return;

		Vector3 v = hand.Fingers[0].StabilizedTipPosition.ToUnity();

		// LeapMotion tracking range in mm
		// y 100mm - 250mm
		// x (-)160mm - 160mm
		// z ignored.

		if (handIndex == 0) {
			// Limit interaction range (Minimizes RSI).
			v.x = Mathf.Clamp (v.x, -100, 100);
			v.y = Mathf.Clamp (v.y, 100, 250);

			// Transform LeapMotion mm into Unity world point.
			v.x = ((v.x + 100) / 200) * UnityEngine.Screen.width;
			v.y = ((v.y - 100) / 150) * UnityEngine.Screen.height;
		} 
		else 
		{
			// Limit interaction range (Minimizes RSI).
			v.x = Mathf.Clamp (v.x, -140, -30);
			v.y = Mathf.Clamp (v.y, 100, 250);
			
			// Transform LeapMotion mm into Unity world point.
			v.x = (1 - ((v.x + 30) / -110)) * UnityEngine.Screen.width;
			v.y = ((v.y - 100) / 150) * UnityEngine.Screen.height;
		}

		// Limit cursor draw range i.e. Keep cursor inside window.
		v.x = Mathf.Clamp (v.x, cursorSize, UnityEngine.Screen.width - cursorSize);
		v.y = Mathf.Clamp (v.y, cursorSize, UnityEngine.Screen.height - cursorSize);

		Vector3 z = Camera.main.ScreenToWorldPoint (v);
		handPosition = z;
		
		if (handIndex == 0) {
			GameObject.Find ("glowing_ring").GetComponent<Rigidbody2D> ().position = new Vector2 (z.x, z.y);
		}
		if(handIndex == 1)
			GameObject.Find ("cursor2").GetComponent<Rigidbody2D> ().position = new Vector2 (z.x, z.y);
	}


}
