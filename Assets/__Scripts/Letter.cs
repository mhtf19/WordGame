using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float timeDuration = .5f;
    public string easingCurve = Easing.InOut;

    [Header("Set Dynamically")]
    public TextMesh tMesh;
    public Renderer tRend;

    public bool big = false;

    public List<Vector3> pts = null;
    public float timeStart = -1;

    private char _c;  //the char shown on letter
    private Renderer rend;

    void Awake(){
        tMesh = GetComponentInChildren<TextMesh>();
        tRend = tMesh.GetComponent<Renderer>();
        rend = GetComponent<Renderer>();
        visible = false;
    }

    //set or get _c and letter shown by 3d text
    public char c{
        get{return _c;}
        set{
            _c = value;
            tMesh.text = _c.ToString();
        }
    }

    public string str{
        get{return _c.ToString();}
        set {c = value[0];}
    }

    //changes mode of renderer- char visible or not

    public bool visible{
        get{return tRend.enabled;}
        set{tRend.enabled = value;}
    }

    public Color color {
        get {return rend.material.color;}
        set {rend.material.color=value;}
    }

    public Vector3 pos{
        set{
            //transform.position = value;
            Vector3 mid = (transform.position+value)/2f;

            float mag = (transform.position-value).magnitude;
            mid += Random.insideUnitSphere*mag*.25f;

            pts = new List<Vector3>() {transform.position, mid, value};

            if(timeStart == -1) timeStart = Time.time;
        }
    }

    public Vector3 posImmediate{
        set{
            transform.position=value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timeStart == -1) return;

        float u = (Time.time-timeStart)/timeDuration;
        u = Mathf.Clamp01(u);
        float u1 = Easing.Ease(u, easingCurve);
        Vector3 v = Utils.Bezier(u1, pts);
        transform.position = v;

        if(u==1) timeStart = -1;
    }
}
