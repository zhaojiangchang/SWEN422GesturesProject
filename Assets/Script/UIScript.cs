using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Leap;

public class UIScript : MonoBehaviour {
	
	public Canvas quitMenu;
	public Button mainMenuText;
	public Button trashText;
	public Controller controller;
	public int cursorSize = 25;
	// Use this for initialization
	void Start () 
	{
		quitMenu = quitMenu.GetComponent<Canvas> ();
		mainMenuText = mainMenuText.GetComponent<Button> ();
		trashText = trashText.GetComponent<Button> ();

		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		
		// Adjust screen tap sensitivity etc.
		controller.Config.SetFloat ("Gesture.KeyTap.MinDownVelocity", 20.0f);
		controller.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", 0.2f);
		controller.Config.SetFloat ("Gesture.KeyTap.MinDistance", 4.0f);
		controller.Config.Save();
		
		quitMenu.enabled = false;
	}
	
	void OnTriggerStay2D(Collider2D other) 
	{
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands[0];
		
		if (frame.Gestures () [0].Type == Gesture.GestureType.TYPEKEYTAP) 
		{
			print ("Screen tap");
		}
		
	}

	public void mainMenuPress()
	{
		quitMenu.enabled = true;
		mainMenuText.enabled = false;
		trashText.enabled = false;
	}
	
	public void NoPress()
	{
		quitMenu.enabled = false;
		mainMenuText.enabled = true;
		trashText.enabled = true;
	}
	
	public void mainMenuLevel()
	{
		Application.LoadLevel (0);
	}

	// Update is called once per frame
	void Update () {
		if (controller.IsConnected)
			trackHand();
	}
	
	void OnTriggerEnter(Collider other) {
		print ("collision");
	}
	
	void trackMouse()
	{	

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
