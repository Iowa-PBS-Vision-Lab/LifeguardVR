using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubjectSubmit : MonoBehaviour
{
    public TMP_Text idInfo;
    public TMP_Text handInfo;
    public TMP_Text ageInfo;
    public TMP_Text genderInfo;
    public GameObject MyPanel;

    //Functions to clean data up for the CSV.
    public string dataClean(string data){
        //Move all letters to uppercase.
        data = data.ToUpper();
        //Clean the data of any non-alphanumeric values.
        return Regex.Replace(data, "[^A-Z0-9 -]", "");
    }
    public void submitButton(){
        if(dataClean(idInfo.text) != "" && dataClean(ageInfo.text) != "" && dataClean(genderInfo.text) != "" && dataClean(handInfo.text) != ""){
            // Application.persistentDataPath points to %userprofile%\AppData\LocalLow\<companyname>\<productname>
            var path = Application.persistentDataPath + "/LIFEGUARDVR-SUBJECTINFO-" + idInfo.text + ".csv";
            Debug.Log(path);
            // Create a file to write to
            // Creates the titles for the log file
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(path))
            {
                //Write data to log.
                string tempUserID = dataClean(idInfo.text);
                PlayerPrefs.SetString("Hand", tempUserID);
                sw.WriteLine("Subject ID: ," + tempUserID);
                sw.WriteLine("Age: ," + dataClean(ageInfo.text));
                sw.WriteLine("Gender: ," + dataClean(genderInfo.text));
                sw.WriteLine("Handedness: ," + dataClean(handInfo.text));
                Debug.Log("Log file created");
            }
            //Disable the popup.
            MyPanel.SetActive(false);
            SceneManager.LoadScene("pool_test");
        }
        else{
            Debug.Log("Please fill out all fields.");
            MyPanel.GetComponent<Image>().color = new Color(0.4f, 0, 0);
        }
    }
}
