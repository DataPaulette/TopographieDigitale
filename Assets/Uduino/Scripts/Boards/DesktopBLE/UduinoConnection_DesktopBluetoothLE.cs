using System.Collections.Generic;
using UnityEngine;

namespace Uduino
{
    public class UduinoConnection_DesktopBluetoothLE : UduinoConnection
    {
        UduinoCommunication_DesktopBluetoothLE communicationController = null;
        public bool bluetoothStarted = false;
        Dictionary<string, string> availableDevices = new Dictionary<string, string>();
        public UduinoConnection_DesktopBluetoothLE() : base() { }

        public override void FindBoards(UduinoManager manager)
        {
            base.FindBoards(manager); // Add reference to manager
            Discover();
            UduinoInterface.Instance.SetConnection(this);
        }

        void InitCommunication(string communicationGoName)
        {
            Debug.Log("Starting service with name " + communicationGoName);
            communicationController.InitPlugin(communicationGoName);
        }
       
        public override void ScanForDevices()
        {
            availableDevices.Clear();
            Debug.Log("Scan for devices desktop");
            UduinoInterface.Instance.StartSearching();
            communicationController.ScanForDevice();
        }


        #region Scanning
        public override void Discover()
        {
            if (UduinoCommunication_DesktopBluetoothLE.Instance == null)
            {
                communicationController = CreateCommunicationController();
                InitCommunication(communicationController.id);
            }
            if (bluetoothStarted)
                ScanForDevices();
        }

        public void DeviceFound(string name, string uuid)
        {
            if (!availableDevices.ContainsKey(name))
            {
                availableDevices.Add(name, uuid);
                UduinoInterface.Instance.AddDeviceButton(name, uuid);
            }
        }

        public void List()
        {
            GetListOfDevices();
        }

        public bool SearchDevicesDidFinish()
        {
            bool searchDevicesDidFinish = false;
            //TODO     searchDevicesDidFinish = androidPlugin.Call<bool>("_SearchDeviceDidFinish");
            return searchDevicesDidFinish;
        }

        public string GetListOfDevices()
        {
            string listOfDevices = "";
            Debug.Log("listOfDevices" + listOfDevices);
            return listOfDevices;
        }
        #endregion

        #region Device connection 
        public bool IsDeviceConnected()
        {
            bool isConnected = false;
            //TODO   isConnected = androidPlugin.Call<bool>("_IsDeviceConnected");
            return isConnected;
        }

        public override bool ConnectPeripheral(string peripheralID, string name)
        {
            bool result = false;
            if (connectedDevice == null)
            {
                communicationController.ConnectToDevice(peripheralID);
                UduinoInterface.Instance.UduinoConnecting(name);
            } else
            {
                Log.Debug("A board is already trying to be connected");
            }
            return result;
        }

        public void BoardConnected(string identity)
        {
            connectedDevice = OpenUduinoDevice(identity);
            connectedDevice.Open();
            DetectUduino(connectedDevice);
        }
        #endregion

        #region Disconnect
        public void DisconnectedFromSource()
        {
            if (connectedDevice != null)
            {
                UduinoInterface.Instance.UduinoDisconnected(connectedDevice.identity);
                UduinoManager.Instance.CloseDevice(connectedDevice);
                connectedDevice = null;

                ScanForDevices();
            } else
            {
            }
        }

        public override bool Disconnect()
        {
            communicationController.DisconnectFromDevice();
            //communicationController.Destroy();
            Debug.Log("Should destroy here");
            return true;
        }

        public override bool BoardNotFound(UduinoDevice uduinoDevice)
        {
            if (connectedDevice.getStatus() != BoardStatus.Found)
            {
                Log.Debug("Impossible to get name on <color=#2196F3>[" + connectedDevice.identity + "]</color>. Closing.");
                connectedDevice.Close();
                communicationController.DisconnectBoard();
              //  uduinoDevice = null;
            }
            return false;
        }
        #endregion

        #region Communication 
        public string GetData()
        {
            string result = null;
            //TODO result = androidPlugin.Call<string>("_GetData");
            return result;
        }
        #endregion

        UduinoCommunication_DesktopBluetoothLE CreateCommunicationController()
        {
            string randName = "DesktopBLECommunication";
            UduinoCommunication_DesktopBluetoothLE communication = null;
            GameObject obj =  GameObject.Find(randName);
            if (obj != null)
                communication = obj.GetComponent<UduinoCommunication_DesktopBluetoothLE>();
            else communication = new GameObject(randName).AddComponent<UduinoCommunication_DesktopBluetoothLE>();
            communication.connection = this;
            communication.id = randName;
            return communication;
        }

        public override UduinoDevice OpenUduinoDevice(string name)
        {
            return new UduinoDevice_DesktopBluetoothLE(this, name);
        }

        public override void PluginWrite(string message)
        {
            if (!message.EndsWith("\r\n")) message += "\r\n";
            communicationController.SendData(message);
            Log.Debug("<color=#4CAF50>" + message.RemoveLineEndings() + "</color> sent to <color=#2196F3>[" + (connectedDevice.name == "" ? connectedDevice.identity : connectedDevice.name) + "]</color>");
        }

        public override void PluginReceived(string message)
        {
            if (connectedDevice != null)
            {
                UduinoInterface.Instance.LastReceviedValue(message);
             //   connectedDevice.AddToArduinoReadQueue(message);
                if (connectedDevice.boardStatus == BoardStatus.Open)
                {
                    Debug.LogError("should not be here");
                }
                else if (connectedDevice.boardStatus == BoardStatus.Found || connectedDevice.boardStatus == BoardStatus.Finding)
                {
                    connectedDevice.MessageReceived(message);
                }
                else
                {
                    //TODO : What ?
                }
            }
        }
    }
}