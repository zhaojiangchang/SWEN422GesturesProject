using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;
using System.Diagnostics;
using System.Linq;

public class ImageBehaviour : MonoBehaviour {

	Controller controller;
	public static List<Rigidbody2D> images = new List<Rigidbody2D>();
	Rigidbody2D image;
	float panelWidth = 780;

	// Use this for initialization

	void Start () {
		controller = new Controller ();
		image = this.GetComponent<Rigidbody2D> ();

		// Adjust screen tap sensitivity etc.
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		
		// Adjust screen tap sensitivity etc.
		controller.Config.SetFloat ("Gesture.KeyTap.MinDownVelocity", 20.0f);
		controller.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", 0.2f);
		controller.Config.SetFloat ("Gesture.KeyTap.MinDistance", 4.0f);


		// For circle gesture
		controller.EnableGesture(Leap.Gesture.GestureType.TYPECIRCLE);
		controller.Config.SetFloat("Gesture.Circle.MinRadius", 10.0f);
		controller.Config.SetFloat("Gesture.Circle.MinArc", 1.0f);


		controller.Config.Save();
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
	}

	void OnTriggerExit2D(Collider2D other)
	{
		timer.Stop ();
	}

	void OnTriggerStay2D(Collider2D other) 
	{
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands[0];

		if (hand.PinchStrength > 0.6f) 
		{
			print ("Selected: " + this.gameObject.name);
			if(!images.Contains(image))
			{
				print ("Added: " + this.gameObject.name);
				images.Add(image);
			}
		}

		if (frame.Gestures () [0].Type == Gesture.GestureType.TYPEKEYTAP) 
		{
			print ("Screen tap");
			timer.Reset();
		}

		// If nothing is selected and curosr is hovering over an image.
		if(images.Count == 0 && timer.ElapsedMilliseconds > 1500) 
		{
			float z = hand.Fingers[0].TipPosition.z > enterZ  ? 0.5f : -0.5f;
			transform.localScale = new Vector2(transform.localScale.x - z, transform.localScale.y - z);
		}
	}

	// Update is called once per frame
	void Update () {

		Frame frame = controller.Frame ();

		if (frame.Gestures () [0].Type == Gesture.GestureType.TYPEKEYTAP) 
		{
			print ("Screen tap");
			images.Clear ();
		}
		else if(frame.Gestures () [0].Type  == Gesture.GestureType.TYPECIRCLE) {
			CircleGesture circle = new CircleGesture(frame.Gestures () [0]);
			Vector centerPoint = circle.Center;
			Pointable circlePointable = circle.Pointable;
			
			if (circle.Pointable.Direction.AngleTo(circle.Normal) <= 3.1415/2) {
				print("CLOCKWISE CIRCLE");
				image.transform.Rotate(Vector3.back, 45 * Time.deltaTime);
			}
			else
			{
				print("COUNTERCLOCKWISE CIRCLE");
				image.transform.Rotate(Vector3.forward, 45 * Time.deltaTime);
			}
		}

		if(images.Contains(image))
			trackLeap (frame);
	}

	void trackLeap(Frame frame)
	{	
		int sep = images.IndexOf (image);
		float xOffset = transform.localScale.x * 1.4f;
		float yOffset = transform.localScale.y + 4.0f;
		// Cursor follow LeapMotion hand position.
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
		v.x = Mathf.Clamp (v.x, xOffset, panelWidth - xOffset);
		v.y = Mathf.Clamp (v.y + 4.0f, yOffset, UnityEngine.Screen.height - yOffset);
		
		Vector3 z = Camera.main.ScreenToWorldPoint (v);
		
		image.position = new Vector2 (z.x, (z.y + (sep * yOffset * 10f)));
	}
}
