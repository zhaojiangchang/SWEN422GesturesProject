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
		Random.seed = 42;
		selectedSidebarPanel (index);
		quitMenu = GameObject.Find ("QuitMenu").GetComponent<Canvas> ();
		mainMenuText = GameObject.Find ("Main Menu").GetComponent<Button> ();

		yesButton = GameObject.Find ("Yes").GetComponent<BoxCollider2D> ();
		noButton = GameObject.Find ("No").GetComponent<BoxCollider2D> ();

		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		
		// Adjust screen tap sensitivity etc.
		controller.Config.SetFloat ("Gesture.KeyTap.MinDownVelocity", 20.0f);
		controller.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", 0.2f);
		controller.Config.SetFloat ("Gesture.KeyTap.MinDistance", 4.0f);
		controller.Config.Save();
		
		quitMenu.enabled = false;

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
				toggleMenuButtons (true);
				mainMenuPress();
			}
			else if(this.gameObject.name.Equals("Yes") && quitMenu.enabled)
			{
				print ("Yes - Key tap");
				mainMenuLevel();
			}
			else if(this.gameObject.name.Equals("No") && quitMenu.enabled)
			{
				print ("No - Key tap");
				NoPress();
			}
			else if(this.gameObject.name.Equals("LoadImages"))
			{
				print ("LoadImages button - Key tap");
				loadImagesPress();
			}
		}
	}

	/// <summary>
	/// Toggles the menu buttons. Otherwise they will fire collision events even when not visible.
	/// </summary>
	/// <param name="enabled">If set to <c>true</c> enabled.</param>
	private void toggleMenuButtons(bool enabled)
	{
		yesButton.enabled = enabled;
		noButton.enabled = enabled;
	}

	public void mainMenuPress()
	{
		quitMenu.enabled = true;
		mainMenuText.enabled = false;
	}

	public void NoPress()
	{
		quitMenu.enabled = false;
		mainMenuText.enabled = true;
		toggleMenuButtons (false);
	}
	
	public void mainMenuLevel()
	{
		toggleMenuButtons (false);
		Application.LoadLevel (0);
	}
	// not work - GameObject.Find will not see deactivated once set to false check the solution
	public void loadImagesPress()
	{
		if (index == 4)
			index = 0;
		index++;
		selectedSidebarPanel(index);
	}

	public void selectedSidebarPanel(int index)
	{
		print ("SidebarSet" + index);
		print (GameObject.Find ("SidebarSet" + index)==null);

		GameObject.Find ("SidebarSet" + index).SetActive(true);
		for (int i = 1; i<5; i++) {
			if(i!=index){
				GameObject.Find ("SidebarSet"+i).SetActive(false);
				
			}
		}
		
	}
	/** solution -  i am not sure how to implement ......
	The technical answer to your question is yes you can, but unfortunately, GameObject.Find will not see deactivated 
	objects. If you want to be able to toggle it on and off at run time you will need to store it in a persistent variable, 
	try making GameObject pause a class variable, and either setting it in the editor, or doing the GameObject.Find in your 
	start function. Like this:
		
		GameObject pause;
	
	void Start()
	{
		pause = GameObject.Find("paused");
	}
	
	void someFunc()
	{
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.P))
		{
			if (Time.timeScale > 0)
			{
				pause.SetActive(true);
				
				Time.timeScale = 0;
			} else {
				pause.SetActive(false);
				
				Time.timeScale = 1;
			}
		}
	}
	**/
	void OnTriggerEnter(Collider other) {
		print ("collision");
	}
}
