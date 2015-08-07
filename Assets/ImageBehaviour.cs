using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class ImageBehaviour : MonoBehaviour {

	Controller controller;
	private static List<Rigidbody2D> images = new List<Rigidbody2D>();
	Rigidbody2D image;

	// Use this for initialization
	void Start () {
		controller = new Controller ();
		image = this.GetComponent<Rigidbody2D> ();

		// Adjust screen tap sensitivity etc.
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		
		// Adjust screen tap sensitivity etc.
		controller.Config.SetFloat ("Gesture.KeyTap.MinDownVelocity", 10.0f);
		controller.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", 0.2f);
		controller.Config.SetFloat ("Gesture.KeyTap.MinDistance", 1.0f);
		controller.Config.Save();
	}

	void OnTriggerStay2D(Collider2D other) 
	{
		Frame frame = controller.Frame ();
		if (frame.Hands [0].PinchStrength > 0.6f) 
		{
			print ("Selected: " + this.gameObject.name);
			if(!images.Contains(image))
			{
				print ("Added: " + this.gameObject.name);
				images.Add(image);
			}
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
		if(images.Contains(image))
			trackLeap (frame);
	}

	void trackLeap(Frame frame)
	{	
		int sep = images.IndexOf (image);
		int offset = 50;
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
		v.x = Mathf.Clamp (v.x, offset, UnityEngine.Screen.width - offset);
		v.y = Mathf.Clamp (v.y, offset, UnityEngine.Screen.height - offset);
		
		Vector3 z = Camera.main.ScreenToWorldPoint (v);
		
		image.position = new Vector2 (z.x, (z.y + (sep * 500)));
	}
}
