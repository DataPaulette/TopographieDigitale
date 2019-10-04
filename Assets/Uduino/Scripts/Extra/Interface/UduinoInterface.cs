using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Uduino
{
   
    public class UduinoInterface : MonoBehaviour
    {
        #region Singleton
        public static UduinoInterface Instance
        {
            get {
                Debug.Log(Application.isPlaying);

                if (_instance != null)
                    return _instance;
                UduinoInterface[] _interfaces = FindObjectsOfType(typeof(UduinoInterface)) as UduinoInterface[];
                if (_interfaces.Length == 0)
                    return CreateInterface();
                else
                {
                    Debug.Log(Application.isPlaying);
                    if (Application.isPlaying && _interfaces[0].currentInterfaceType == UduinoInterfaceType.None && UduinoManager.Instance.interfaceType != UduinoInterfaceType.None) { 
                        _interfaces[0].Destroy();
                        return CreateInterface();
                    } else 
                        return _interfaces[0];
                }
            } set {
                if (_instance == null) {
                    _instance = value;
                    value.transform.SetParent(UduinoManager.Instance.transform);
                } else {
                    Log.Warning("You can only use one Interface. Destroying the Interface attached to the GameObject " + value.gameObject.name);
                    Destroy(value.gameObject);
                }
            }
        }
        private static UduinoInterface _instance = null;
        #endregion
        [HideInInspector]
        public UduinoInterfaceType currentInterfaceType = UduinoInterfaceType.None;

        public static UduinoInterface CreateInterface()
        {
            UduinoInterface targetInterface = null;
            /*
            if (!Application.isPlaying)
            {
                return UduinoManager.Instance.gameObject.AddComponent<UduinoInterface_Empty>();
            }
            */
            switch (UduinoManager.Instance.interfaceType)
            {
                case UduinoInterfaceType.None:
                    GameObject g = new GameObject("UduinoInterface");
                    targetInterface = g.AddComponent<UduinoInterface_Empty>();
                    g.transform.SetParent(UduinoManager.Instance.transform);
                    targetInterface.currentInterfaceType = UduinoInterfaceType.None;
                    break;
                default:
                    bool useBLE = false;
#if UNITY_EDITOR || UNITY_STANDALONE
                    if (UduinoManager.Instance.ExtensionIsPresentAndActive("UduinoDevice_DesktopBluetoothLE"))
                        useBLE = true;
#elif UNITY_ANDROID
                   if (UduinoManager.Instance.ExtensionIsPresentAndActive("UduinoDevice_AndroidBluetoothLE"))
                        useBLE = true;
#endif
                    GameObject tmpInterface = null;
                    if (useBLE) {
                        tmpInterface = Instantiate(Resources.Load("UduinoInterface_Bluetooth")) as GameObject;
                    }
                    else {
                        tmpInterface = Instantiate(Resources.Load("UduinoInterface")) as GameObject;
                    }
                    targetInterface = tmpInterface.GetComponent<UduinoInterface>();
                    tmpInterface.gameObject.name = "UduinoInterface";
                    tmpInterface.transform.SetParent(UduinoManager.Instance.transform);
                    break;
            }
            return targetInterface;
        }

#region Public variables
        public UduinoConnection boardConnection = null;

        [Header("Full panel")]
        public GameObject fullUI;
        public GameObject errorPanel;
        public GameObject fullDevicePanel;
        public GameObject scanButtonFull;
        public GameObject notFound;
        public GameObject boardButton;

        [Header("Minimal Panel")]
        public GameObject minimalUI;
        public GameObject minimalErrorPanel;
        public GameObject minimalDevicePanel;
        public GameObject minimalScanButton;
        public GameObject minimalNotFound;
        public GameObject minimalBoardButton;

        [Header("Debug Panel")]
        public GameObject debugPanel;
        public Text sendValue;
        public Text lastReceivedValue;
#endregion

        void Awake()
        {
            OnAwake();
        }

        public void Create() { }

        public virtual void OnAwake()
        {
            Instance = this;
            switch (UduinoManager.Instance.interfaceType)
            {
                case UduinoInterfaceType.Full:
                    minimalUI.SetActive(false);
                    fullUI.SetActive(true);
                    break;
                case UduinoInterfaceType.Minimal:
                    minimalUI.SetActive(true);
                    fullUI.SetActive(false);
                    break;
                case UduinoInterfaceType.None:
                    //  CreateInterface();
                    DestroyImmediate(this.gameObject);
                    CreateInterface();
                    return;
            }
            debugPanel.SetActive(false);
            getDeviceButtonPrefab().SetActive(false);
        }

        public virtual void SetConnection(UduinoConnection connection)
        {
            boardConnection = connection;
        }

        public virtual void AddDeviceButton(string name, string uuid = "")
        {
            if (UduinoManager.Instance.interfaceType == UduinoInterfaceType.None)
                return;

            GameObject deviceBtn = GameObject.Instantiate(getDeviceButtonPrefab(), getPanel());
            deviceBtn.transform.name = name;
            deviceBtn.transform.Find("DeviceName").transform.GetComponent<Text>().text = name;
           // Button btn = deviceBtn.GetComponent<Button>();

            deviceBtn.SetActive(true);
            NoDeviceFound(false);

           deviceBtn.transform.Find("Disconnect").GetComponent<Button>().onClick.AddListener(() => this.DisconnectUduino(name));
        }

        public virtual void SendCommand(string t)
        {

            Debug.Log("Wrong !! ");
            boardConnection.PluginWrite(t + "\r\n");
        }

        public virtual void Read()
        {

        }

        public virtual void SendValue()
        {
            boardConnection.PluginWrite(sendValue.text);
        }

        public virtual void LastReceviedValue(string value)
        {
            lastReceivedValue.text = value;
        }

        public virtual void StartSearching()
        {
            NoDeviceFound(false);
            getScanButton().text = "Finding boards...";
        }

        public virtual void StopSearching()
        {
            getScanButton().text = "Discover Boards";
        }

#region Getting elements from differents UIs
        public Text getScanButton() {
            return UduinoManager.Instance.interfaceType == UduinoInterfaceType.Full ?
                    scanButtonFull.transform.Find("ScanText").GetComponent<Text>() :
                    minimalScanButton.transform.Find("ScanText").GetComponent<Text>();
        }
        public Slider getScanSlider() {
            return UduinoManager.Instance.interfaceType == UduinoInterfaceType.Full ?
                    scanButtonFull.transform.Find("Slider").GetComponent<Slider>() :
                    minimalScanButton.transform.Find("Slider").GetComponent<Slider>();
        }
        public GameObject getDeviceButtonPrefab() {
            return UduinoManager.Instance.interfaceType == UduinoInterfaceType.Full ? boardButton : minimalBoardButton;
        }
        public Transform getPanel() {
            return UduinoManager.Instance.interfaceType == UduinoInterfaceType.Full ?
                    fullDevicePanel.transform :
                    minimalDevicePanel.transform;
        }
        public GameObject getErrorPanel() {
            return UduinoManager.Instance.interfaceType == UduinoInterfaceType.Full ? errorPanel : minimalErrorPanel;
        }
        public GameObject getNotFound() {
            return UduinoManager.Instance.interfaceType == UduinoInterfaceType.Full ? notFound : minimalNotFound;
        }
        public GameObject getBoardButton(string name) {
            return getPanel().transform.Find(name).gameObject;
        }
#endregion


        public void Detect()
        {
            UduinoManager.Instance.DiscoverPorts();
        }

        public virtual void NoDeviceFound(bool active)
        {
            getNotFound().SetActive(active);
        }

        public virtual void DisplayError(string message)
        {
            if (message == "")
            {
                getErrorPanel().SetActive(false);
            }
            else
            {
                getErrorPanel().SetActive(true);
                getErrorPanel().transform.Find("Content").Find("ErrorMessage").Find("ErrorText").GetComponent<Text>().text = message;
            }
        }

        public virtual void DetectDevice()
        {
            boardConnection.Discover();
        }

        public virtual void BoardNotFound(string message)
        {
            StopSearching();
            debugPanel.SetActive(false);
            getNotFound().SetActive(true);
            getNotFound().GetComponent<Text>().text = message;
        }

        public virtual void DisconnectUduino(string name)
        {
            UduinoManager.Instance.CloseDevice(name);
        }

        public virtual void RemoveDeviceButton(string name)
        {
            BoardNotFound("Board disconnected");
            try
            {
                Destroy(getBoardButton(name));
            } catch(System.Exception e)
            {
                Log.Debug(e);
            }
        }

        public virtual void UduinoConnected(string name)
        {
            StopSearching();
            debugPanel.SetActive(true);
            getNotFound().SetActive(false);
        }

        public virtual void UduinoDisconnected(string name) { }

        public virtual void UduinoConnecting(string name) { }

        public void Destroy()
        {
            if (this.gameObject != null && this.gameObject != UduinoManager.Instance.gameObject)
                DestroyImmediate(this.gameObject);
            else
                DestroyImmediate(this);
        }
    }

    public class UduinoInterface_Empty : UduinoInterface
    {
        public override void OnAwake()
        {
            Instance = this;
            this.hideFlags = HideFlags.HideInInspector;
        }
        public override void SetConnection(UduinoConnection connection) { }
        public override void SendCommand(string t) { }
        public override void Read() { }
        public override void SendValue() { }
        public override void LastReceviedValue(string value) { }
        public override void StartSearching() { }
        public override void StopSearching() { }
        public override void NoDeviceFound(bool active) { }
        public override void DisplayError(string message) { }
        public override void DetectDevice() { }
        public override void BoardNotFound(string message) { }
        public override void UduinoConnected(string name) { }
        public override void DisconnectUduino(string name) { }
        public override void RemoveDeviceButton(string name) { }
        public override void UduinoDisconnected(string name) { }
        public override void UduinoConnecting(string name) { }
    }
}