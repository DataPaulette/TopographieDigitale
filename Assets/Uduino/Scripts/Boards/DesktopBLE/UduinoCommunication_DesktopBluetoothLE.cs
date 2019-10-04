using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Uduino
{
    public class UduinoCommunication_DesktopBluetoothLE : MonoBehaviour
    {
        public UduinoConnection_DesktopBluetoothLE connection = null;
        public static UduinoCommunication_DesktopBluetoothLE Instance = null;
        public string id = null;

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEConnectCallbacks(
        [MarshalAs(UnmanagedType.FunctionPtr)]SendBluetoothMessageDelegate sendMessage,
        [MarshalAs(UnmanagedType.FunctionPtr)]DebugDelegate log,
        [MarshalAs(UnmanagedType.FunctionPtr)]DebugDelegate warning,
        [MarshalAs(UnmanagedType.FunctionPtr)]DebugDelegate error);
        public delegate void DebugDelegate([MarshalAs(UnmanagedType.LPStr)]string message);
        public delegate void SendBluetoothMessageDelegate([MarshalAs(UnmanagedType.LPStr)]string gameObjectName, [MarshalAs(UnmanagedType.LPStr)]string methodName, [MarshalAs(UnmanagedType.LPStr)]string message);

        private static DebugDelegate _LogDelegate;
        private static DebugDelegate _WarningDelegate;
        private static DebugDelegate _ErrorDelegate;
        private static SendBluetoothMessageDelegate _SendMessageDelegate;

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLELog([MarshalAs(UnmanagedType.LPStr)]string message);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEInitialize(bool asCentral, bool asPeripheral, string goName);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEDeInitialize();

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEPauseMessages(bool isPaused);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEScanForPeripheralsWithServices([MarshalAs(UnmanagedType.LPStr)]string serviceUUIDsString, bool allowDuplicates, bool rssiOnly, bool clearPeripheralList);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLERetrieveListOfPeripheralsWithServices([MarshalAs(UnmanagedType.LPStr)]string serviceUUIDsString);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEStopScan();

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEConnectToPeripheral([MarshalAs(UnmanagedType.LPStr)]string name);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEDisconnectPeripheral([MarshalAs(UnmanagedType.LPStr)]string name);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEReadCharacteristic([MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string service, [MarshalAs(UnmanagedType.LPStr)]string characteristic);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEWriteCharacteristic([MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string service, [MarshalAs(UnmanagedType.LPStr)]string characteristic, byte[] data, int length, bool withResponse);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLESubscribeCharacteristic([MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string service, [MarshalAs(UnmanagedType.LPStr)]string characteristic);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEUnSubscribeCharacteristic([MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string service, [MarshalAs(UnmanagedType.LPStr)]string characteristic);

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _uduinoBluetoothLEDisconnectAll();

        [DllImport("UduinoWinBluetoothLE")]
        private static extern void _threadLoop();


        string serviceGUID = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
        string subscribeCharacteristic = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";
        string writeCharacteristic = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";


        static readonly Queue<Action> incommingMessages = new Queue<Action>();

        public bool ConnectToDevice(string name)
        {
            _uduinoBluetoothLEConnectToPeripheral(name);
            return true;
        }

        public void ScanForDevice()
        {
            _uduinoBluetoothLEScanForPeripheralsWithServices(serviceGUID, false, false, true);
        }

        public void DeviceDetected(string message)
        {
            string [] content = message.Split('|');
            connection.DeviceFound(content[1], content[0]);
            UduinoInterface.Instance.StopSearching();
        }

        public void OnBluetoothMessage(string message)
        {
            Debug.Log("Message received " + message);
        }

        public void InitPlugin(string goName)
        {
            _uduinoBluetoothLEConnectCallbacks(UnitySendMessageWrapper, DebugLog, DebugLogWarning, DebugLogError);
            _uduinoBluetoothLEInitialize(true, false, goName);
        }


        #region to remove
        public static void Log(string message)
        {
            {
                _uduinoBluetoothLELog(message);
            }
        }

        private void Update()
        {
            _threadLoop();

            lock (incommingMessages)
            {
                while (incommingMessages.Count > 0)
                {
                    incommingMessages.Dequeue().Invoke();
                }
            }
        }

        private static void Enqueue(System.Action action)
        {
            lock (incommingMessages)
            {
                incommingMessages.Enqueue(action);
            }
        }

        private  void UnitySendMessageWrapper(string gameObjectName, string methodName, string message)
        {
            Enqueue(() =>
            {
                var communication = GameObject.Find(gameObjectName);
                if (communication != null)
                {
                    if (message == "")
                        communication.SendMessage(methodName);
                    else
                        communication.SendMessage(methodName, message);
                }
            });
        }

        private static void DebugLog(string message)
        {
            Debug.Log(message);
        }

        private static void DebugLogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        private static void DebugLogError(string message)
        {
            Debug.LogError(message);
        }
#endregion

        public void Awake()
        {
            Instance = this;
        }

        public void DeviceDetected()  { }
  
        public void PluginMessageReceived(string s)
        {
           connection.PluginReceived(s);
        }
  
        public void DisconnectBoard()
        {
            if (connection.connectedDevice != null)
            {
                DisconnectFromDevice();
                Enqueue(() =>
                {
                    connection.CloseDevices();
                });
            } else
            {
                // This should not happend.
            }
        }

        public void DisconnectFromDevice()
        {
            UnSubscribeRead();
            _uduinoBluetoothLEDisconnectPeripheral(connection.connectedDevice.identity);
        }

        public void SendData(string data)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(data);
            _uduinoBluetoothLEWriteCharacteristic(connection.connectedDevice.identity, serviceGUID, subscribeCharacteristic, bytes, bytes.Length, false);
        }

        void SubscribeRead()
        {
           _uduinoBluetoothLESubscribeCharacteristic(connection.connectedDevice.identity, serviceGUID, writeCharacteristic);
        }

        void UnSubscribeRead()
        {
            Debug.Log("<color=#ff0000>unsubscriberead</color>");
            _uduinoBluetoothLEUnSubscribeCharacteristic(connection.connectedDevice.identity, serviceGUID, writeCharacteristic);
        }

        void BleInitialized(string message)
        {
            Debug.Log("OnBleDidInitialize" + message);

            if (message == "Success")
            {
                connection.bluetoothStarted = true;
                connection.Discover();
            }
            else
            {
                UduinoInterface.Instance.DisplayError("Cannot start BLE: " + message);
            }
        }

        void BoardConnected(string name) // TODO : renommer en BoardConnected (comme sur androidSerial)
        {
            if (name != "")
            {
               connection.BoardConnected(name);
               SubscribeRead();
            }
        }

        void BoardDisconnectedFromSource(string message)
        {
            Debug.Log(message);
            if (connection.connectedDevice != null && message == connection.connectedDevice.identity)
                connection.DisconnectedFromSource();
        }

        private void OnApplicationQuit()
        {
        }
    }
}