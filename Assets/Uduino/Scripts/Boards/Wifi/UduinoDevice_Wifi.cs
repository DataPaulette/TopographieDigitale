using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Uduino
{
#if UDUINO_READY
    public class UduinoDevice_Wifi : UduinoDevice
    {
        new UduinoConnection_Wifi _connection = null;

        //TODO : faire les fonctions set Rea
        public UduinoDevice_Wifi(UduinoConnection_Wifi connection) : base()
        {
            this._connection = connection;
            this.identity = UduinoManager.Instance.uduinoIpAddress;
        }

        public override void UduinoFound()
        {
            base.UduinoFound();
            UduinoInterface.Instance.AddDeviceButton(name);
        }

        #region Commands
        /// <summary>
        /// Loop every thead request to write a message on the arduino (if any)
        /// </summary>
        public override bool WriteToArduinoLoop()
        {
            commandhasBeenSent = false;
            //TODO : if is connected
            //  if (serial == null || !serial.IsOpen)
            //       return false;
            lock (writeQueue)
            {
                if (writeQueue.Count == 0)
                    return false;

                string message = (string)writeQueue.Dequeue();
                try
                {
                    Byte[] sendBytes = Encoding.UTF8.GetBytes(message);
                    _connection.udpClient.Send(sendBytes, sendBytes.Length, _connection.serverEndpoint);
                    Log.Info("<color=#4CAF50>" + message + "</color> sent to <color=#2196F3>[" + (name != "" ? name : identity) + "]</color>", true);
                }
                catch (Exception e)
                {
                    Log.Warning("Impossible to send the message <color=#4CAF50>" + message + "</color> to <color=#2196F3>[" + UduinoManager.Instance.uduinoIpAddress + "]</color>: " + e, true);
                    return false;
                }

                commandhasBeenSent = true;
                WritingSuccess(message);
            }
            return true;
        }

        /// <summary>
        /// Read Arduino serial port
        /// </summary>
        /// <param name="message">Write a message to the serial port before reading the serial</param>
        /// <param name="instant">Read the message value now and not in the thread loop</param>
        /// <returns>Read data</returns>
        public override string ReadFromArduino(string message = null, bool instant = false)
        {
            //TODO : if is open
            //       if (serial == null || !serial.IsOpen)
            //       return null;
            return base.ReadFromArduino(message, instant);
        }

        public override bool ReadFromArduinoLoop(bool forceReading = false)
        {            //TODO : if is open
                     //   if (serial == null || !serial.IsOpen)
                     //    return false;

            if (!base.ReadFromArduinoLoop(forceReading))
                return false; // If the conditions don't match, we return here

            try
            {
                string tempBuffer = "";
                try
                {
                    for(int i=0; i<25;i++)
                    {
                        Byte[] receiveBytes = _connection.udpClient.Receive(ref _connection.remoteIpEndPoint); // Blocks until a message returns on this socket from a remote host.
                        string returnData = Encoding.UTF8.GetString(receiveBytes);
                        tempBuffer += returnData;
                        if (tempBuffer.EndsWith("\r\n"))
                        {
                            string readedLine = tempBuffer.TrimEnd(Environment.NewLine.ToCharArray());
                            MessageReceived(readedLine);
                            return true;
                        }
                    }
                    MessageReceived(tempBuffer);
                    return true;
                }
                catch (SocketException e)
                {
                    if (boardStatus == BoardStatus.Found)
                    {
                        if (e.ErrorCode != 10004)
                            Log.Debug("ReadTimeout. Are you sure something is sent? \n" + e);
                        else Log.Debug(e);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return false;
        }
        #endregion

        #region Close
        /// <summary>
        /// Close Serial port 
        /// </summary>
        public override void Close()
        {
            base.Close();
            Log.Warning("Closing connection with <color=#2196F3>[" + UduinoManager.Instance.uduinoIpAddress + "]</color>");

            // if (serial != null && serial.IsOpen)
            {
                boardStatus = BoardStatus.Closed;
            }
        }
        #endregion
    }
#else
    public class UduinoDevice_Wifi : MonoBehaviour
    {
        [Header("You need to download Uduino first")]
        public string downloadUduino = "https://www.assetstore.unity3d.com/#!/content/78402";
    }

#endif
}