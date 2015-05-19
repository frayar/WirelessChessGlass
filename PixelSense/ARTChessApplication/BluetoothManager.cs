// http://www.nudoq.org/#!/Projects/32feet.NET
// http://stackoverflow.com/questions/16802791/pair-bluetooth-devices-to-a-computer-with-32feet-net-bluetooth-library

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Windows.Forms;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace ARTChessApplication
{
    class BluetoothManager
    {
        /// <summary>
        /// Attributes
        /// </summary>
        private static Guid UUID = new Guid("{00001101-0000-1000-8000-00805F9B34FB}");
        private static string LOCAL_ADDRESS = "90:A4:DE:A1:82:98";          // PIXELSENSE 
        //private static string DEVICE_ADDESS = "30:76:6F:7E:F5:C6";        //
        //private static string DEVICE_ADDESS = "43:29:1A:00:00:00";          // LG P970
        private static string DEVICE_ADDESS = "88:33:14:40:2E:2A";          // BT-200
        //private static string DEVICE_ADDESS = "D8:90:E8:EC:89:47";          // GALAXY S4
        private static string PAIRING_PIN = "0000";

        private BluetoothEndPoint EP;
        private BluetoothClient BC;
        private BluetoothDeviceInfo BTDevice;                               // Remote device that would connect (direct connection, no scanning)
        private NetworkStream stream = null;


        /// <summary>
        /// Constructor
        /// </summary>
        public BluetoothManager()
        {
            EP = new BluetoothEndPoint(BluetoothAddress.Parse(LOCAL_ADDRESS), BluetoothService.SerialPort);
            BC = new BluetoothClient(EP);
            BTDevice = new BluetoothDeviceInfo(BluetoothAddress.Parse(DEVICE_ADDESS));
            stream = null;
        }


        /// <summary>
        /// Ask for connection
        /// </summary>
        public void Connect()
        {
            // Ask for pairing
            if (BluetoothSecurity.PairRequest(BTDevice.DeviceAddress, PAIRING_PIN))
            {
                // Notification
                Console.WriteLine("PairRequest: OK");

                // Check if device is authentificated
                if (BTDevice.Authenticated)
                {
                    Console.WriteLine("Authenticated: OK - " + BTDevice.DeviceName);

                    // Pair the devices
                    BC.SetPin(PAIRING_PIN);

                    // Validate the connexion, and notify the callback function
                    BC.BeginConnect(BTDevice.DeviceAddress, BluetoothService.SerialPort, new AsyncCallback(Connected), BTDevice);
                }
                else
                    Console.WriteLine("Authenticated: No");
            }
            else
                Console.WriteLine("PairRequest: No");

            // End notification 
            Console.WriteLine("\n- THE END -");
            Console.WriteLine("Please type a key to end the application.");
            Console.ReadLine();
        }


        // Callback function is connexion has been made
        private void Connected(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                // Client is connected now :)
                Console.WriteLine("BC.Connected? " + BC.Connected);

                //WRITE
                if (BC.Connected)
                {
                    Console.WriteLine("BC.RemoteMachineName? " + BC.RemoteMachineName);

                    // Get stream
                    stream = BC.GetStream();

                    if (stream != null)
                        Send("START"); 
                }

                Console.ReadLine();
            }
        }


        /// <summary>
        /// Send a message in the stream
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            // Encode message
            var buffer = System.Text.Encoding.UTF8.GetBytes(message);

            // Write in the stream
            stream.Write(buffer, 0, buffer.Length);

            // Log
            Console.WriteLine("Message sent: [" + message + "].");
        }


        /// <summary>
        /// Read a message from the stream
        /// </summary>
        private void Read()
        {
            if (stream.CanRead)
            {

                byte[] myReadBuffer = new byte[1024];
                StringBuilder myCompleteMessage = new StringBuilder();
                int numberOfBytesRead = 0;

                // Incoming message may be larger than the buffer size. 
                do
                {
                    numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);

                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                }
                while (stream.DataAvailable);

                // Print out the received message to the console.
                Console.WriteLine("You received the following message : " + myCompleteMessage);
            }
            else
            {
                Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
            }
        }


        /// <summary>
        /// Disconnection
        /// </summary>
        private void Disconnect()
        {
            // Clear stream
            stream.Flush();
            stream.Close();

            // End client
            BC.EndConnect(null);

            // Free attributes
            BTDevice = null;
            EP = null;
        }


        /// <summary>
        /// If a device is connected, return its name
        /// </summary>
        /// <returns>Name of the connected device</returns>
        public String GetDeviceName()
        {
            return BC.Connected ? BC.RemoteMachineName : "NULL";
        }

    } // End (class)
} // End (namespace)
