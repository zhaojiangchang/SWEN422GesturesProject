/**
* Image Behaviours Class
* Image Select: Right hand hover over image + left hand grab to select.
* Image Move: Grab to move select image.
* Image Rotate: Select iamge than Circle gesture
* Image Zooming: one hand hover image then push(zoom in)/pull(zoom out) to zooming the iamge.
* Image Drop: TPYEKEY to drop image.
* Image occludes other images (change the order): one hand hover image, another hand push (forward)/pull(backward) the iamge.
**/
using UnityEngine;
using System.Collections.Generic;
using Leap;
using System.Diagnostics;
using System.Threading;

public class ImageBehaviour : MonoBehaviour {
	
	private static bool intialized;
	private static Controller controller;
	public static List<Rigidbody2D> images = new List<Rigidbody2D>();
	Rigidbody2D image;
	private static float panelWidth;
	private Vector3 intialPosition;
	private static GameObject cursor;
	
	
	// Use this for initialization
	void Start () 
	{
		image = this.GetComponent<Rigidbody2D> ();
		intialPosition = image.position;
		
		if (intialized)
			return;

		panelWidth = ((float) UnityEngine.Screen.width) - GameObject.Find ("Sidebar").GetComponent<RectTransform> ().rect.width;
		cursor = GameObject.Find ("glowing_ring");
		controller = new Controller ();
		
		// Adjust screen tap sensitivity etc.
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		controller.Config.SetFloat ("Gesture.KeyTap.MinDownVelocity", 20.0f);
		controller.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", 0.2f);
		controller.Config.SetFloat ("Gesture.KeyTap.MinDistance", 4.0f);
		
		controller.EnableGesture(Leap.Gesture.GestureType.TYPECIRCLE);
		controller.Config.SetFloat("Gesture.Circle.MinRadius", 10.0f);
		controller.Config.SetFloat("Gesture.Circle.MinArc", 5.0f);
		
		controller.Config.Save();
		
		intialized = true;
	}
	
	float enterZ;
	Stopwatch timer;
	void OnTriggerEnter2D(Collider2D other)
	{
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands[0];
		enterZ = hand.Fingers [0].TipPosition.z;
		timer = new Stopwatch ();
		timer.Start ();
		cursor.GetComponent<Animator> ().enabled = true;
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		timer.Stop ();
		cursor.GetComponent<Animator> ().enabled = false;
	}
	//Trigger the gesture when hand is hovering over an object
	void OnTriggerStay2D(Collider2D other) 
	{	
		// Images can collide. Only bother with this if its the cursor.
		if (!other.gameObject.name.Equals ("glowing_ring"))
			return;
		
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands[0];
		Hand otherHand = frame.Hands[1];
		Gesture gesture = frame.Gestures () [0];

		print ("x: "+ this.image.position.x + "y: "+this.image.position.y);

		
		if (!images.Contains (image) && otherHand.GrabStrength > 0.7f && frame.Hands.Count > 1) {
//			print ("Selected: " + this.gameObject.name);
//			print ("Added: " + this.gameObject.name);
			images.Add (image);
			this.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
		}
		
		// Unselect images.
		if (gesture.Type == Gesture.GestureType.TYPEKEYTAP) {
			images.Clear ();
			this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
			timer.Reset ();
		} 
		// Rotate images.
		else if (images.Contains (image) && gesture.Type == Gesture.GestureType.TYPECIRCLE) {
			CircleGesture circle = new CircleGesture (frame.Gestures () [0]);
			Vector centerPoint = circle.Center;
			Pointable circlePointable = circle.Pointable;
			
			if (circle.Pointable.Direction.AngleTo (circle.Normal) <= 3.1415 / 2) {
				image.transform.Rotate (Vector3.back, 45 * Time.deltaTime);
			} else {
				image.transform.Rotate (Vector3.forward, 45 * Time.deltaTime);
			}
		}
		
		// If nothing is selected and cursor is hovering over an image start Zoom.
		if (timer == null)
			return;
		if (images.Count == 0 && timer.ElapsedMilliseconds > 2000 && frame.Hands.Count == 1) {
			
			float z = hand.Fingers [0].TipPosition.z > enterZ ? 0.5f : -0.5f;
			
			// Limit zoom level.
			if (transform.localScale.x > 300 && z == -0.5f || transform.localScale.x < 50 && z == 0.5f) 
				return;
			
			transform.localScale = new Vector2 (transform.localScale.x - z, transform.localScale.y - z);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands [0];
		
		if (frame.Gestures () [0].Type == Gesture.GestureType.TYPEKEYTAP) 
		{
			this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
			images.Clear ();
		}

		// if at least one image is selected
		if(images.Contains(image) && frame.Hands.Count > 1)
		{
			float z = frame.Hands.Leftmost.PalmPosition.z;
			z = Mathf.Clamp(z, 0.0f, 120.0f);
			int depth = (int) Mathf.Floor((z / 120.0f) * 5.0f);
			this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder = depth;
		}

		// position for deleting an object and putting it back in the menu
		if (this.image.position.x < -1600.0f && this.image.position.y < -1000.0f) {
			
			Vector3 rot = transform.rotation.eulerAngles;
			rot = new Vector3(0,0,0);
			image.transform.localRotation = Quaternion.Euler (rot);
			image.transform.localScale = new Vector3(45.0f, 45.0f, 0.0f);
			this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
			image.position = intialPosition;
			images.Remove (image);
		}

		// for moving the object
		if(images.Contains(image) && hand.PinchStrength > 0.7f)
			trackLeap (frame);
	}
	
	void trackLeap(Frame frame)
	{	
		int sep = images.IndexOf (image);
		float xOffset = transform.localScale.x * 1.4f;
		float yOffset = transform.localScale.y;

		// Cursor follow LeapMotion hand position.
		Vector3 z = tracking.handPosition;
		image.position = new Vector2 (z.x, (z.y + (sep * yOffset * 10f)));
	}
}
