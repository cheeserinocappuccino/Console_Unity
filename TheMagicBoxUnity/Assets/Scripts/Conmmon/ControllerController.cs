using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;
using System.Data;
using System.Threading.Tasks;


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

    void Awake()
    {
       
        port1 = new SerialPort(useport, baudrate, parity, databits, stopbits);

        connectionThread.Start();
        readThread.Start();

        /*connectionThread2.Start();
        readThread2.Start();*/

        //InnerRing_CalData = new UInt16[1024];
        //OutterRing_CalData = new UInt16[1024];
       
    }


    // run a another thread to prevent performance issue
    // Other's example often join this thread back to the main thread, but we are kinda polling commands and we need to always polling
    // so maybe It can be here, as long as we don't accidently create more thread;
    private static void SerialPort1_DataReading()
    {
        while (true)
        {
            
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

                                    Debug.Log("內圈上一次偵測是 " + LastlinnerRing_Rawdata + " 現在轉到了 " + innerRing_Rawdata + " ，共差距 " + diff);
                                    Debug.Log("現在指針 = " + InnerRing_CalData);
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
                    port1.Read(commandBuffer2, 1, 7);

                    // get those data into a string for a easy debug
                    for (int i = 1; i < commandBuffer2.Length; i++)
                    {
                        commandShow2 += "|";
                        commandShow2+= commandBuffer2[i].ToString();

                    }
                    //Debug.Log(commandShow2);

                    // now we know what command we received, Let's assign some task

                    // 待做----//

                    // a response should be asigned above through a delegate, so we can now clear the buffer for our next incommin command
                    commandBuffer = new byte[8];
                }

            }
            catch
            {
                //Debug.Log("reading port2 error");
                port1Reconnecting = true;
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
                //Debug.Log("Failed Open Port1 ");
               


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
                port1Reconnecting = false;

            }
            catch
            {
               // Debug.Log("Failed Open Port2 ");



            }

        }

    }

    void OnDestroy()
    {
        connectionThread.Abort();
       
        readThread.Abort();
        Debug.Log("Stopped port thread");
    }
}
