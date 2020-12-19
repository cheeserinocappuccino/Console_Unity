using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Metronome : MonoBehaviour
{
    [SerializeField]
    public GameLevelMusic nowplay;
    public AudioSource LevelAudioSource;

    private GotoMain gotoMain;
    public Gif backgrounds;

 

    public event System.EventHandler<NotesEventArgs> SpawnNote;
    
    public event System.EventHandler<NotesEventArgs> TimeToPress;


    // --------------- 從舊的metronome拿來的 ----------------
    public float lastBeat = 0;     // 音樂如果比較晚開始，就讓這值是負值(加入offset的意思)
    public float quarterNoteTime;   // 音樂的解析度:每x秒算一次
    public int totalBeatCount;

    public static float arriveTime = 0.8f; // 音符提早生成的時間(音符到目標所花時間) 不要設超過StartDelay // 現在設成EletricEffect上的音符到達時間(一訂要"相對") 因為飛行速度跟提早生成的時間彼此在程式上沒有關聯

    public int spawnLoopStartNum = 0;
    int ReceiveLoopStartNum = 0;

    // 給延遲啟動用的
    public float StartDelay = 3;
    private float StartDelayTimer = 0;
    bool IsPlaying = false;


    // 節拍輸入的容許誤差
    public float tolerence = 0;
    public float offset = 0; // 整體偏移 避免全都太快或太慢
    private float playtime = 0;
    // 分數
    public static int score = 0;
    public static int combo = 0;
    public static int bad = 0;
    public static int good = 0;
    public static int miss = 0;

    // end  --------------- 從舊的metronome拿來的 ----------------
    void Awake()
    {

        try
        {
            gotoMain = GameObject.FindGameObjectWithTag("GoToMain").GetComponent<GotoMain>();


        }
        catch
        {
            Debug.Log("沒有GotoMain");
        }

        // 進遊戲跟gotomain拿資訊，以前是用ScriptableObject但很不穩所以改成用一個Gotomain
        nowplay = this.GetComponent<GameLevelMusic>();
        nowplay.SongquarterNoteTime = gotoMain.songQuarterNoteTime;
        nowplay.audio = gotoMain.selectedSongAudio;
        nowplay.noteJsonFileName = gotoMain.jsonFileName;
        nowplay.GameLevelMusicInstantiate();

        // 進遊戲時拿Gotomain改背景
        backgrounds.allImages = gotoMain.gameBackgrounds;

        quarterNoteTime = gotoMain.songQuarterNoteTime;

        // 撥音樂
        try
        {
            LevelAudioSource = this.GetComponent<AudioSource>();
            LevelAudioSource.clip = nowplay.audio;
        }
        catch
        {
            Debug.Log("沒有AudioSource");
        }
        
    }

    void Start()
    {
        


    }

    void Update()
    {
        Play();
        CallPressing(tolerence); // 之後tolerence可能寫在譜里比較好
        
        
        SummonNotes();
        
    }

    public void SummonNotes()
    {

        if (LevelAudioSource.time <= 0 && playtime - StartDelay >= (nowplay.notes[spawnLoopStartNum].Appear_Time - arriveTime + offset) && spawnLoopStartNum < nowplay.notes.Count - 1) // 這樣寫不對
        {
            SpawnNote?.Invoke(this, new NotesEventArgs(nowplay.notes[spawnLoopStartNum], nowplay.notes[spawnLoopStartNum].spawnners, nowplay.notes[spawnLoopStartNum].noteColor)); // 誰生成的應該在讀譜的時候就先記錄好吧 // 完成
            spawnLoopStartNum++;
        }
        else if (LevelAudioSource.time > 0 && LevelAudioSource.time >= (nowplay.notes[spawnLoopStartNum].Appear_Time - (arriveTime) + offset) && spawnLoopStartNum < nowplay.notes.Count - 1)
        {
            //Debug.Log("Type >> " + nowplay.notes[spawnLoopStartNum].attacktype[0]);
            if (false)
            {
                
            }
            else
            {
                SpawnNote?.Invoke(this, new NotesEventArgs(nowplay.notes[spawnLoopStartNum], nowplay.notes[spawnLoopStartNum].spawnners, nowplay.notes[spawnLoopStartNum].noteColor));
                spawnLoopStartNum++;
                //tunneBrightness = 1;

            }

        }
        else if (spawnLoopStartNum == nowplay.notes.Count - 1)
        {
            StartCoroutine(IEndGame());
        }

    }

    public void CallPressing(float _tolerence)
    {
        if (LevelAudioSource.time <= 0 && playtime - StartDelay >= (nowplay.notes[ReceiveLoopStartNum].Appear_Time + offset) && ReceiveLoopStartNum < nowplay.notes.Count - 1)
        {
            float thisnoteTime = nowplay.notes[ReceiveLoopStartNum].Appear_Time + offset;
            float nextNoteTime = thisnoteTime;
            if (nowplay.notes[ReceiveLoopStartNum].spawnners == nowplay.notes[ReceiveLoopStartNum + 1].spawnners)
            {
                nextNoteTime = nowplay.notes[ReceiveLoopStartNum + 1].Appear_Time + offset;
            }

            TimeToPress?.Invoke(this, new NotesEventArgs(_tolerence, nowplay.notes[ReceiveLoopStartNum].spawnners, nextNoteTime - thisnoteTime));
            ReceiveLoopStartNum++;


        }
        else if (LevelAudioSource.time > 0 && LevelAudioSource.time >= (nowplay.notes[ReceiveLoopStartNum].Appear_Time - _tolerence + offset) && ReceiveLoopStartNum < nowplay.notes.Count - 1)
        {
            float thisnoteTime = nowplay.notes[ReceiveLoopStartNum].Appear_Time + offset - _tolerence;
            float nextNoteTime = 0;



            for (int i = 0; i < nowplay.notes.Count; i++) // 這邊計算太多了其實 要注意
            {
                if (nowplay.notes[ReceiveLoopStartNum].spawnners[0] == true && nowplay.notes[ReceiveLoopStartNum].spawnners[0] == nowplay.notes[i].spawnners[0])
                {
                    nextNoteTime = nowplay.notes[ReceiveLoopStartNum + i].Appear_Time + offset - _tolerence;
                    // Debug.Log("0 is true");
                    i = nowplay.notes.Count;
                }
                else if (nowplay.notes[ReceiveLoopStartNum].spawnners[1] == true && nowplay.notes[ReceiveLoopStartNum].spawnners[1] == nowplay.notes[i].spawnners[1])
                {
                    nextNoteTime = nowplay.notes[ReceiveLoopStartNum + i].Appear_Time + offset - _tolerence;
                    // Debug.Log("1 is true");
                    i = nowplay.notes.Count;
                }

            }

            //Debug.Log(nextNoteTime - thisnoteTime + "NEXTNOTETIME");
            TimeToPress?.Invoke(this, new NotesEventArgs(_tolerence, nowplay.notes[ReceiveLoopStartNum].spawnners, nextNoteTime - thisnoteTime));
            ReceiveLoopStartNum++;

        }
    }

    public void Play()
    {
        playtime = StartDelayTimer + LevelAudioSource.time; // 
        if (StartDelayTimer < StartDelay - 0.021f && IsPlaying == false) // 0.021大約是fixedupdate一幀的時間 防止他扣成負數
        {
            StartDelayTimer += Time.deltaTime;
        }
        else if (StartDelayTimer >= StartDelay - 0.021f && IsPlaying == false)
        {
            StartDelayTimer = StartDelay;
            LevelAudioSource.Play();
            IsPlaying = true;
        }
    }

    IEnumerator IEndGame()
    {

        /*DontDestroyOnLoad(fadingImage.transform.parent.gameObject);
        
        Debug.Log(LevelAudioSource.volume + "audioVolume");
        while (LevelAudioSource.volume > 0.005f)
        {
            LevelAudioSource.volume -= 0.0005f;
            Debug.Log(LevelAudioSource.volume);
            yield return new WaitForSeconds(0.1f);
        }

        fadingImage.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Result");*/

        yield return new WaitForSeconds(100.0f);
    }
}
