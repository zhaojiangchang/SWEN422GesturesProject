using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Leap;

public class UIScript : MonoBehaviour {
	
	public Canvas quitMenu;
	public Canvas trashMenu;
	public Button mainMenuText;
	public Button trashText;
	public Controller controller;
	public int cursorSize = 25;
	// Use this for initialization
	void Start () 
	{
		quitMenu = quitMenu.GetComponent<Canvas> ();
		trashMenu = trashMenu.GetComponent<Canvas> ();

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
		trashMenu.enabled = false;
	}
	
	void OnTriggerStay2D(Collider2D other) 
	{
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands[0];
		
		if (frame.Gestures () [0].Type == Gesture.GestureType.TYPEKEYTAP) 
		{
			
			if(this.gameObject.name.Equals("Main Menu"))
			{
				print ("Main Menu - Key tap");
				mainMenuPress();
			}
			if(this.gameObject.name.Equals("Trash"))
			{
				print ("Trash - Key tap");
				trashPress();
			}
			if(this.gameObject.name.Equals("Yes"))
			{
				print ("Yes - Key tap");
				mainMenuLevel();
			}
			if(this.gameObject.name.Equals("No"))
			{
				print ("No - Key tap");
				NoPress();
			}
			if(this.gameObject.name.Equals("Confirm"))
			{
				print ("Confirm - Key tap");
				ConfirmPress();
			}
			if(this.gameObject.name.Equals("Reject"))
			{
				print ("Reject - Key tap");
				RejectPress();
			}
		}
		
	}
	
	public void mainMenuPress()
	{
		print ("mainmenu pressed");
		quitMenu.enabled = true;
		mainMenuText.enabled = false;
		trashText.enabled = false;
	}

	public void trashPress()
	{
		print ("trash pressed");
		trashMenu.enabled = true;
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

	public void ConfirmPress()
	{
		print ("confirm pressed"+ ImageBehaviour.images.Count);
		ImageBehaviour.images.Clear ();
		trashMenu.enabled = false;
		mainMenuText.enabled = true;
		trashText.enabled = true;	
	}
	public void RejectPress()
	{
		print ("reject pressed");
		trashMenu.enabled = false;
		mainMenuText.enabled = true;
		trashText.enabled = true;
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
