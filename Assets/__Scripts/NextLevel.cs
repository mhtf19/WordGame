using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    void Awake(){
        GameObject.Find("Button").GetComponentInChildren<Text>().text = "Leave";
        PlayerPrefs.SetInt("ShowNextLevelButton", 0); 
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetInt("ShowNextLevelButton") == 1){
            GameObject.Find("Button").GetComponentInChildren<Text>().text = "Next Level";
        }
    }

    public void OnPress(){
        if(PlayerPrefs.GetInt("ShowNextLevelButton")==1){
            Next();
        }
        else{
            if(PlayerPrefs.GetInt("GameType")==7){
                if(PlayerPrefs.GetInt("CurrentScore") > PlayerPrefs.GetInt("HighScore7")){
                    PlayerPrefs.SetInt("HighScore7", PlayerPrefs.GetInt("CurrentScore"));
                }
            }
            if(PlayerPrefs.GetInt("GameType")==6){
                if(PlayerPrefs.GetInt("CurrentScore") > PlayerPrefs.GetInt("HighScore6")){
                    PlayerPrefs.SetInt("HighScore6", PlayerPrefs.GetInt("CurrentScore"));
                }
            }
            Leave();
        }
    }

    void Next(){
        SceneManager.LoadScene("Game"); 
    }

    void Leave(){
        SceneManager.LoadScene("Start"); 
    }

}
