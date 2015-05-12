package com.example.bluetoothtest;

import java.util.UUID;

import android.bluetooth.BluetoothAdapter;
import android.os.Handler;
import android.os.Message;
import android.util.Log;

public class BluetoothManager {
	
	public static BluetoothAdapter bluetoothAdapter;
	
    // Message types sent from the BluetoothChatService Handler
    public static final int MESSAGE_STATE_CHANGE = 1;
    public static final int MESSAGE_READ = 2;
    public static final int MESSAGE_WRITE = 3;
    public static final int MESSAGE_DEVICE_NAME = 4;
    public static final int MESSAGE_TOAST = 5;
    
    public static final String NAME = "BluetoothServer";
    public static final UUID MY_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
    
    // Pixelsense : "90:A4:DE:A1:82:98"
    public static final String REMOTE_DEVICE = "90:A4:DE:A1:82:98";
    
	public BluetoothManager() {
	}
	
	public static void init() {
		Log.v("BT_MANAGER", "Init...");
		// Check if bluetooth is available
        bluetoothAdapter = BluetoothAdapter.getDefaultAdapter();				  
		if (bluetoothAdapter == null)
		{
			/*Toast.makeText(MainActivity.this, "No bluetooth", 
					  Toast.LENGTH_SHORT).show();*/
		} else {
			/*Toast.makeText(MainActivity.this, "Bluetooth available", 
					  Toast.LENGTH_SHORT).show();*/
			//BluetoothDevice remoteDevice = bluetoothAdapter.getRemoteDevice(REMOTE_DEVICE);
			new ServerConnectThread(mHandler).start();		
		}
		Log.v("BT_MANAGER", "Initialized !");
	}
	
    public static void finish()
    {
    	// TODO: Disconnect bluetooth
    }
	
	public static Handler mHandler = new Handler() {
		public void handleMessage(Message msg) {
		  switch (msg.what) {
		     case MESSAGE_READ: {
		    	 Log.v("BT_MANAGER", "Message obtained ...");
		    	 byte[] readBuf = (byte[]) msg.obj;
	             String readMessage = new String(readBuf, 0, msg.arg1);
	             Log.v("BT_MANAGER", "Message obtained : " + readMessage);
		     }
		  }
		}
	};
    
}