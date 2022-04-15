using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class saveGameFile : MonoBehaviour
{
    public string curUn;
    public int curPt;
    public int uCount;

    string txtDocumentName;
    string uN = "userName";
    string pT = "points";
    string end = "end";

    public Dictionary<string, int> highscores;
    public List<string> uN_list;
    public List<int> pT_list;

    string[] uFake = new string[] { "John Smith", "Amazing Amanda", "Chris Evans", "Pink Cheese" };
    int[] sFake = new int[] { 8, 3, 5, 2 };

    /// <summary>
    /// Prepwork for this to work:
    /// 
    ///You'd need to make this in your scene
    ///You'd also need to have a button or event that calls the void startSaveData()
    ///Also, please not the comments in the void Update()
    ///
    /// </summary>
    public Text userName, totalScore;

    // Start is called before the first frame update
    void Start()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath + "/Save_Data/");

        //userName = GameObject.Find("userName").GetComponent<Text>();
        //totalScore = GameObject.Find("totalScore").GetComponent<Text>();

    }

    public void startSaveData()
    {
        Debug.Log("is working");
        ///
        /// make a file pathway 
        /// then, if a save file doesn't exist, make a file
        /// insert fake data
        /// 

        txtDocumentName = Application.streamingAssetsPath + "/Save_Data/" + "SaveLog" + ".txt";

        if (!File.Exists(txtDocumentName))
        {
            //File.WriteAllText(txtDocumentName, "");
            for (int i = 0; i < uFake.Length; i++)
            {
                File.AppendAllText(txtDocumentName, (uN + i + "[") + uFake[i] + ("]" + pT + i + "[") + sFake[i] + ("]" + end + i) + "\n");
            }
        }

        ///
        /// Count the number of lines in the document
        /// number of lines to calc number of users
        /// 

        var lineCount = 0;
        using (var reader = File.OpenText(txtDocumentName))
        {
            while (reader.ReadLine() != null)
            {
                lineCount++;
            }
        }
        uCount = lineCount;

        ///
        /// Now we add the player to the text doc
        /// 

        File.AppendAllText(txtDocumentName, uN + uCount + "[" + curUn + "]" + pT + uCount + "[" + curPt + "]end" + uCount + "\n");
        uCount++;

        doesUserAlreadyExist();

    }
    public void doesUserAlreadyExist()
    {
        Debug.Log("does it exist");
        /// 
        /// username1[ example ]score1[ 123 ]end1
        /// determine if current player is among the saved data
        /// note, this for loop is basically the same thing thats in establishData
        /// 
        var search_uN = "";

        for (int i = 0; i < uCount; i++)
        {
            string a_string = uN + i + "[";
            string b_string = "]" + pT + i + "[";

            int Pos1 = File.ReadAllText(txtDocumentName).IndexOf(a_string) + a_string.Length;
            int Pos2 = File.ReadAllText(txtDocumentName).IndexOf(b_string);
            int Pos3 = File.ReadAllText(txtDocumentName).IndexOf(b_string) + b_string.Length;

            search_uN = File.ReadAllText(txtDocumentName).Substring(Pos1, Pos2 - Pos1);
        }

        if (curUn == search_uN)
        {
            establishSaveData();
            //return;
        }
        else if (curUn != search_uN)
        {
            Debug.Log("WARNING there is a player with this name already");
            establishSaveData();

        }
        //establishSaveData();

    }

    public void establishSaveData()
    {
        Debug.Log("youre establishing");

        string data_uN;
        string data_sA;

        ///
        /// calculate the dictionary variable
        /// Note, for the dictionary to not have error if there is a duplicate username
        /// We add the filler i_
        /// Later on we can find the underscore
        /// get what character index number the underscore is located
        /// Then make a substring where the textbox will start whereever character index is after _
        /// The old code is commented out if you'd like to work with the original version
        /// 

        highscores = new Dictionary<string, int>(uCount);

        for (int i = 0; i < uCount; i++)
        {
            //string a_string = uN + i + "[";
            string a_string = uN + i + "[";
            string b_string = "]" + pT + i + "[";
            string c_string = "]" + end + i;

            int Pos1 = File.ReadAllText(txtDocumentName).IndexOf(a_string) + a_string.Length;
            int Pos2 = File.ReadAllText(txtDocumentName).IndexOf(b_string);
            int Pos3 = File.ReadAllText(txtDocumentName).IndexOf(b_string) + b_string.Length;
            int Pos4 = File.ReadAllText(txtDocumentName).IndexOf(c_string);

            data_uN = File.ReadAllText(txtDocumentName).Substring(Pos1, Pos2 - Pos1);
            data_sA = File.ReadAllText(txtDocumentName).Substring(Pos3, Pos4 - Pos3);

            highscores.Add(i + "_" + data_uN, int.Parse(data_sA));
            //highscores.Add(data_uN, int.Parse(data_sA));

        }
        ///
        /// dictionary has a string and int variable. 
        /// strings are a Key and numbers are Value
        /// basically we are reordering the ditionary based on Value from ascending/descending order
        /// 

        highscores = highscores.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        var sortedDict = from entry in highscores orderby entry.Value ascending select entry;

        ///
        /// dictionaries are a little tricky to control so the purpose here
        /// is to set it's values, after being ordered
        /// to a list which is easier to control
        /// 

        uN_list = new List<string>(uCount);
        pT_list = new List<int>(uCount);
        foreach (KeyValuePair<string, int> hs in highscores)
        {
            uN_list.Add(hs.Key);
            pT_list.Add(hs.Value);
        }

        printToHighScores();
    }
    public void printToHighScores()
    {
        Debug.Log("you made it");

        userName.text = "";
        totalScore.text = "";

        ///
        /// Rememer how we added filler to the dictionary variable?
        /// Now we look for this filler.
        /// Per word, aka the uN_list
        /// We look at each character
        /// the max value for the for loop x is the length of the word
        /// if the character is in the for loop is _
        /// then get the x value 
        /// then change uN_list so it no longer includes the filler via substring
        /// substring tells where to begin the string again in this situation
        /// 


        for (int i = 0; i < uN_list.Capacity; i++)
        {
            for (int x = 0; x < uN_list[i].Length; x++)
            {
                char c = uN_list[i][x];
                string cToS = c.ToString();
                if (cToS == "_")
                {
                    uN_list[i] = uN_list[i].Substring(x + 1);
                }

            }
            //string finalUN= 
            userName.text = userName.text + uN_list[i] + "\n";
            totalScore.text = totalScore.text + pT_list[i] + "\n";

        }
    }


    // Update is called once per frame
    void Update()
    {
        ///
        /// You need to get the user name from somewhere
        /// as well as a score
        /// 

        curUn = UsernameInput.newusername;
        if(curUn == null)
        {
            curUn = "Placeholder Name";
        }
        curPt = Random.Range(0,7);

        //curUn = saveFileVers2.playerName;
        //curPt = saveFileVers2.playerScore;
    }
}
