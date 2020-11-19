using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldLevel{

    public int levelNum;
    public int longWordIndex;
    public string word;

    public Dictionary<char, int> charDict;

    public List<string> subWords;

    static public Dictionary<char, int> MakeCharDict(string w){
        Dictionary<char, int> dict = new Dictionary<char, int>();
        char c;
        for(int i = 0; i<w.Length; i++){
            c = w[i];
            if(dict.ContainsKey(c)){
                dict[c]++;
            } else{
                dict.Add (c, 1);
            }
        }
        return(dict);
    }
}
