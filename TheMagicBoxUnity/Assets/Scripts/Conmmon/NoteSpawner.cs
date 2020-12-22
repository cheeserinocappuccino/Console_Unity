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

    public static UInt16 TouchBoardCollect = 0;
    static int CollectedNum = 0;
    static int MaxCollectedNum = 6;
    static int scannedNum = 0;
    public static bool canCollectTouchBoard = false;
    public static bool CanSendToTouch = false;
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
            if (e.spawners[17] == true && thisSpawnerNumber == 17)
            { // 如果是觸控板的話{
                canCollectTouchBoard = true;


            }

            if (e.spawners[thisSpawnerNumber] == true && thisSpawnerNumber != 17) 
            {
                
                if(true)
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

        if (canCollectTouchBoard && thisSpawnerNumber == 17)
        {
            scannedNum++;
            for (int i = 18; i <= 32; i++)
            {
                
                if (e.spawners[i] == true)
                {
                    TouchBoardCollect |= (UInt16)(1 << (i - 18));
                    CollectedNum++;
                    Debug.Log(i + "__偵測到的排數");
                }

                if(CollectedNum >= MaxCollectedNum)
                {
                    // 如果 掃到6個了就開始送command
                    i = 33;
                    CanSendToTouch = true;
                }
            }
            // 或者是掃超過16個都還沒湊滿的話
            if(scannedNum >= 16)
            {
                CanSendToTouch = true;
                scannedNum = 0;
            }

        }

        if (CanSendToTouch && thisSpawnerNumber == 17)
        {
            byte[] temp = new byte[6];
            int a = 0;
            // 開始傳指令
            Debug.Log(TouchBoardCollect + "__看TouchBoardCollect是多少");
            for (int i = 1; i < 16; i++)
            {
                UInt16 tempCollect = (UInt16)(TouchBoardCollect & (UInt16)(1 << (i - 1)));
                if ( (tempCollect >> (i-1)) == 1 && a < 6)
                {

                    temp[a] = (byte)i;

                    a++;// 這個-1是當次點亮觸控板數量
                }

            }

            // 計算temp[]裡有哪一些顆數
            byte oneTwo = 0;
            byte thrFour = 0;
            byte fiveSix = 0;

            for (int j = 0; j < temp.Length; j++)
            {
                switch (j)
                {
                    case 0:
                        {
                            byte add = (byte)(temp[j] << 4);
                            if(add == 0)
                            {
                                add = 10 << 4;
                            } 

                            oneTwo += add;
                            
                            break;
                        }
                    case 1:
                        {
                            byte add = temp[j];
                            if (add == 0)
                            {
                                add = 14;
                            }
                            oneTwo += add;

                            break;
                        }
                    case 2:
                        {
                            byte add = (byte)(temp[j] << 4);
                            if (add == 0)
                            {
                                add = 12 << 4;
                            }

                            thrFour += add;
                            break;
                        }
                    case 3:
                        {
                            byte add = temp[j];
                            if (add == 0)
                            {
                                add = 8;
                            }
                            thrFour += add;
                            break;
                        }
                    case 4:
                        {
                            byte add = (byte)(temp[j] << 4);
                            if (add == 0)
                            {
                                add = 4 << 4;
                            }
                            fiveSix += add;
                            break;
                        }
                    case 5:
                        {
                            byte add = temp[j];
                            if (add == 0)
                            {
                                add = 1;
                            }

                            fiveSix += add;
                            break;
                        }


                }
               
            }
            //ControllerController.TBSend_GoBackGround();
            
            // 送command
            byte[] touchSpawn = new byte[] { 1, 1, (byte)(a-1), oneTwo, thrFour, fiveSix, 50, 50 };

            // byte amount = (byte)UnityEngine.Random.Range(0, 5);


            //byte[] touchSpawn = new byte[] { 1, 1, 6, 21, 191, 103, 50, 50 };
            
            ControllerController.TBSend_Touch(touchSpawn);

            Debug.Log(a + "__看看a是多少");

            // 結束
            TouchBoardCollect = 0;
            canCollectTouchBoard = false;
            CollectedNum = 0;
            CanSendToTouch = false;
        }

    }




}
