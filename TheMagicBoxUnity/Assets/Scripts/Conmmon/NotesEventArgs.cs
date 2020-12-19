using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 這裡用來設定生成(匯入)音符時要做的事情
public class NotesEventArgs : System.EventArgs
{
    public bool[] spawners; // 20201216..看起來是第一channel要不要生譜是用bool[1] = true or false去確認，第二channel用bool[2]，類推
    public float tolerence, nextNoteTiming;
    public Color color;
    public NotesEventArgs(NoteAttributes _NoteAttribute, bool[] _spawners, Color _color)
    {
        spawners = _spawners;
        color = _color;
    }
    public NotesEventArgs(float _tolerence, bool[] _spawners, float _nextNoteTiming)
    {

        tolerence = _tolerence;
        spawners = _spawners;
        nextNoteTiming = _nextNoteTiming;
    }
}
