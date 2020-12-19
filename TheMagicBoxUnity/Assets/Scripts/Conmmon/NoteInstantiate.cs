using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteInstantiate : MonoBehaviour
{
    public float arriveTime = 1.0f;
    Image thisNoteImage;
    public Color noteColor;

    float startSize = 1.0f;
    public float endSize = 0.3f;
    float nowSize = 0;

    float shrinkSpeed = 0;

    public float noteIndex = 0;
    void Awake()
    {
        thisNoteImage = GetComponent<Image>();
        

        shrinkSpeed = ((startSize - endSize) / arriveTime);
        nowSize = startSize;
        Debug.Log(shrinkSpeed);

        // 共有6個位置，設定為0~5，跟json的位置一樣
        thisNoteImage.rectTransform.rotation = Quaternion.Euler(0, 0, noteIndex * 60);
    }


    void FixedUpdate()
    {
        thisNoteImage.color = noteColor;
        nowSize -= shrinkSpeed * Time.deltaTime;
        nowSize = Mathf.Clamp(nowSize, endSize, startSize);
        thisNoteImage.rectTransform.localScale = new Vector3(nowSize, nowSize, 1.0f);

        if(nowSize <= endSize)
        {
            Destroy(this.gameObject);
        }
    }
}
