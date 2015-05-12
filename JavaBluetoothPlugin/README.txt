------------------------------
BLUETOOTH - ANDROID
------------------------------

- Open Bluetooth/BluetoothPlugin as an Android project in Eclipse (java 1.7)
- Refresh the project to allow Eclipse to generate the binaries
- Go in BluetoothPlugin\bin\classes
- Generate a archive containing com/... files
- Rename it BluetoothPlugin.jar
- Copy/Paste it in <Unity_Project>/Assets/Plugins/Android/

- [INFO] BluetoothManager.java

	// Send the move back to Unity
	UnityPlayer.UnitySendMessage("Scene", "ProcessMessage", readMessage);
