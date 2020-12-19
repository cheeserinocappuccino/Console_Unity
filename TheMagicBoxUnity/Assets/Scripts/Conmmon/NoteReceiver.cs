using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class NoteReceiver : MonoBehaviour
{

    // 目前判斷能調整的空間蠻小的 可能要針對個別的音符來啟動而不是接收器
    Metronome _metronome;
    public int receiverNumber; // 現在太分散 最好集中管理
    //public static string[] primTriggersName = new string[2] { "RightPrimTrigger", "LeftPrimTrigger" };
    //public static string[] wheelTriggersName = new string[2] { "R_HitTriiger", "L_HitTriiger" };
    //public Transform[] sparkPos;
   //public Transform[] frontSparkPos;
    //public Transform[] cdPos;
    //public Transform[] energyPos;
    //public GameObject sparkInstance;
    //public GameObject frontSparkInstance;
    //public GameObject cdInstance;
    //public GameObject energyInstance;

    // TEST
    /*public Text b;


    public Slider slider;

    public Text combo;
    */
    bool threshHold = false;
    float threshHoldTimer = 0;
    int spawner;


    //public string[] playingButtons; // 這裡最好不要這樣寫會變成每一個都要設定

    public delegate void CallPressContainer();

    public CallPressContainer callPressContainer;

    bool unMiss = true;
    float tolerenceTemp;


    public Color normalColor;
    public Color goodHitColor;
    public Color badHitColor;
    private Color nowColor;
  
    void Awake()
    {
        _metronome = GameObject.FindGameObjectWithTag("GameController").GetComponent<Metronome>();
        this._metronome.TimeToPress += this.NoteThreshHoldOn;
        
    }

    void Update()
    {
      

        if (threshHoldTimer > 0)
        {
            threshHoldTimer -= Time.deltaTime;
        }
        else if (threshHoldTimer <= 0)
        {
            //Debug.Log(unMiss);
            threshHoldTimer = 0;
            threshHold = false;
            callPressContainer = DisCombo;
            if (unMiss == false)
            {
                DisCombo();
                Metronome.combo = 0;
                Metronome.miss++;
                unMiss = true;
            }

        }
        //slider.value = threshHoldTimer;
        //b.text = threshHold == true ? "★" : "☆";

        callPressContainer?.Invoke();
        //nowColor = Vector4.Lerp(nowColor, normalColor, Time.deltaTime * 5.0f);

        
        //Press();

    }
    void NoteThreshHoldOn(object sender, NotesEventArgs e)
    {
        threshHoldTimer = 0;
        bool foundPressing = false;

        //Debug.Log(e.nextNoteTiming + "<< nextnoteTiming" );
        if (e.nextNoteTiming > 0 && e.nextNoteTiming <= e.tolerence)
        {
            threshHoldTimer = e.nextNoteTiming / 1.5f;
            //threshHoldTimer = 0;
            //Debug.Log(threshHoldTimer + "threshHolde");
        }
        else
        {
            threshHoldTimer = e.tolerence * 2.0f;
        }
        //threshHoldTimer = e.tolerence * 2.0f;
        tolerenceTemp = threshHoldTimer;
        if (tolerenceTemp <= 0.1f)
            tolerenceTemp = 0.15f;
        //Debug.Log(threshHoldTimer + "threshHolde");
        for (int i = 0; i < e.spawners.Length; i++)
        {
            if (e.spawners[receiverNumber] == true) // 應該要偵測到就不再回圈 不然會永遠都是true或是永遠是false
            {
                this.threshHold = true;
                callPressContainer = Press;
                unMiss = false;
                foundPressing = true;

            }
            else
            {

                unMiss = true;
            }
            if (foundPressing == true)
            {
                break;
            }
        }



    }

    public static int good, bad = 0;
    void Press()
    {


        if (threshHold == true && Input.GetKeyDown(KeyCode.T))
        {
            //Debug.Log("press");
            if (threshHoldTimer >= tolerenceTemp / 8.0f && threshHoldTimer <= (tolerenceTemp / 8.0f * 6f))
            {
                Debug.Log("GOOD");
                //_reactingBarMaterial.SetColor("_EmissiveColor", new Vector4(goodHitColor.r * 1, goodHitColor.r * 1, goodHitColor.r, 1.0f));
                nowColor = goodHitColor * 5.0f;
                good++;
                Metronome.good++;
               
            }
            else
            {
                Debug.Log("BAD");
                //_reactingBarMaterial.SetColor("_EmissiveColor", new Vector4(badHitColor.r * 1, badHitColor.r * 1, badHitColor.r, 1.0f));
                nowColor = badHitColor * 2.0f;
                bad++;
                Metronome.bad++;
                
            }
            Metronome.score++; // 以後也用事件可能比較好
            Metronome.combo += 1;
            
            callPressContainer = DisCombo; // 按過就關掉了
            //combo.text = "Combo : " + Metronome.combo + " total Good : " + good + " BAD : " + bad;
            //Debug.Log(Metronome.score);
            //_reactingBarMaterial.SetColor("_EmissiveColor", new Vector4(Color.red.r * 2, Color.red.g * 2, Color.red.b, 2.0f));
            unMiss = true;
        }

    }
    void DisCombo()
    {
        if (threshHold == false && Input.GetKeyDown(KeyCode.T))
        {
            Metronome.combo = 0;
            Debug.Log("MISS");
            //combo.text = "Combo : " + Metronome.combo + " total Good : " + good + " BAD : " + bad;

        }
        // Metronome.combo = 0;
        
    }

}




