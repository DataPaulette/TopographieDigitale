
using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ReadSerial : MonoBehaviour {

     SerialPort stream; //Set the port and the baud rate
     float[] lastRot = {0,0,0}; //Need the last rotation to tell how far to spin the camera
     
     
     void Start () {
           stream = new SerialPort("/dev/ttyACM0", 115200, Parity.None, 8, StopBits.One); //Set the port and the baud rate
           stream.Open(); //Open the Serial Stream.
     }
     
     // Update is called once per frame
     void Update () {
	Debug.Log(stream.ReadLine());
                 //stream.BaseStream.Flush(); //Clear the serial information so we assure we get new information.
     }
}