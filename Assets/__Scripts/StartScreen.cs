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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void On7Button(){
        PlayerPrefs.SetInt("GameType", 7);
        SceneManager.LoadScene("Game");       
    }

    void On6Button(){
        PlayerPrefs.SetInt("GameType", 6);
        SceneManager.LoadScene("Game");
    }
}
