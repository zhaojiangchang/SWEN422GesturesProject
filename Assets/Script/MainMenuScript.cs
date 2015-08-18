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

		//quitMenu.enabled = false;
	}

	void OnTriggerStay2D(Collider2D other) 
	{	
		//print ("colliding");
		Frame frame = controller.Frame ();
		Hand hand = frame.Hands[0];
	
		if (frame.Gestures () [0].Type == Gesture.GestureType.TYPEKEYTAP) 
		{
			print (frame.Gestures () [0].Type == Gesture.GestureType.TYPEKEYTAP);
			print (this.gameObject.name);

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

	public void ExitPress()
	{
		startText.enabled = false;
		exitText.enabled = false;
		ExitGame ();
	}

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
