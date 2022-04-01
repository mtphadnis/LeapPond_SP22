using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpManager : MonoBehaviour
{
    public GameObject PopUpCanvas;
    TextMeshProUGUI popUpText;
    TextMeshProUGUI popUpImageText;
    Image popUpImage;
    string _recievedText;
    GameObject previousPopUp;

    // Start is called before the first frame update
    void Start()
    {
        popUpText = GameObject.Find("RecievedText (TMP)").GetComponent<TextMeshProUGUI>();

        PopUpCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "PopUpText")
        {
            PopUpCanvas.SetActive(true);
            _recievedText = other.GetComponent<PopUp>().DeliveryText;
            Debug.Log(_recievedText);
            Debug.Log(popUpText);

            popUpText.SetText(_recievedText);

            Time.timeScale = 0;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            other.gameObject.SetActive(false);
        }
        else if(other.transform.tag == "PopUpImage")
        {

        }
    }

    public void Continue()
    {
        PopUpCanvas.SetActive(false);
        Time.timeScale = 1;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
