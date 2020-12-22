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


    public string[] playingButtons; // 這裡最好不要這樣寫會變成每一個都要設定

    public delegate void CallPressContainer();

    public CallPressContainer callPressContainer;

    bool unMiss = true;
    float tolerenceTemp;

    int red_index = 0;
    int blue_index = 0;

    // 之後要跟ControllerController要資料
    public static float red_degree = 0;
    public static float blue_degree = 0;
    public GameObject blueGamepoint, redGamePoint;
    static bool innerRingPressed, OuterRingPressed = false;

    ControllerController mainboard;

    // 按鈕亮起的時間
    public static float Inner_gamePointBrightTime = 0.2f;
    public static float Inner_gamePointBrightTime_Timer = 0;

    public static float Outter_gamePointBrightTime = 0.2f;
    public static float Outter_gamePointBrightTime_Timer = 0;

    // combo字體
    //public Text //thePerfectGoodMiss;

    // combo特效
    public Image[] hitEffects;

    void Awake()
    {
        _metronome = GameObject.FindGameObjectWithTag("GameController").GetComponent<Metronome>();
        this._metronome.TimeToPress += this.NoteThreshHoldOn;

        mainboard = GameObject.FindGameObjectWithTag("controllerController").GetComponent<ControllerController>();

    }

    void Update()
    {
        // 藍按鈕亮起
        if (Inner_gamePointBrightTime_Timer > 0)
        {
            Inner_gamePointBrightTime_Timer -= Time.deltaTime;
            blueGamepoint.GetComponent<Image>().color = new Color(0, 1.0f, 1.0f, 1.0f);
            //Debug.Log("???");
        }
        else if (Inner_gamePointBrightTime_Timer <= 0)
        {
            Inner_gamePointBrightTime_Timer = 0;
            blueGamepoint.GetComponent<Image>().color = new Color(0, 0.6f, 1.0f, 0.6f);

        }
        // 紅按鈕亮起
        if (Outter_gamePointBrightTime_Timer > 0)
        {
            Outter_gamePointBrightTime_Timer -= Time.deltaTime;
            redGamePoint.GetComponent<Image>().color = new Color(1.0f, 0, 0, 1.0f);
            //Debug.Log("???");
        }
        else if (Outter_gamePointBrightTime_Timer <= 0)
        {
            Outter_gamePointBrightTime_Timer = 0;
            redGamePoint.GetComponent<Image>().color = new Color(1.0f, 0, 0, 0.6f);

        }

        //blue_degree = Mathf.Repeat(ControllerController.InnerRing_CalData, 360);
        blue_degree = ControllerController.InnerRing_CalData / (float)2.84444;
        blue_degree = Mathf.Repeat(blue_degree, 360);
        blueGamepoint.transform.rotation = Quaternion.Euler(0, 0, -blue_degree);

        // REd
        red_degree = -(ControllerController.OutterRing_CalData / (float)41.0f);
        red_degree = Mathf.Repeat(red_degree, 360);
        redGamePoint.transform.rotation = Quaternion.Euler(0, 0, -red_degree);

        //Debug.Log(blue_degree);
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
                Debug.Log("you Missed");
                //thePerfectGoodMiss.color = new Color(0.1f, 0.1f, 0.1f, 1.0f);
                //thePerfectGoodMiss.text = "MISS";
            }
            // 模擬input.getkeyDown
            innerRingPressed = false;
            OuterRingPressed = false;
        }
   

        callPressContainer?.Invoke();
     


        
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
        if(receiverNumber == 0)// 0~5是紅色負責，receivernumber要給0，現在debug是按T
        {
            for (int i = 0; i < 6; i++) 
            {
                if (e.spawners[i] == true) // 應該要偵測到就不再回圈 不然會永遠都是true或是永遠是false
                {
                    this.threshHold = true;
                    callPressContainer = Press;
                    unMiss = false;
                    foundPressing = true;
                    red_index = i;// 至少要知道這顆紅色是第幾顆紅色才能判斷指針吧

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
        }else if(receiverNumber == 1)
        {
            for (int i = 6; i < 12; i++)
            {
                if (e.spawners[i] == true) // 應該要偵測到就不再回圈 不然會永遠都是true或是永遠是false
                {
                    this.threshHold = true;
                    callPressContainer = Press;
                    unMiss = false;
                    foundPressing = true;
                    blue_index = i; // 至少要知道這顆藍色是第幾顆藍色才能判斷指針吧
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
    



    }

    public static int good, bad = 0;
    void Press()
    {

        if (receiverNumber == 0)
        {
            
               
            int notePosTemp = 360 - (((red_index + 1) * 60 - 60));
            
            //Debug.Log(red_degree+1 + ">" + Mathf.Repeat(notePosTemp - 30, 329) + "");
            if (threshHold == true && OuterRingPressed && red_degree + 1 >= Mathf.Repeat(notePosTemp - 30, 329) && Mathf.Repeat(red_degree, 330) < Mathf.Repeat(notePosTemp + 30, 360))
            {
                //Debug.Log("press");
                if (threshHoldTimer >= tolerenceTemp / 8.0f && threshHoldTimer <= (tolerenceTemp / 8.0f * 6f))
                {
                    Debug.Log("GOOD");
                    //_reactingBarMaterial.SetColor("_EmissiveColor", new Vector4(goodHitColor.r * 1, goodHitColor.r * 1, goodHitColor.r, 1.0f));
                    //nowColor = goodHitColor * 5.0f;
                    good++;
                    Metronome.good++;
                    //thePerfectGoodMiss.color = new Color(0.0f, 1.0f, 0, 1.0f);
                    //thePerfectGoodMiss.text = "GOOD";

                    // 打擊特效
                    hitEffects[red_index].GetComponent<HitEffect>().CallHit(new Color(0, 1, 0, 1));

                }
                else
                {
                    Debug.Log("BAD");
                    //_reactingBarMaterial.SetColor("_EmissiveColor", new Vector4(badHitColor.r * 1, badHitColor.r * 1, badHitColor.r, 1.0f));
                    //nowColor = badHitColor * 2.0f;
                    bad++;
                    //thePerfectGoodMiss.color = new Color(0.8f, 0, 0, 1.0f);
                    //thePerfectGoodMiss.text = "BAD";
                    Metronome.bad++;

                    // 打擊特效
                    hitEffects[red_index].GetComponent<HitEffect>().CallHit(new Color(1, 0, 0, 1));

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
        else if (receiverNumber == 1)
        {
            int notePosTemp = 360 - (((blue_index - 5) * 60 - 60));
            //Debug.Log(blue_degree+1 + ">" + Mathf.Repeat(notePosTemp - 30, 329) + "");
            if (threshHold == true && innerRingPressed &&  blue_degree+1 >= Mathf.Repeat(notePosTemp - 30,329) && Mathf.Repeat(blue_degree,330) < Mathf.Repeat(notePosTemp+30,360))
            {
                Debug.Log("pressBlue");
                if (threshHoldTimer >= tolerenceTemp / 8.0f && threshHoldTimer <= (tolerenceTemp / 8.0f * 6f))
                {
                    Debug.Log("GOOD");
                    //_reactingBarMaterial.SetColor("_EmissiveColor", new Vector4(goodHitColor.r * 1, goodHitColor.r * 1, goodHitColor.r, 1.0f));
                    //nowColor = goodHitColor * 5.0f;
                    good++;
                    Metronome.good++;
                    //thePerfectGoodMiss.color = new Color(0.0f, 1.0f, 0, 1.0f);
                    //thePerfectGoodMiss.text = "GOOD";

                    // 打擊特效
                    hitEffects[blue_index - 6].GetComponent<HitEffect>().CallHit(new Color(0,1,0,1));
                }
                else
                {
                    Debug.Log("BAD");
                    //_reactingBarMaterial.SetColor("_EmissiveColor", new Vector4(badHitColor.r * 1, badHitColor.r * 1, badHitColor.r, 1.0f));
                    //nowColor = badHitColor * 2.0f;
                    bad++;
                    Metronome.bad++;
                    //thePerfectGoodMiss.color = new Color(0.8f, 0, 0, 1.0f);
                    //thePerfectGoodMiss.text = "BAD";

                    // 打擊特效
                    hitEffects[blue_index - 6].GetComponent<HitEffect>().CallHit(new Color(1, 0, 0, 1));

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
       
    }
    void DisCombo()
    {
        if(receiverNumber == 0 && threshHold == false && OuterRingPressed)
        {
            Metronome.combo = 0;
            Debug.Log("MISS");
            OuterRingPressed = false;
        }
        else if (receiverNumber == 1 &&threshHold == false && innerRingPressed)
        {
            Metronome.combo = 0;
            Debug.Log("MISS");
            innerRingPressed = false;
            //combo.text = "Combo : " + Metronome.combo + " total Good : " + good + " BAD : " + bad;

        }
        // Metronome.combo = 0;
        
    }

    // 因為接收指令的執行續跟unity不一樣，會有時差，所以讓接收指令那邊呼叫unity執行續的東西，讓innerRingPressed加入unity執行續就能做到類似input.getkeyDOWN只開一幀的效果
    public static void InnerRingPressed_ThreadHandle()
    {
        innerRingPressed = true;
        Inner_gamePointBrightTime_Timer = Inner_gamePointBrightTime;
    }

    public static void OutterRingPressed_ThreadHandle()
    {
        OuterRingPressed = true;
        Outter_gamePointBrightTime_Timer = Outter_gamePointBrightTime;
    }
}




