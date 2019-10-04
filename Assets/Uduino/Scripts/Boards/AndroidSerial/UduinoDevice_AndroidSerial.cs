using System;

namespace Uduino
{
    public class UduinoDevice_AndroidSerial : UduinoDevice
    {

        //TODO : faire les fonctions set Rea
        public UduinoDevice_AndroidSerial() : base() { }

        public UduinoDevice_AndroidSerial(UduinoConnection connection) : base()
        {
            _connection = connection;
            this.identity = "androidBoard";
        }

        /// <summary>
        /// Open a specific serial port
        /// </summary>
        public override void Open()
        {
            boardStatus = BoardStatus.Open;
            Log.Info("Opening stream on port <color=#2196F3>[" +  "" + "]</color>");
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
            lock (writeQueue)
            {
                if (writeQueue.Count == 0)
                    return false;

                string message = (string)writeQueue.Dequeue();
                if (!message.EndsWith("\r\n")) message += "\r\n";

                try
                {
                    try
                    {
                        _connection.PluginWrite(message);
                    }
                    catch (Exception)
                    {
                        writeQueue.Enqueue(message);
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Error on port <color=#2196F3>[" + "" + "]</color> : " + e);
                    // Close();
                    return false;
                }
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
        public override string ReadFromArduino(string message = null,  bool instant = false)
        {
            return base.ReadFromArduino(message, instant);
        }

        public override bool ReadFromArduinoLoop(bool forceReading = false)
        {
            return base.ReadFromArduinoLoop(forceReading);
        }
        #endregion

        #region Close
        #endregion
    }
}