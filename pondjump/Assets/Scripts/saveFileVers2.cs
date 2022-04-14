using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class saveFileVers2 : MonoBehaviour
{
    GameObject startMenu, theGame, highScores, InputFieldBox, yesNoPopup;
    InputField InputName;
    Text playerScoreTxt, displayNameTxt, popupTxt;

    public static string playerName;
    public static int playerScore;

    public string[] userListings;
    public int[] pointListings;

    // Start is called before the first frame update
    void Start()
    {
        startMenu = GameObject.Find("startMenu");
        theGame = GameObject.Find("theGame");
        highScores = GameObject.Find("highScores");
        InputFieldBox = GameObject.Find("InputField");
        yesNoPopup = GameObject.Find("yesNoPopup");

        playerScoreTxt = GameObject.Find("playerScoreTxt").GetComponent<Text>();
        displayNameTxt = GameObject.Find("displayNameTxt").GetComponent<Text>();
        popupTxt = GameObject.Find("popupTxt").GetComponent<Text>();

        InputName = InputFieldBox.GetComponent<InputField>();

        startMenu.SetActive(true);
        theGame.SetActive(false);
        highScores.SetActive(false);
        InputFieldBox.SetActive(false);
        yesNoPopup.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pressPlay()
    {
        InputFieldBox.SetActive(true);
        //Debug.Log("you hit start");
    }
    public void submitName()
    {
        if (InputName.text != "")
        {
            yesNoPopup.SetActive(true);
            popupTxt.text = "Hello " + InputName.text + ". Start a new game?";
            InputFieldBox.SetActive(false);
        }
    }
    public void yesBtn()
    {
        playerName = InputName.text;

        theGame.SetActive(true);
        displayNameTxt.text = playerName;
        startMenu.SetActive(false);
    }
    public void noBtn()
    {
        yesNoPopup.SetActive(false);   
    }
    public void showScores()
    {
        highScores.SetActive(true);
        theGame.SetActive(false);
    }
    public void addBtn()
    {
        playerScore++;
        playerScoreTxt.text = playerScore.ToString();
    }
    public void minBtn()
    {
        if (playerScore == 0){return;}
        else { playerScore--; }
        playerScoreTxt.text = playerScore.ToString();

    }
    public void backBtn()
    {
        theGame.SetActive(true);
        displayNameTxt.text = playerName;
        highScores.SetActive(false);
    }
    public void menuBtn()
    {
        startMenu.SetActive(true);
        theGame.SetActive(false);
        InputFieldBox.SetActive(false);
        yesNoPopup.SetActive(false);
        highScores.SetActive(false);
    }
}
