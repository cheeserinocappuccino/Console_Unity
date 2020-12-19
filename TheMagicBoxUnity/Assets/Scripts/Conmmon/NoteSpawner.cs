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
    void Awake()
    {
        _metronome = GameObject.FindGameObjectWithTag("GameController").GetComponent<Metronome>();
        this._metronome.SpawnNote += this.OnSpawn;
    }


    void Update()
    {

    }

    public void OnSpawn(object sender, NotesEventArgs e)
    {
        if (thisSpawnerNumber != -1)
        {
            if (e.spawners[thisSpawnerNumber] == true) 
            {
                // 生成音符的寫法
                GameObject noteTemp = Instantiate(noteInstance);
                noteTemp.GetComponent<Image>().rectTransform.rotation = Quaternion.Euler(0, 0, thisSpawnerNumber * 60);
                //noteTemp.transform.parent = canvas.transform;
                noteTemp.transform.SetParent(canvas.transform);
                noteTemp.transform.localPosition = new Vector3(0, 0, 0);
                noteTemp.GetComponent<NoteInstantiate>().noteColor = e.color;
            }
        }

    }




}
