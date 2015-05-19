package com.artchess.bluetoothplugin;

import java.util.UUID;

import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import com.unity3d.player.UnityPlayer;

public class BluetoothManager {
	
	public static BluetoothAdapter bluetoothAdapter;
	
    // Message types sent from the BluetoothChatService Handler
    public static final int MESSAGE_STATE_CHANGE = 1;
    public static final int MESSAGE_READ = 2;
    public static final int MESSAGE_WRITE = 3;
    public static final int MESSAGE_DEVICE_NAME = 4;
    public static final int MESSAGE_TOAST = 5;
    
    public static final String NAME = "BluetoothServer";
    public static UUID MY_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");;
    
    // FRED : "C0:CB:38:DC:B1:98"
    // Pixelsense : "90:A4:DE:A1:82:98"
    public static final String REMOTE_DEVICE = "90:A4:DE:A1:82:98";
    
    
	public static void init(final Activity currentActivity) {

		Log.v("BT_MANAGER", "Init...");
		
		// Get default Bluettoth adapter
        bluetoothAdapter = BluetoothAdapter.getDefaultAdapter();		
        
        // Check if bluetooth is available
		if (bluetoothAdapter != null) {
			// BLUETOOTH
			//Toast.makeText(MainActivity.this, "Bluetooth available",  Toast.LENGTH_SHORT).show();
			
			
			currentActivity.runOnUiThread(new Runnable() {
				public void run() {
					
					// Specify a callback function for further use when connection will be established 
					Log.v("BT_MANAGER", "Creating handler ...");
					Handler mHandler = new Handler() {
						public void handleMessage(Message msg) {
						  switch (msg.what) {
						     case MESSAGE_READ: {
						    	 Log.v("BT_MANAGER", "Message obtained ...");
						    	 byte[] readBuf = (byte[]) msg.obj;
					             String readMessage = new String(readBuf, 0, msg.arg1);
					             Log.v("BT_MANAGER", "Message obtained : " + readMessage);
					             // Send the move back to Unity
					             UnityPlayer.UnitySendMessage("Scene", "ProcessMessage", readMessage);
						     }
						  }
						}
					};
					
					// Start server-side bluetooth 
					Log.v("BT_MANAGER", "Starting ServerConnect Thread");
					new ServerConnectThread(mHandler).start();	
				}
			});
				
			
		} else {
			// NO BLUETOOTH
			//Toast.makeText(MainActivity.this, "No bluetooth",  Toast.LENGTH_SHORT).show();
		}
		
		Log.v("BT_MANAGER", "Initialized !");

		
	}
	
    public static void finish()
    {
    	// TODO: Disconnect bluetooth
		Log.v("BT_MANAGER", "Finished !");
    }

}