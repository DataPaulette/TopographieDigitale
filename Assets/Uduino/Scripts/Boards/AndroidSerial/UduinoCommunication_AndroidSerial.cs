using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Uduino {

    public class UduinoCommunication_AndroidSerial : MonoBehaviour {

        public UduinoConnection connection = null;
        public static UduinoCommunication_AndroidSerial Instance = null;
        public string id = null;

        public void Awake()
        {
            Instance = this;
        }

        public void DeviceDetected()
        {
            Debug.Log("DEEEEEEEEEE");
        }

        public void CallUnityEvent(string s)
        {
            Log.Debug("UnityEvent: " + s);
        }

        public void PluginMessageReceived(string s)
        {
            connection.PluginReceived(s);
        }

        public void BoardConnected(string s)
        {
            Debug.Log("Board detected");
            if (UduinoManager.Instance.autoDiscover)
                UduinoManager.Instance.DiscoverPorts();
        }

        public void BoardDisconnected(string s)
        {
            Log.Debug("USB Disconnected");
            connection.CloseDevices();
          //  UduinoInterface_Serial.Instance.DisconnectDevice();
        }
    }
}