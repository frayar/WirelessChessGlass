package com.artchess.bluetoothplugin;

public class BluetoothPluginActivity {
	
	public static final String message = "A2B3";
	
	// Methods
	public static String GetMessage() {
		
		return (message.length() == 0) ?  "A1B2" : message;
	}

}
