using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;

namespace ShellyGarage
{
	[Activity (Label = "Shelly's Garage", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private TextView status;
		private BluetoothSocket socket;
		private Button connectorButton;

		protected override void OnDestroy(){
			if (socket != null) {
				socket.Close ();
				socket.Dispose ();
			}
			base.OnDestroy ();
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			status = FindViewById<TextView> (Resource.Id.statusText);
			TryToConnect ();

			FindViewById<Button> (Resource.Id.light1).Click += delegate { SendAMessage("q","toggling light 1"); };
			FindViewById<Button> (Resource.Id.light2).Click += delegate { SendAMessage("w","toggling light 2"); };
			FindViewById<Button> (Resource.Id.light3).Click += delegate { SendAMessage("e","toggling light 3"); };
			FindViewById<Button> (Resource.Id.door1).Click += delegate { SendAMessage("1","toggling door 1"); };
			FindViewById<Button> (Resource.Id.door2).Click += delegate { SendAMessage("2","toggling door 2"); };
			FindViewById<Button> (Resource.Id.door3).Click += delegate { SendAMessage("3","toggling door 3"); };
			connectorButton = FindViewById<Button> (Resource.Id.cnx);
			connectorButton.Click += delegate {
				if (socket != null && socket.IsConnected) {
					socket.Close ();
					socket.Dispose ();
					socket = null;
					connectorButton.Text = "Connect";
					status.Text = "ain't connected";
				}else{
					TryToConnect();
				}
			};
			connectorButton.Enabled = true;
		}

		private async void SendAMessage(string messageToSend, string statusMessage){
			if(socket != null && socket.IsConnected){
				var buffer = System.Text.Encoding.Unicode.GetBytes (messageToSend);
				status.Text = statusMessage;
				await socket.OutputStream.WriteAsync(buffer,0,buffer.Length);
				status.Text = "cnxioned...";
			}
		}

		private void SetStatusText(string message){
			RunOnUiThread (() => {
				status.Text = message;
			});
		}

		private void TryToConnect(){
			System.Threading.ThreadPool.QueueUserWorkItem (o => {
				BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
				SetStatusText("acquiring adapter...");
				if (bluetoothAdapter == null) {
					SetStatusText("No Bluetooth Adapter found.");
					return;
				} else if (!bluetoothAdapter.IsEnabled) {
					SetStatusText("BlueTooth Adapter disabled.");
					return;
				}
				SetStatusText("finding Shelly's Garage");
				var device = bluetoothAdapter.BondedDevices.FirstOrDefault (x => x.Name == "Shelly'sGarage");
				if (device == null) {
					SetStatusText("You ain't paired with Shelly's Garage.");
					return;
				}
				SetStatusText("connecting...");
				socket = device.CreateRfcommSocketToServiceRecord (Java.Util.UUID.FromString ("00001101-0000-1000-8000-00805f9b34fb"));
				try {
					socket.Connect ();
					bluetoothAdapter.Dispose();
					SetStatusText("Connected to Shelly's Garage");
					RunOnUiThread(()=>{
						connectorButton.Text = "Disconnect";
					});
				} catch {//Exception ex) {
					SetStatusText("Connection failt.");
				}
			});
		}
	}
}


//public class Receiver : BroadcastReceiver
//{ 
//	Activity _chat;
//
//	public Receiver (Activity chat)
//	{
//		_chat = chat;
//	}
//
//	public override void OnReceive (Context context, Intent intent)
//	{ 
//		string action = intent.Action;
//
//		// When discovery finds a device
//		if (action == BluetoothDevice.ActionFound) {
//			// Get the BluetoothDevice object from the Intent
//			BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra (BluetoothDevice.ExtraDevice);
//			// If it's already paired, skip it, because it's been listed already
//			if (device.BondState != Bond.Bonded) {
//				newDevicesArrayAdapter.Add (device.Name + "\n" + device.Address);
//			}
//			// When discovery is finished, change the Activity title
//		} else if (action == BluetoothAdapter.ActionDiscoveryFinished) {
//			_chat.SetProgressBarIndeterminateVisibility (false);
//			_chat.SetTitle (Resource.String.select_device);
//			if (newDevicesArrayAdapter.Count == 0) {
//				var noDevices = _chat.Resources.GetText (Resource.String.none_found).ToString ();
//				newDevicesArrayAdapter.Add (noDevices);
//			}
//		}
//	} 
//}