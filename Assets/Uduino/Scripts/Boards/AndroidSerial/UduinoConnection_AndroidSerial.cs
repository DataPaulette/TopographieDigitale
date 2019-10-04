using UnityEngine;

namespace Uduino
{
    public class UduinoConnection_AndroidSerial : UduinoConnection
    {
        AndroidJavaClass _class = null;
        AndroidJavaObject androidPlugin { get { return _class.GetStatic<AndroidJavaObject>("instance"); } }
        UduinoCommunication_AndroidSerial communicationController = null;

        string javaClassName = "com.mteys.uduinoserial.Serial_Connection";

        public UduinoConnection_AndroidSerial() : base() { }

        public override void FindBoards(UduinoManager manager)
        {
            base.FindBoards(manager); // Add reference to manager

            CreateDebugCanvas();
            Discover();
        }

        void InitCommunication(string communicationGoName)
        {
            _class = new AndroidJavaClass(javaClassName); //The arduino baord should be found automatically
            _class.CallStatic("start", communicationGoName);
            Debug.Log("Starting service with name " + communicationGoName);
            Debug.Log("Setting baud rate to:" + _manager.BaudRate);
            androidPlugin.Call("ChangeBaudRate", _manager.BaudRate);
        }

        public override void Discover()
        {
            UduinoInterface.Instance.StartSearching();

            if(UduinoCommunication_AndroidSerial.Instance == null)
            {
                communicationController = CreateCommunicationController();
                InitCommunication(communicationController.id);
                Debug.Log("Create new communication controller");
            }

            if (connectedDevice != null)
                connectedDevice.Close();

            connectedDevice = null;

            connectedDevice = OpenUduinoDevice("arduinoDevice");
            connectedDevice.Open();
            DetectUduino(connectedDevice);
        }

        UduinoCommunication_AndroidSerial CreateCommunicationController()
        {
            string randName = "AndroidCommunication" + Random.Range(0, 100);
            UduinoCommunication_AndroidSerial communication = new GameObject(randName).AddComponent<UduinoCommunication_AndroidSerial>();
            communication.connection = this;
            communication.id = randName;
            return communication;
        }

        public override UduinoDevice OpenUduinoDevice(string id)
        {
            return new UduinoDevice_AndroidSerial(this);
        }

        public override void PluginWrite(string message)
        {
            androidPlugin.Call("writeToArduino", message);
            Log.Info("<color=#4CAF50>" + message + "</color> sent to <color=#2196F3>[" + connectedDevice.name + "]</color>");
        }

        public override void PluginReceived(string message)
        {
            connectedDevice.MessageReceived(message);
            UduinoInterface.Instance.LastReceviedValue(message);
        }

        public override void BoardFound(string name)
        {
            UduinoInterface.Instance.UduinoConnected(name);
        }

        public override bool BoardNotFound(UduinoDevice uduinoDevice)
        {
            bool isFound = base.BoardNotFound(uduinoDevice);
            if(!isFound)
                UduinoInterface.Instance.BoardNotFound("Can't find any board.");
            return isFound;
        }

    }
}