using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // 讀檔案路徑用的

public class GameLevelMusic : MonoBehaviour
{
    public AudioClip audio;
    public string noteJsonFileName;
    public float SongquarterNoteTime;

    public List<NoteAttributes> notes = new List<NoteAttributes>();

    // 從json拿note進來
    StreamReader musicNoteFile_StreamReader;
    string jsonLoadBuffer;
    public NoteJsonHeader _noteJsonHeader;



    public void GameLevelMusicInstantiate()
    {
        EjectNoteFromNoteJson();
        NoteTimingCaculater();
        SpawnerCaculator();
        
    }

    public void EjectNoteFromNoteJson()
    {


        musicNoteFile_StreamReader = new StreamReader(System.IO.Path.Combine(Application.streamingAssetsPath, noteJsonFileName));

        jsonLoadBuffer = musicNoteFile_StreamReader.ReadToEnd();

        musicNoteFile_StreamReader.Close();


        _noteJsonHeader = JsonUtility.FromJson<NoteJsonHeader>(jsonLoadBuffer);

 

        notes = _noteJsonHeader.everyNotes;
        ForceSerialization();
    }

    // 用的時候要在mono呼叫一次
    public void NoteTimingCaculater()
    {
        // 從節拍器找現在在播的歌的scriptableobject
        Metronome metronome = GameObject.FindGameObjectWithTag("GameController").GetComponent<Metronome>();
        // 找要算的歌的bpm
        float songquarterNoteTime = metronome.nowplay.SongquarterNoteTime;

        for (int i = 0; i < _noteJsonHeader.everyNotes.Count; i++)
        {
            // 把節拍轉成實際時間 0.043103
            float _appear_Time = songquarterNoteTime * (((_noteJsonHeader.everyNotes[i].measurement - 1) * 128) + _noteJsonHeader.everyNotes[i].measure32);
            _noteJsonHeader.everyNotes[i].Appear_Time = _appear_Time;

        }
        ForceSerialization();
    }

    public void SpawnerCaculator()
    {
        // 譜還沒定義的暫時的寫法
        System.Random ran = new System.Random();

        // 這裡應該是從json拿資料轉換的部分
        for (int i = 0; i < _noteJsonHeader.everyNotes.Count; i++)
        {
            int spawner0 = _noteJsonHeader.everyNotes[i].rowPos == 0 ? 1 : 0;
            int spawner1 = _noteJsonHeader.everyNotes[i].rowPos == 1 ? 1 : 0;
            int spawner2 = _noteJsonHeader.everyNotes[i].rowPos == 2 ? 1 : 0;



            bool[] temp = new bool[3];
            temp[0] = spawner0 == 1 ? true : false;
            temp[1] = spawner1 == 1 ? true : false;
            temp[2] = false;

            _noteJsonHeader.everyNotes[i].spawnners = temp;

            // 之前雷射需要的參數，我想我這邊還不需要，先註解掉，但格式可以參考
            /*if (_noteJsonHeader.everyNotes[i].attacktype[0] == "Laser")
            {
                _noteJsonHeader.everyNotes[i].startPoint = 0.7f;
                _noteJsonHeader.everyNotes[i].swipeDir = -1;
                _noteJsonHeader.everyNotes[i].swipeSpeed = 0.7f; ;
                _noteJsonHeader.everyNotes[i].periodInMeasure32 = 160;
                _noteJsonHeader.everyNotes[i].warningTimeInMeasure32 = 96;
            }*/
        }
        ForceSerialization();
    }
    void ForceSerialization()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
