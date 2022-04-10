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

        postToSaveData();
    }
    public void postToSaveData()
    {
        /// 
        /// username1[ example ]score1[ 123 ]end1
        /// determine if current player is among the saved data
        /// note, this for loop is basically the same thing thats in establishData
        /// 
        var search_uN = "";
        var search_pT = "";

        for (int i = 0; i < uCount; i++)
        {
            string a_string = uN + i + "[";
            string b_string = "]" + pT + i + "[";
            string c_string = "]" + end + i;

            int Pos1 = File.ReadAllText(txtDocumentName).IndexOf(a_string) + a_string.Length;
            int Pos2 = File.ReadAllText(txtDocumentName).IndexOf(b_string);
            int Pos3 = File.ReadAllText(txtDocumentName).IndexOf(b_string) + b_string.Length;
            int Pos4 = File.ReadAllText(txtDocumentName).IndexOf(c_string);

            search_uN = File.ReadAllText(txtDocumentName).Substring(Pos1, Pos2 - Pos1);
            search_pT = File.ReadAllText(txtDocumentName).Substring(Pos3, Pos4 - Pos3);

            /**
             * 
            if (search_uN == curUn)
            {
                search_uN = search_uN.Replace(search_uN, curUn);
                search_pT = search_pT.Replace(search_pT, curPt.ToString());
            }
            else if (search_uN != curUn)
            {
                File.AppendAllText(txtDocumentName, uN + uCount + "[" + curUn + "]" + pT + uCount + "[" + curPt + "]end" + uCount + "\n");
                uCount++;

            }
            Debug.Log(search_uN);
            */
        }
        if (curUn == search_uN)
        {
            return;
        }
        else if (curUn != search_uN)
        {
            File.AppendAllText(txtDocumentName, uN + uCount + "[" + curUn + "]" + pT + uCount + "[" + curPt + "]end" + uCount + "\n");
            uCount++;

        }


        establishSaveData();
    }

    public void establishSaveData()
    {
        string data_uN;
        string data_sA;

        ///
        /// calculate the dictionary variable

        highscores = new Dictionary<string, int>(uCount);

        for (int i = 0; i < uCount; i++)
        {
            string a_string = uN + i + "[";
            string b_string = "]" + pT + i + "[";
            string c_string = "]" + end + i;

            int Pos1 = File.ReadAllText(txtDocumentName).IndexOf(a_string) + a_string.Length;
            int Pos2 = File.ReadAllText(txtDocumentName).IndexOf(b_string);
            int Pos3 = File.ReadAllText(txtDocumentName).IndexOf(b_string) + b_string.Length;
            int Pos4 = File.ReadAllText(txtDocumentName).IndexOf(c_string);

            data_uN = File.ReadAllText(txtDocumentName).Substring(Pos1, Pos2 - Pos1);
            data_sA = File.ReadAllText(txtDocumentName).Substring(Pos3, Pos4 - Pos3);

            highscores.Add(data_uN, int.Parse(data_sA));
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
    }
    public void printToHighScores()
    {
        userName.text = "";
        totalScore.text = "";

        for (int i = 0; i < uN_list.Capacity; i++)
        {
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

        curUn = "sirHolderofPlaces";
        curPt = Random.Range(0,7);

        //curUn = saveFileVers2.playerName;
        //curPt = saveFileVers2.playerScore;
    }
}
