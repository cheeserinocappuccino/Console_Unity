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
            int spawner3 = _noteJsonHeader.everyNotes[i].rowPos == 3 ? 1 : 0;
            int spawner4 = _noteJsonHeader.everyNotes[i].rowPos == 4 ? 1 : 0;
            int spawner5 = _noteJsonHeader.everyNotes[i].rowPos == 5 ? 1 : 0;
            int spawner6 = _noteJsonHeader.everyNotes[i].rowPos == 6 ? 1 : 0;
            int spawner7 = _noteJsonHeader.everyNotes[i].rowPos == 7 ? 1 : 0;
            int spawner8 = _noteJsonHeader.everyNotes[i].rowPos == 8 ? 1 : 0;
            int spawner9 = _noteJsonHeader.everyNotes[i].rowPos == 9 ? 1 : 0;
            int spawner10 = _noteJsonHeader.everyNotes[i].rowPos == 10 ? 1 : 0;
            int spawner11 = _noteJsonHeader.everyNotes[i].rowPos == 11 ? 1 : 0;

            int spawner17 = _noteJsonHeader.everyNotes[i].rowPos == 17 ? 1 : 0;

            int spawner18 = _noteJsonHeader.everyNotes[i].rowPos == 18 ? 1 : 0;
            int spawner19 = _noteJsonHeader.everyNotes[i].rowPos == 19 ? 1 : 0;
            int spawner20 = _noteJsonHeader.everyNotes[i].rowPos == 20 ? 1 : 0;
            int spawner21 = _noteJsonHeader.everyNotes[i].rowPos == 21? 1 : 0;
            int spawner22 = _noteJsonHeader.everyNotes[i].rowPos == 22 ? 1 : 0;
            int spawner23 = _noteJsonHeader.everyNotes[i].rowPos == 23 ? 1 : 0;
            int spawner24 = _noteJsonHeader.everyNotes[i].rowPos == 24 ? 1 : 0;
            int spawner25 = _noteJsonHeader.everyNotes[i].rowPos == 25 ? 1 : 0;
            int spawner26 = _noteJsonHeader.everyNotes[i].rowPos == 26 ? 1 : 0;
            int spawner27 = _noteJsonHeader.everyNotes[i].rowPos == 27 ? 1 : 0;
            int spawner28 = _noteJsonHeader.everyNotes[i].rowPos == 28 ? 1 : 0;
            int spawner29 = _noteJsonHeader.everyNotes[i].rowPos == 29 ? 1 : 0;
            int spawner30 = _noteJsonHeader.everyNotes[i].rowPos == 30 ? 1 : 0;
            int spawner31 = _noteJsonHeader.everyNotes[i].rowPos == 31 ? 1 : 0;
            int spawner32 = _noteJsonHeader.everyNotes[i].rowPos == 32 ? 1 : 0;
        






            bool[] temp = new bool[33];
            // red
            temp[0] = spawner0 == 1 ? true : false;
            temp[1] = spawner1 == 1 ? true : false;
            temp[2] = spawner2 == 1 ? true : false;
            temp[3] = spawner3 == 1 ? true : false;
            temp[4] = spawner4 == 1 ? true : false;
            temp[5] = spawner5 == 1 ? true : false;
            // blue
            temp[6] = spawner6 == 1 ? true : false;
            temp[7] = spawner7 == 1 ? true : false;
            temp[8] = spawner8 == 1 ? true : false;
            temp[9] = spawner9 == 1 ? true : false;
            temp[10] = spawner10 == 1 ? true : false;
            temp[11] = spawner11 == 1 ? true : false;


            //TouchBoard
            temp[17] = spawner17 == 1 ? true : false;

            temp[18] = spawner18 == 1 ? true : false;
            temp[19] = spawner19 == 1 ? true : false;
            temp[20] = spawner20 == 1 ? true : false;
            temp[21] = spawner21 == 1 ? true : false;
            temp[22] = spawner22 == 1 ? true : false;
            temp[23] = spawner23 == 1 ? true : false;
            temp[24] = spawner24 == 1 ? true : false;
            temp[25] = spawner25 == 1 ? true : false;
            temp[26] = spawner26 == 1 ? true : false;
            temp[27] = spawner27 == 1 ? true : false;
            temp[28] = spawner28 == 1 ? true : false;
            temp[29] = spawner29 == 1 ? true : false;
            temp[30] = spawner30 == 1 ? true : false;
            temp[31] = spawner31 == 1 ? true : false;
            temp[32] = spawner32 == 1 ? true : false;



            _noteJsonHeader.everyNotes[i].spawnners = temp;

            bool isRed = false;
            // 決定顏色
            for(int red = 0; red < 6; red++)
            {
                if(temp[red] == true)
                {
                    isRed = true;
                }
                
            }

            if(isRed == true)
            {
                _noteJsonHeader.everyNotes[i].noteColor = new Color(255, 0, 0, 255);
            }
            else
            {
                _noteJsonHeader.everyNotes[i].noteColor = new Color(0, 213, 255, 255);
            }

            /*Color Red = new Color(255, 0, 75, 255);
            Color Blue = new Color(0, 213, 255, 255);*/


            // 之前雷射需要的參數，我想我這邊還不需要，先註解掉，但格式可以參考
            /*if (_noteJsonHeader.everyNotes[i].attacktype[0] == "Blue")
            {
                _noteJsonHeader.everyNotes[i].isBlue = true;
             
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
