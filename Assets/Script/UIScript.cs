using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Leap;
using UnityEngine;

public class UIScript : MonoBehaviour {

	private static bool initialized;
	private static Canvas quitMenu;
	private static Button mainMenuText;
	private static BoxCollider2D yesButton;
	private static BoxCollider2D noButton;
	private static Controller controller;
	public static int cursorSize = 25;
	public static int index = 1;

	// Use this for initialization
	void Start () 
	{
		// Only run this once.
		if (initialized)
			return;
		mainMenuText = GameObject.Find ("Main Menu").GetComponent<Button> ();


		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		
		// Adjust screen tap sensitivity etc.
		controller.Config.SetFloat ("Gesture.KeyTap.MinDownVelocity", 20.0f);
		controller.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", 0.2f);
		controller.Config.SetFloat ("Gesture.KeyTap.MinDistance", 4.0f);
		controller.Config.Save();

		initialized = true;
	}
	
	void OnTriggerStay2D(Collider2D other) 
	{
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands[0];
		
		if (frame.Gestures () [0].Type == Gesture.GestureType.TYPEKEYTAP) 
		{	print (other == null);
			print (this.gameObject == null);
			if(this.gameObject.name.Equals("Main Menu"))
			{
				print ("Main Menu - Key tap");
				mainMenuPress();
			}
		}
	}

	public void mainMenuPress()
	{
		Application.LoadLevel (0);

	}


	void OnTriggerEnter(Collider other) {
		print ("collision");
	}
}
