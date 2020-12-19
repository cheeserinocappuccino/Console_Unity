using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 這裡用來定義每個音符要有甚麼屬性，例如打到他可以得分或扣血或震動(?)之類的都可以寫上

// 用來定義每一個"音符"
[System.Serializable]
public class NoteJsonHeader
{
    //public NoteAttributes[] everyNotes;
    public List<NoteAttributes> everyNotes = new List<NoteAttributes>();
}

// 用來定義每一個"音符裡帶的資訊"
[System.Serializable]
public class NoteAttributes // : MonoBehaviour Json要讀的東西不能繼承自MonoBehavior
{
    public int measurement;
    public int measure32;
    public string[] attacktype;
    public int rowPos;
    public string column1;
    public string column2;
    public string column3;
    public Color noteColor;

    // 以下這些json沒有設定(因為要之後靠程式算) 所以不會被複寫
    public float Appear_Time = 0;
    public bool[] spawnners; // = rowPos
}

