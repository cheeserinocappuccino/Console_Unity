using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class NoteSpawner : MonoBehaviour
{
    public int thisSpawnerNumber;


    public GameObject noteInstance;
    public GameObject canvas;

    [SerializeField]
    Metronome _metronome = null;

    // 生成音符的警示
    public Image theWarnLight;
    private float warnLightBrightness = 0;
    public float fadeSpeed = 2.0f;
    
    void Awake()
    {
        _metronome = GameObject.FindGameObjectWithTag("GameController").GetComponent<Metronome>();
        this._metronome.SpawnNote += this.OnSpawn;


    }


    void Update()
    {
        if (warnLightBrightness > 0)
        {
            warnLightBrightness -= Time.deltaTime * fadeSpeed;
        }
        else if (warnLightBrightness <= 0)
        {
            warnLightBrightness = 0;
        }
        
        theWarnLight.color = new Color(1.0f, 1.0f, 1.0f, warnLightBrightness);
        
      
        
    }

    public void OnSpawn(object sender, NotesEventArgs e)
    {
        if (thisSpawnerNumber != -1)
        {
            if (e.spawners[thisSpawnerNumber] == true) 
            {

                warnLightBrightness = 1.0f;
                // 生成音符的寫法
                GameObject noteTemp = Instantiate(noteInstance);
                noteTemp.GetComponent<Image>().rectTransform.rotation = Quaternion.Euler(0, 0, thisSpawnerNumber * 60);
                //noteTemp.transform.parent = canvas.transform;
                noteTemp.transform.SetParent(canvas.transform);
                noteTemp.transform.localPosition = new Vector3(0, 0, 0);
                noteTemp.GetComponent<NoteInstantiate>().noteColor = e.color;
            }// 專門給warnlight的
            if (thisSpawnerNumber >= 6 && e.spawners[thisSpawnerNumber - 6] == true)
            {
                warnLightBrightness = 1.0f;
            }
            if (thisSpawnerNumber < 6 && e.spawners[thisSpawnerNumber + 6] == true)
            {
                warnLightBrightness = 1.0f;
            }
        }
        
    }




}
