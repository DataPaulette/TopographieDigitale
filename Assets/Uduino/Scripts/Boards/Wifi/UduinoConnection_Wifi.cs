using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UDUINO_READY
using System.IO.Ports;
#endif

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Uduino
{
#if UDUINO_READY

    public class UduinoConnection_Wifi : UduinoConnection
    {
        public UdpClient udpClient;
        public IPEndPoint remoteIpEndPoint;
        public IPEndPoint serverEndpoint;

        public UduinoConnection_Wifi() : base()
        {
            StartNetwork();
        }

        public override void FindBoards(UduinoManager manager)
        {
            base.FindBoards(manager);
            Discover();
        }

        void StartNetwork()
        {
            try
            {
                udpClient = new UdpClient(UduinoManager.Instance.uduinoWifiPort);
                udpClient.Client.SendTimeout = UduinoManager.Instance.writeTimeout;
                udpClient.Client.ReceiveTimeout = UduinoManager.Instance.readTimeout;

                serverEndpoint = new IPEndPoint(
                    IPAddress.Parse(UduinoManager.Instance.uduinoIpAddress),
                   UduinoManager.Instance.uduinoWifiPort);
                remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                //    ar_ = udpClient.BeginReceive(Receive, new object());
            }
            catch (Exception e)
            {
                Debug.Log("Failed to listen for UDP at port " + UduinoManager.Instance.uduinoWifiPort + ": " + e.Message);
                return;
            }
        }

        public override void Discover()
        {
            UduinoDevice tmpDevice = OpenUduinoDevice("");
            tmpDevice.Open();
            DetectUduino(tmpDevice);
        }

        public override UduinoDevice OpenUduinoDevice(string id)
        {
            return new UduinoDevice_Wifi(this);
        }

        public override void Stop()
        {
            udpClient.Close();
            serverEndpoint = null;
            remoteIpEndPoint = null;
        }
    }

#else
    public class UduinoConnection_Wifi : MonoBehaviour
    {
        [Header("You need to download Uduino first")]
        public string downloadUduino = "https://www.assetstore.unity3d.com/#!/content/78402";
    }
#endif
}