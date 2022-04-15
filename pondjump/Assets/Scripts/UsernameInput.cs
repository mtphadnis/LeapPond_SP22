using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class UsernameInput : MonoBehaviour
{
    public GameObject inputbox;
    public GameObject popup;
    public Button yes;
    public Button no;
    public Text popuptext;
    public static string newusername;
    public InputField inputfield;


    // Start is called before the first frame update
    void Start()
    {
        inputfield = inputbox.GetComponent<InputField>();
        popup.SetActive(false);




    }

    public void PopupOn()
    {
        if (inputfield.text != "" )
        {
            popup.SetActive(true);
            inputbox.SetActive(false);
            popuptext.text = "Hello" + inputfield.text + ". Would you like to play?";
        }

    }

    public void SubmitName()
    {
    
        newusername = inputfield.text;


    }

    public void CancelName()
    {
        popup.SetActive(false );
        inputbox.SetActive(true);


    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
