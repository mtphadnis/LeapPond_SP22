using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpManager : MonoBehaviour
{
    public GameObject PopUpTextCanvas;
    public GameObject PopUpImageCanvas;
    TextMeshProUGUI popUpText;
    TextMeshProUGUI popUpImageText;
    string _recievedText;
    Sprite _recievedImage;
    bool textOnly;

    public int Collection;

    // Start is called before the first frame update
    void Start()
    {
        popUpText = GameObject.Find("RecievedText(T) (TMP)").GetComponent<TextMeshProUGUI>();
        popUpImageText = GameObject.Find("RecievedText(I) (TMP)").GetComponent<TextMeshProUGUI>();

        PopUpTextCanvas.SetActive(false);
        PopUpImageCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "PopUpText")
        {
            textOnly = true;
            PopUpTextCanvas.SetActive(true);
            _recievedText = other.GetComponent<PopUp>().DeliveryText;

            popUpText.SetText(_recievedText);

            Time.timeScale = 0;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            other.gameObject.SetActive(false);
        }
        else if(other.transform.tag == "PopUpImage")
        {
            textOnly = false;
            PopUpImageCanvas.SetActive(true);
            _recievedText = other.GetComponent<PopUp>().DeliveryText;
            _recievedImage = other.GetComponent<PopUp>().DeliveryImage;

            popUpImageText.SetText(_recievedText);
            GameObject.Find("Image(I)").GetComponent<Image>().sprite = _recievedImage;


            Time.timeScale = 0;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            other.gameObject.SetActive(false);
        }
        else if(other.transform.tag == "Collectable")
        {
            Collection++;
            other.gameObject.SetActive(false);
            Debug.Log("Score: " + Collection);
        }
    }

    public void Continue()
    {
        if (textOnly)
        { PopUpTextCanvas.SetActive(false); } else { PopUpImageCanvas.SetActive(false); }
        Time.timeScale = 1;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
