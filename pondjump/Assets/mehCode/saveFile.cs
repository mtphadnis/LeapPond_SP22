using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class saveFile : MonoBehaviour
{
    public InputField inputName;
    public GameObject inputArea;
    public Text scoreboardTxt;

    string txtDocumentName;

    int maxLetters = 20;

    //what will be printed in text file
    string uN = "userName";
    string sA = "scoreA";
   
    string curUser;
    int curScore;
    
    int userCount;

    public Dictionary<string, int> highScores;
    public List<string> uN_list;
    public List<int> sA_list;

    string[] uFake = new string[] { "John Smith", "Amazing Amanda", "Chris Evans", "Pink Cheese" };
    int[] sFake = new int[] { 8, 3, 5, 2 };

    void Start()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath + "/Save_Data/");
        inputName = GameObject.Find("InputField").GetComponent<InputField>();

        inputArea = GameObject.Find("inputArea");
        scoreboardTxt = GameObject.Find("scoreboardTxt").GetComponent<Text>();



    }

    public void submitName()
    {
        inputName.characterLimit = maxLetters;
        if (inputName.text == "") { return; }
        else { CreateText(); }

    }
    public void CreateText()
    {

        txtDocumentName = Application.streamingAssetsPath + "/Save_Data/" + "SaveLog" + ".txt";

        if (!File.Exists(txtDocumentName))
        {
            for (int i = 0; i < uFake.Length; i++)
            {
                //If there is no save file, make one fill it up with fake users
                //Username equals:
                //userName[example]score0[123]end0
                //\n
                File.AppendAllText(txtDocumentName, (uN + i + "[") + uFake[i] + ("]" + sA + i + "[") + sFake[i] + ("]end" + i) + "\n");
            }
        }
            //When the user types something in the inupt field, make this the name
            //for now we're making up a random score
            curUser = inputName.text;
            curScore = Random.Range(0, 9);
        

        //For every line in this document represent a user. So count the lines and you have number of users
        var lineCount = 0;
        using (var reader = File.OpenText(txtDocumentName))
        {
            while (reader.ReadLine() != null)
            {
                lineCount++;
            }
        }
        //Debug.Log(lineCount);
        userCount = lineCount;


        /**
        
        //Check to see if this is a new player or not
        string tosearch="";
        for(int i = 0; i < userCount; i++)
        {
            string a_string = uN + i + "[";
            string b_string = "]" + sA + i ;

            int Pos1 = File.ReadAllText(txtDocumentName).IndexOf(a_string) + a_string.Length;
            int Pos2 = File.ReadAllText(txtDocumentName).IndexOf(b_string);

            tosearch = File.ReadAllText(txtDocumentName).Substring(Pos1, Pos2 - Pos1);
        }
        if (curUser == tosearch)
        {
            Debug.Log("Welcome back " + curUser);
        }
        else if(curUser != tosearch)
        {
            Debug.Log(curUser + "is a new player!");
            File.AppendAllText(txtDocumentName, uN + userCount + "[" + curUser + "]" + sA + userCount + "[" + curScore + "]end" + userCount + "\n");
            userCount++;
        }
        **/
        File.AppendAllText(txtDocumentName, uN + userCount + "[" + curUser + "]" + sA + userCount + "[" + curScore + "]end" + userCount + "\n");
        userCount++;

        GetUserData();
        inputName.text = "";
        
    }
    public void GetUserData()
    {
        string data_uN;
        string data_sA;

        highScores = new Dictionary<string,int>(userCount);

        for (int i = 0; i < userCount; i++)
        {

            string a_string = uN + i + "[";
            string b_string = "]" + sA + i + "[";
            string c_string = "]end" + i;

            int Pos1 = File.ReadAllText(txtDocumentName).IndexOf(a_string) + a_string.Length;
            int Pos2 = File.ReadAllText(txtDocumentName).IndexOf(b_string);
            int Pos3 = File.ReadAllText(txtDocumentName).IndexOf(b_string) + b_string.Length;
            int Pos4 = File.ReadAllText(txtDocumentName).IndexOf(c_string);

            data_uN = File.ReadAllText(txtDocumentName).Substring(Pos1, Pos2 - Pos1);
            data_sA = File.ReadAllText(txtDocumentName).Substring(Pos3, Pos4 - Pos3);
          
            
            highScores.Add(data_uN, int.Parse(data_sA));
        }

        //this reorders the scores. Note the word ascending or descending!
        highScores = highScores.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        var sortedDict = from entry in highScores orderby entry.Value ascending select entry;

        //now let's make our username list and scores equal the order of the highscores
        uN_list = new List<string>(userCount);
        sA_list = new List<int>(userCount);

        foreach (KeyValuePair<string, int> hs in highScores)
        {
            uN_list.Add(hs.Key);
            sA_list.Add(hs.Value);

            //Now you can access the key and value both separately from this attachStat as:
            Debug.Log(hs.Key + " and " + hs.Value);
        }
        scoreboardTxt.text = "";
        showResults();
    }
    
    public void showResults()
    {
        for(int i = 0; i < userCount; i++)
        {
            scoreboardTxt.text = scoreboardTxt.text + sA_list[i] + " ----- " + uN_list[i] + "\n";
        }
    }

    public void Update()
    {
        
    }
}