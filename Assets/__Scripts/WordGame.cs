﻿using System.Collections;
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
    [Header("Set in Inspector")]
    public GameObject prefabLetter;
    public Rect wordArea = new Rect(-24, 19,48, 28);
    public float letterSize = 1.5f;
    public bool showAllWyrds = true;

    [Header("Set Dynamically")]
    public GameMode mode = GameMode.preGame;
    public WordLevel currLevel;
    public List<Wyrd> wyrds;

    private Transform letterAnchor, bigLetterAnchor;

    void Awake(){
        S = this;
        letterAnchor = new GameObject("LetterAnchor").transform;
        bigLetterAnchor = new GameObject("BigLetterAnchor").transform;
    }

    // Start is called before the first frame update
    void Start(){
        mode = GameMode.loading;

        WordList.INIT();
    }

    public void WordListParseComplete(){
        mode = GameMode.makeLevel;
        currLevel = MakeWordLevel();
    }

    public WordLevel MakeWordLevel(int levelNum = -1){
        WordLevel level = new WordLevel();
        if(levelNum == -1){
            //pick random level
            level.longWordIndex = Random.Range(0, WordList.LONG_WORD_COUNT);
        } else{
            //will be added later
        }
        level.levelNum = levelNum;
        level.word = WordList.GET_LONG_WORD(level.longWordIndex);
        level.charDict = WordLevel.MakeCharDict(level.word);

        StartCoroutine(FindSubWordsCoroutine(level));
        return level;
    }

    public IEnumerator FindSubWordsCoroutine(WordLevel level){
        level.subWords = new List<string>();
        string str;

        List<string> words = WordList.GET_WORDS();

        for(int i = 0; i<WordList.WORD_COUNT; i++){
            str = words[i];

            if(WordLevel.CheckWordInLevel(str, level)){
                level.subWords.Add(str);
            }

            if(i%WordList.NUM_TO_PARSE_BEFORE_YIELD == 0){
                yield return null;
            }
        }

        level.subWords.Sort();
        level.subWords = SortWordsByLength(level.subWords).ToList();

        SubWordSearchComplete();
    }

    public static IEnumerable<string> SortWordsByLength(IEnumerable<string> ws){
        ws = ws.OrderBy(s => s.Length);
        return ws;
    }

    public void SubWordSearchComplete(){
        mode = GameMode.levelPrep;
        Layout();
    }

    void Layout(){

        wyrds = new List<Wyrd>();

        GameObject go;
        Letter lett;
        string word;
        Vector3 pos;
        float left = 0;
        float columnWidth = 3;
        char c;
        Color col;
        Wyrd wyrd;

        int numRows = Mathf.RoundToInt(wordArea.height/letterSize);

        //make a Wyrd of each level.subWord
        for(int i = 0; i<currLevel.subWords.Count; i++){
            wyrd = new Wyrd();
            word = currLevel.subWords[i];

            columnWidth = Mathf.Max(columnWidth, word.Length);

            for(int j = 0; j<word.Length; j++){
                c = word[j];
                go = Instantiate<GameObject>(prefabLetter);
                go.transform.SetParent(letterAnchor);
                lett = go.GetComponent<Letter>();
                lett.c = c;

                pos = new Vector3(wordArea.x+left+j*letterSize, wordArea.y, 0);

                pos.y -= (i%numRows)*letterSize;

                lett.pos = pos; //add more to this line later

                go.transform.localScale = Vector3.one*letterSize;

                wyrd.Add(lett);
            }
            if(showAllWyrds) wyrd.visible = true;

            wyrds.Add(wyrd);

            //adds new collumn if previous one is maxed
            if(i%numRows == numRows-1){
                left += (columnWidth+.5f) * letterSize;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
