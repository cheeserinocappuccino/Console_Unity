using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;
using System.Data;
using System.Threading.Tasks;
using System;


// note: pls use another thread to do serial stuff
// https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.readline?redirectedfrom=MSDN&view=dotnet-plat-ext-5.0#System_IO_Ports_SerialPort_ReadLine

public class ControllerController : MonoBehaviour
{
    // port1 ----------
    [SerializeField]
    //private string[] coms;
    public string useport;
    public int baudrate;
    public int databits;
    public StopBits stopbits;
    public Parity parity;
    public static byte commandSTR = 77;

    static SerialPort port1;

    Thread connectionThread = new Thread(Connection1_Establishing);
    public static bool port1Reconnecting = true;

    Thread readThread = new Thread(SerialPort1_DataReading);

    // end port1 ----------

    // port2 ----------
    public string useport2;
    public int baudrate2;
    public int databits2;
    public StopBits stopbits2;
    public Parity parity2;
    public static byte commandSTR2 = 77;

    static SerialPort port2;

    Thread connectionThread2 = new Thread(Connection2_Establishing);
    public static bool port2Reconnecting = true;

    Thread readThread2 = new Thread(SerialPort2_DataReading);

    static byte[] commandBuffer2 = new byte[8];
    // end port2 ----------

    // command buffer 
    static byte[] commandBuffer = new byte[8];

    // console data
    public static int TouchBoards_data = 0;
    public static int innerRing_Rawdata = 0;
    public static int outterRing_Rawdata = 0;

    public static int LastlinnerRing_Rawdata = -1;
    public static int LastoutterRing_Rawdata = -1;

    public static float InnerRing_CalData = 0;
    public static float OutterRing_CalData = 0;


    // 定義差距超過多少才算是偵測到有轉過1023跟0之間
    public static int overCenterDetection_value = 500;

    // 偵測按鈕有被按下
    public static bool InnerRingPressed = false;

    

    void Awake()
    {
       
        port1 = new SerialPort(useport, baudrate, parity, databits, stopbits);
        port1.Open();
        //connectionThread.Start();
        readThread.Start();


        port2 = new SerialPort(useport2, baudrate2, parity2, databits2, stopbits2);
        port2.Open();

        //connectionThread2.Start();
        readThread2.Start();

        //InnerRing_CalData = new UInt16[1024];
        //OutterRing_CalData = new UInt16[1024];
       
    }

     void Update()
    {
        
    }

    // run a another thread to prevent performance issue
    // Other's example often join this thread back to the main thread, but we are kinda polling commands and we need to always polling
    // so maybe It can be here, as long as we don't accidently create more thread;
    private static void SerialPort1_DataReading()
    {
        while (true)
        {
            InnerRingPressed = false;

            try
            {
                // First, find the start command, in this case I like to use 77;
                byte FindingStartCmd = (byte)port1.ReadByte();

                // when finding the start command
                if (FindingStartCmd == commandSTR)
                {
                    // lets get the start command into our buffer first
                    commandBuffer[0] = commandSTR;
                    String commandShow = "";
                    commandShow += commandBuffer[0];

                    // and read the remain data, because I already determined that I'm using 8 byte data length, I know I need to read 7 more byte;
                    port1.Read(commandBuffer, 1, 7);

                    // get those data into a string for a easy debug
                    for (int i = 1; i < commandBuffer.Length; i++)
                    {
                        commandShow += "|";
                        commandShow += commandBuffer[i].ToString();

                    }
                    //Debug.Log(commandShow);

                    // now we know what command we received, Let's assign some task

                    // Do thing depends on the commands----//

                    switch (commandBuffer[5])
                    {
                        case 1: // 觸控板
                            {

                                break;
                            }
                        case 2: // 內圈
                            {
                                
                                if (LastlinnerRing_Rawdata != -1)
                                {
                                    

                                    innerRing_Rawdata = 0;
                                    int MSB = commandBuffer[6] << 8; // because the data's maximun is 1024 so needed two byte to store, this line decode the sceond byte
                                    innerRing_Rawdata += MSB;
                                    innerRing_Rawdata += commandBuffer[7];

                                    int diff = innerRing_Rawdata - LastlinnerRing_Rawdata;
                                    
                                    // 轉過1023跟0之間的時候 diff會異常地大，以下算式是發現這情況的時候所做的補償，讓程式知道1023跟0其實只差1
                                    // 先把這個值叫做overCenterDetection_value
                                    if(Mathf.Abs(diff) >= overCenterDetection_value)
                                    {
                                        int temp = (-1023 * (diff / Mathf.Abs(diff))) + diff;
                                        diff = temp;
                                    }
                                    InnerRing_CalData += diff;

                                    //Debug.Log("內圈上一次偵測是 " + LastlinnerRing_Rawdata + " 現在轉到了 " + innerRing_Rawdata + " ，共差距 " + diff);
                                    //Debug.Log("現在指針 = " + InnerRing_CalData);
                                    LastlinnerRing_Rawdata = innerRing_Rawdata;

                                    
                                }
                                else // 起始設定
                                {
                                    // if the innerRingdata == -1 means that it's first initial
                                    // so store the initial data to 歸0
                                    int MSB = commandBuffer[6] << 8;
                                    LastlinnerRing_Rawdata = 0;
                                    LastlinnerRing_Rawdata += MSB;
                                    LastlinnerRing_Rawdata += commandBuffer[7];
                            

                             
                                    Debug.Log(innerRing_Rawdata + " <<<<<內圈的起始位置");

                                }
                        
                                //Debug.Log(innerRing_Rawdata);

                          
                                break;
                            }
                        case 3: // 外圈
                            {
                                if (LastoutterRing_Rawdata != -1)
                                {
                                    outterRing_Rawdata = 0;
                                    int MSB = commandBuffer[6] << 8; // because the data's maximun is 1024 so needed two byte to store, this line decode the sceond byte
                                    outterRing_Rawdata += MSB;
                                    outterRing_Rawdata += commandBuffer[7];

                                    int diff = outterRing_Rawdata - LastoutterRing_Rawdata;

                                    // 轉過1023跟0之間的時候 diff會異常地大，以下算式是發現這情況的時候所做的補償，讓程式知道1023跟0其實只差1
                                    // 先把這個值叫做overCenterDetection_value
                                    if (Mathf.Abs(diff) >= overCenterDetection_value)
                                    {
                                        int temp = (-1023 * (diff / Mathf.Abs(diff))) + diff;
                                        diff = temp;
                                    }
                                    OutterRing_CalData += diff;

                                    //Debug.Log("內圈上一次偵測是 " + LastlinnerRing_Rawdata + " 現在轉到了 " + innerRing_Rawdata + " ，共差距 " + diff);
                                    //Debug.Log("現在指針 = " + InnerRing_CalData);
                                    LastoutterRing_Rawdata = outterRing_Rawdata;
                                }
                                else //起始設定
                                {
                                    // if the innerRingdata == -1 means that it's first initial
                                    // so store the initial data to 歸0
                                    int MSB = commandBuffer[6] << 8;
                                    LastoutterRing_Rawdata = 0;
                                    LastoutterRing_Rawdata += MSB;
                                    LastoutterRing_Rawdata += commandBuffer[7];
                                    //Debug.Log("LastoutterRing_Rawdata = " + LastoutterRing_Rawdata);

                                    // 硬把offset後的位置算進去
                                   // OutterRing_CalData

                                }

                                //Debug.Log(outterRing_Rawdata);

                                break;
                            }
                        case 4: // 內圈按鈕
                            {
                                
                                int PressedCommand = commandBuffer[7];
                                if(PressedCommand >= 1)
                                {
                                    try
                                    {
                                        NoteReceiver.InnerRingPressed_ThreadHandle();
                                        
                                    }
                                    catch
                                    {
                                        Debug.Log("沒有NoteReceiver，你應該是在選歌");
                                    }

                                    try
                                    {
                                        GameController_stepONE.InnerRingPressed_ThreadHandle();
                                        GameController_stepONE.GetInSelectingSong();
                                    }
                                    catch
                                    {
                                        Debug.Log("沒有GameController_stepONE，你應該是在遊戲");
                                    }

                                   // Debug.Log("iNNERrING pRESSED");
                                }
                                break;
                            }
                        case 5: // 外圈按鈕
                            {
                                int PressedCommand = commandBuffer[7];
                                if (PressedCommand >= 1)
                                {
                                    try
                                    {
                                        NoteReceiver.OutterRingPressed_ThreadHandle();
                                    }
                                    catch
                                    {
                                        Debug.Log("沒有NoteReceiver，你應該是在選歌");
                                    }

                                    try
                                    {
                                        GameController_stepONE.OutterRingPressed_ThreadHandle();
                                    }
                                    catch
                                    {
                                        Debug.Log("沒有GameController_stepONE，你應該是在遊戲");
                                    }

                                    //Debug.Log("OutterING pRESSED");
                                }
                                break;
                            }
                        
                        default:
                            {
                                break;
                            }
                    }

                    // a response should be asigned above through a delegate, so we can now clear the buffer for our next incommin command
                    commandBuffer = new byte[8];
                }
                   
            }
            catch {
                //Debug.Log("reading port1 error");
                port1Reconnecting = true;
            }
        }
        
    }

    private static void SerialPort2_DataReading()
    {
        while (true)
        {

            try
            {
                // First, find the start command, in this case I like to use 77;
                byte FindingStartCmd = (byte)port2.ReadByte();

                // when finding the start command
                if (FindingStartCmd == commandSTR)
                {
                    // lets get the start command into our buffer first
                    commandBuffer2[0] = commandSTR;
                    String commandShow2 = "";
                    commandShow2 += commandBuffer2[0];

                    // and read the remain data, because I already determined that I'm using 8 byte data length, I know I need to read 7 more byte;
                    port2.Read(commandBuffer2, 1, 7);

                    // get those data into a string for a easy debug
                    for (int i = 1; i < commandBuffer2.Length; i++)
                    {
                        commandShow2 += "|";
                        commandShow2+= commandBuffer2[i].ToString();

                    }
                    //Debug.Log(commandShow2);

                    // now we know what command we received, Let's assign some task

                    // 待做----//

                    switch (commandBuffer2[5])
                    {
                        case 6: //確認選歌
                            {
                                int chooseSongokCommand = commandBuffer2[7];
                                if (chooseSongokCommand >= 1)
                                {


                                    try
                                    {
                                        GameController_stepONE.CanGoInGame_ThreadHandle();
                                    }
                                    catch
                                    {
                                        Debug.Log("沒有GameController_stepONE，你應該是在遊戲");
                                    }



                                    Debug.Log("收到進遊戲的指令");
                                }
                                Debug.Log("收到進遊戲的指令");
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                    // a response should be asigned above through a delegate, so we can now clear the buffer for our next incommin command
                    commandBuffer2 = new byte[8];
                }

            }
            catch
            {
                //Debug.Log("reading port2 error");
                port2Reconnecting = true;
            }
        }

    }


    private static void Connection1_Establishing()
    {
        while (port1Reconnecting)
        {
           
            try
            {
                
                port1.Open();
                //Debug.Log("port1 opened");
                port1Reconnecting = false ;
               
            }
            catch
            {
                Debug.Log("Failed Open Port1 ");
               


            }
            
        }
        
    }

    private static void Connection2_Establishing()
    {
        while (port2Reconnecting)
        {

            try
            {

                port2.Open();
                //Debug.Log("port2 opened");
                port2Reconnecting = false;

            }
            catch
            {
                Debug.Log("Failed Open Port2 ");



            }

        }

    }

    // ----------送指令給觸控板 ----------
    // 顯示"選歌"字樣
    public static void TBSend_ShowSelectSongText()
    {
        byte[] SSByte = new byte[] { 5, 2, 2, 2, 2, 2, 2, 2 };
        port2.Write(SSByte, 0, 8);
    }
    // 進入選歌確認滑動模式
    public static void TBSend_EnterChooseSongSlide()
    {
        byte[] CCByte = new byte[] { 7, 2, 2, 2, 2, 2, 2, 2 };
        port2.Write(CCByte, 0, 8);
    }
    // 進入遊戲開始，顯示背景圖"OwO"
    public static void TBSend_GoBackGround()
    {
        byte[] CCByte = new byte[] { 6, 2, 2, 2, 2, 2, 2, 2 };
        port2.Write(CCByte, 0, 8);
    }

    // 最重要的送觸控板指令
    public static void TBSend_Touch(byte[] touchboardCommand)
    {
        byte[] CCByte = touchboardCommand;
        port2.Write(CCByte, 0, 8);
    }


    // 關遊戲的時候斷開連結
    void OnDestroy()
    {
        connectionThread.Abort();

        readThread.Abort();
        port1.Close();

        connectionThread2.Abort();
        readThread2.Abort();
        port2.Close();

        Debug.Log("Stopped port thread");
    }
}
