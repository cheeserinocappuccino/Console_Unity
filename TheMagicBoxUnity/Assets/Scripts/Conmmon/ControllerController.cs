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
    [SerializeField]
    private string[] coms;
    public string useport;
    public int baudrate;
    public int databits;
    public StopBits stopbits;
    public Parity parity;
    public static byte commandSTR = 77;

    static SerialPort port1;
    Thread readThread = new Thread(SerialPort_DataReading);


    static byte[] commandBuffer = new byte[8];



    void Awake()
    {
       
        port1 = new SerialPort(useport, baudrate, parity, databits, stopbits);

        try
        {
      
            port1.Open();
            readThread.Start();

            Debug.Log("Port OPENED");
        }
        catch
        {
            Debug.Log("Port OPEN FAILED");
        }

     
        //readThread.Join();
    }


    void FixedUpdate()
    {
        
     

    }

    // run a another thread to prevent performance issue
    // Other's example often join this thread back to the main thread, but we are kinda polling commands and we need to always polling
    // so maybe It can be here, as long as we don't accidently create more thread;
    private static void SerialPort_DataReading()
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
                    Debug.Log(commandShow);

                    // now we know what command we received, Let's assign some task

                    // 待做----//

                    // a response should be asigned above through a delegate, so we can now clear the buffer for our next incommin command
                    commandBuffer = new byte[8];
                }
                   
            }
            catch (TimeoutException) {
                
            }
        }
        
    }
}
