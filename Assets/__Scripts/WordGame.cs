using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GameMode {
    preGame, //before game
    loading,
    makeLevel,
    levelPrep,
    inLevel
}

public class WordGame : MonoBehaviour
{
    static public WordGame S;

    [Header("Set Dynamically")]
    public GameMode mode = GameMode.preGame;

    void Awake(){
        S = this;
    }
    // Start is called before the first frame update
    void Start(){
        mode = GameMode.loading;

        WordList.INIT();
    }

    public void WordListParseComplete(){
        mode = GameMode.makeLevel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
