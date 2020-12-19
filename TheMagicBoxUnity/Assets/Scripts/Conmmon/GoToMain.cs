using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering;
public class GotoMain : MonoBehaviour
{

    public string jsonFileName;
    public float songQuarterNoteTime;
    public AudioClip selectedSongAudio;


    void Awake()
    {
        //nowplay.audio = this.GetComponent<AudioSource>().clip;

        Object.DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}