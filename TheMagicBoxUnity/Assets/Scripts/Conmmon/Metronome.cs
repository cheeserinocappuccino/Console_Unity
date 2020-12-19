using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    [SerializeField]
    public GameLevelMusic nowplay;
    public GotoMain gotoMain;

    void Awake()
    {

        try
        {
            gotoMain = GameObject.FindGameObjectWithTag("GoToMain").GetComponent<GotoMain>();
            //sceneVolumn.profile.TryGet(out hdri_Sky);
            //hdri_Sky.hdriSky.value = GameObject.FindGameObjectWithTag("scriptableSavior").GetComponent<GotoMain>().cubeMap.value;
        }
        catch
        {
            Debug.Log("沒有GotoMain");
        }

        nowplay = this.GetComponent<GameLevelMusic>();
        nowplay.SongquarterNoteTime = gotoMain.songQuarterNoteTime;
        nowplay.audio = gotoMain.selectedSongAudio;
        nowplay.GameLevelMusicInstantiate();


    }
}
