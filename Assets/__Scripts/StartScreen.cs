using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("CurrentScore", 0);
        PlayerPrefs.SetInt("CurrentTotal", 0);
        if(!PlayerPrefs.HasKey("HighScore6")){
            PlayerPrefs.SetInt("HighScore6", 150);
        }
        if(!PlayerPrefs.HasKey("HighScore7")){
            PlayerPrefs.SetInt("HighScore7", 150);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void On7Button(){
        PlayerPrefs.SetInt("GameType", 7);
        SceneManager.LoadScene("Game");       
    }

    public void On6Button(){
        PlayerPrefs.SetInt("GameType", 6);
        SceneManager.LoadScene("Game");
    }
}
