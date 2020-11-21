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
    [Header("Set in Inspector")]
    public GameObject prefabLetter;
    public Rect wordArea = new Rect(-24, 19,48, 28);
    public float letterSize = 1.5f;
    public bool showAllWyrds = false;
    public float bigLetterSize = 4f;
    public Color bigColorDim = new Color(.8f, .8f, .8f);
    public Color bigColorSelected = new Color(1f, .9f, .7f);
    public Vector3 bigLetterCenter = new Vector3(0, -16, 0);
    public Color[] wyrdPalette;

    [Header("Set Dynamically")]
    public GameMode mode = GameMode.preGame;
    public WordLevel currLevel;
    public List<Wyrd> wyrds;
    public List<Letter> bigLetters;
    public List<Letter> bigLettersActive;
    public string testWord;
    private string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

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

                lett.posImmediate = pos+Vector3.up*(20+i%numRows);

                lett.pos = pos; //add more to this line later

                lett.timeStart = Time.time + i*.05f;

                go.transform.localScale = Vector3.one*letterSize;

                wyrd.Add(lett);
            }
            if(showAllWyrds) wyrd.visible = true;

            wyrd.color = wyrdPalette[word.Length-WordList.WORD_LENGTH_MIN];
            wyrds.Add(wyrd);

            //adds new collumn if previous one is maxed
            if(i%numRows == numRows-1){
                left += (columnWidth+.5f) * letterSize;
            }
        }

        bigLetters = new List<Letter>();
        bigLettersActive = new List<Letter>();

        //create a big letter for each letter in big word
        for(int i = 0; i<currLevel.word.Length; i++){
            c = currLevel.word[i];
            go = Instantiate<GameObject>(prefabLetter);
            go.transform.SetParent(bigLetterAnchor);
            lett = go.GetComponent<Letter>();
            lett.c = c;
            go.transform.localScale = Vector3.one*bigLetterSize;

            //set initial position of big letters below the screen

            pos = new Vector3(0, -100, 0);

            lett.posImmediate = pos;
            lett.pos = pos; //will add code here later

            lett.timeStart = Time.time + currLevel.subWords.Count*.05f;
            lett.easingCurve = Easing.Sin+"-0.18";

            col = bigColorDim;
            lett.color = col;
            lett.visible = true;
            lett.big = true;
            bigLetters.Add(lett);
        }

        //shuffle letters
        bigLetters = ShuffleLetters(bigLetters);

        //arrange onto screen
        ArrangeBigLetters();

        mode = GameMode.inLevel;
    }

    List<Letter> ShuffleLetters(List<Letter> letts){
        List<Letter> newL = new List<Letter>();
        int ndx;
        while(letts.Count > 0){
            ndx = Random.Range(0, letts.Count);
            newL.Add(letts[ndx]);
            letts.RemoveAt(ndx);
        }
        return newL;
    }

    void ArrangeBigLetters(){
        float halfWidth = ((float) bigLetters.Count)/2f - .5f;
        Vector3 pos;
        for(int i = 0; i<bigLetters.Count; i++){
            pos = bigLetterCenter;
            pos.x += (i-halfWidth)*bigLetterSize;
            bigLetters[i].pos = pos;
        }

        halfWidth = ((float) bigLettersActive.Count)/2f-.5f;
        for(int i = 0; i<bigLettersActive.Count; i++){
            pos = bigLetterCenter;
            pos.x += (i-halfWidth)*bigLetterSize;
            pos.y += bigLetterSize*1.25f;
            bigLettersActive[i].pos = pos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Letter ltr;
        char c;

        switch (mode){
            case GameMode.inLevel:
                foreach (char cIt in Input.inputString){
                    c = System.Char.ToUpperInvariant(cIt);

                    if(upperCase.Contains(c)){
                        ltr = FindNextLetterByChar(c);

                        if(ltr != null){
                            testWord += c.ToString();

                            bigLettersActive.Add(ltr);
                            bigLetters.Remove(ltr);
                            ltr.color = bigColorSelected;
                            ArrangeBigLetters();
                        }
                    }

                    if(c=='\b'){
                        if(bigLettersActive.Count == 0) return;
                        if(testWord.Length > 1){
                            testWord = testWord.Substring(0, testWord.Length-1);
                        } else {
                            testWord = "";
                        }

                        ltr = bigLettersActive[bigLettersActive.Count-1];

                        bigLettersActive.Remove(ltr);
                        bigLetters.Add(ltr);
                        ltr.color = bigColorDim;
                        ArrangeBigLetters();
                    }

                    if(c == '\n' || c == '\r'){
                        CheckWord();
                    }

                    if(c == ' '){
                        bigLetters = ShuffleLetters(bigLetters);
                        ArrangeBigLetters();
                    }
                    
                }
                break;
        }
    }

    Letter FindNextLetterByChar(char c){
        foreach (Letter ltr in bigLetters){
            if(ltr.c == c){
                return ltr;
            }            
        }
        return null;
    }

    public void CheckWord(){
        string subWord;
        bool foundTestWord = false;

        List<int> containedWords = new List<int>();

        for(int i=0; i<currLevel.subWords.Count; i++){

            if(wyrds[i].found){
                continue;
            }

            subWord = currLevel.subWords[i];

            if(string.Equals(testWord, subWord)){
                HighlightWyrd(i);
                ScoreManager.SCORE(wyrds[i], 1);
                foundTestWord = true;
            } else if(testWord.Contains(subWord)){
                containedWords.Add(i);
            }
        }

        if(foundTestWord){
            int numContained = containedWords.Count;
            int ndx;

            for(int i = 0; i<containedWords.Count; i++){
                ndx = numContained -i-1;
                HighlightWyrd(containedWords[ndx]);
                if(containedWords[ndx].Length == PlayerPrefs.GetInt("GameType")){
                    PlayerPrefs.SetInt("ShowNextLevelButton", 1);
                }
                ScoreManager.SCORE(wyrds[containedWords[ndx]], i+2);
            }
        }

        ClearBigLettersActive();
    }

    void HighlightWyrd(int ndx){
        wyrds[ndx].found = true;

        wyrds[ndx].found = true;

        wyrds[ndx].color = (wyrds[ndx].color+Color.white)/2f;
        wyrds[ndx].visible = true;
    }

    void ClearBigLettersActive(){
        testWord = "";
        foreach(Letter ltr in bigLettersActive){
            bigLetters.Add(ltr);
            ltr.color = bigColorDim;
        }
        bigLettersActive.Clear();
        ArrangeBigLetters();
    }
}
