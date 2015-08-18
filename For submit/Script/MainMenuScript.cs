/**
 * This class is for Main menu with play and exit button
 * Main menu: (control by mouse or Leap Motion)
 * 		Mouse: Single click on left key
 * 		Leap Motion: KEYTAP on button to action the button script
 * Level 0 : Display Main Menu
 * Level 1: Display Game screen
 * **/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Leap;

public class MainMenuScript : MonoBehaviour {

	private Canvas quitMenu;
	private Button startText;
	private Button exitText;
	public Controller controller;
	public int cursorSize = 25;

	// Use this for initialization
	void Start () 
	{
		//quitMenu = GameObject.Find ("QuitMenu").GetComponent<Canvas> ();
		startText = GameObject.Find("Play").GetComponent<Button> ();
		exitText = GameObject.Find("Exit").GetComponent<Button> ();
		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);

		// Adjust screen tap sensitivity etc.
		controller.Config.SetFloat ("Gesture.KeyTap.MinDownVelocity", 20.0f);
		controller.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", 0.2f);
		controller.Config.SetFloat ("Gesture.KeyTap.MinDistance", 4.0f);
		controller.Config.Save();
	}
	//Trigger the gesture when hand stay on the object
	void OnTriggerStay2D(Collider2D other) 
	{	
		print ("colliding");
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands[0];
	
		if (frame.Gestures () [0].Type == Gesture.GestureType.TYPEKEYTAP) 
		{
			print (frame.Gestures () [0].Type == Gesture.GestureType.TYPEKEYTAP);
			print (this.gameObject.name);
			// Check gameobject's name when hand over the object
			if(this.gameObject.name.Equals("Play"))
			{
				print ("Play - Key tap");
				StartLevel();
			}
			if(this.gameObject.name.Equals("Exit"))
			{
				print ("Exit - Key tap");
				ExitPress();
			}
		}
	}

	/**
	 * Exit button pressed
	 * Exit game
	 * */
	public void ExitPress()
	{
		ExitGame ();
	}
	/**
	 * Play button pressed
	 * Enter the game
	 * */
	public void StartLevel()
	{
		Application.LoadLevel (1);
	}

	public void ExitGame()
	{
		Application.Quit ();
	}

	void OnTriggerEnter(Collider other) 
	{
		print ("collision");
	}

}
