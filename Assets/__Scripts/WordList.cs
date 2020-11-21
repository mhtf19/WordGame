using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordList : MonoBehaviour
{
    private static WordList S;

    [Header("Set in the Inspector")]
    public TextAsset wordListText;
    public int numToParseBeforeYield = 1000;
    public int wordLengthMin = 3;
    

    [Header("Set Dynamically")]
    public int currLine = 0;
    public int totalLines;
    public int longWordCount;
    public int wordCount;
    public int wordLengthMax;

    //Private fields
    private string[] lines;
    private List<string> longWords;
    private List<string> words;

    void Awake(){
        S = this;
        if(!PlayerPrefs.HasKey("GameType")){
            PlayerPrefs.SetInt("GameType", 7);
            wordLengthMax = 7;
        } else if(PlayerPrefs.GetInt("GameType") != 7 && PlayerPrefs.GetInt("GameType") != 6){
            PlayerPrefs.SetInt("GameType", 7);
            wordLengthMax = 7;
        } else{
            wordLengthMax = PlayerPrefs.GetInt("GameType");
        }
    }

    
    public void Init(){
        lines = wordListText.text.Split('\n');
        totalLines = lines.Length;

        StartCoroutine(ParseLines());
    }
    
    static public void INIT(){
        S.Init();
    }

    //All coroutines have IEnumerator as return type
    public IEnumerator ParseLines(){
        string word;

        //initialize lists
        longWords = new List<string>();
        words = new List<string>();

        for(currLine = 0; currLine < totalLines; currLine++){
            word = lines[currLine];

            //if the word's length is the max length
            if(word.Length == wordLengthMax){
                longWords.Add(word);
            }

            //if word is in range
            if(word.Length>=wordLengthMin && word.Length <=wordLengthMax){
                words.Add(word);
            }

            //determine if coroutine should yield
            if(currLine % numToParseBeforeYield == 0){
                //count the words in each list to show parsing progress
                longWordCount = longWords.Count;
                wordCount = words.Count;
                //this yields execution until next frame
                
                
                yield return null;

                //the yield will cause the execution of this method to wait
                //until other code executes then continue where it left off
            }
        }

        longWordCount = longWords.Count;
        wordCount = words.Count;

        gameObject.SendMessage("WordListParseComplete");
    }

    static public List<string> GET_WORDS(){
        return (S.words);
    }

    static public string GET_WORD(int ndx){
        return(S.words[ndx]);
    }

    static public List<string> GET_LONG_WORDS(){
        return(S.longWords);
    }

    static public string GET_LONG_WORD(int ndx){
        return(S.longWords[ndx]);
    }

    static public int WORD_COUNT{
        get {return S.wordCount;}
    }

    static public int LONG_WORD_COUNT{
        get {return S.longWordCount;}
    }

    static public int NUM_TO_PARSE_BEFORE_YIELD{
        get {return S.numToParseBeforeYield;}
    }

    static public int WORD_LENGTH_MIN{
        get {return S.wordLengthMin;}
    }

    static public int WORD_LENGTH_MAX{
        get {return S.wordLengthMax;}
    }

    // Update is called once per frame
    void Update(){
        
    }
}
