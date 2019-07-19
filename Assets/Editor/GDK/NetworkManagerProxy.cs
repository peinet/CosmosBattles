/********************************************************************
	created:	2018/01/31 16:40:01
	author:		chens
	purpose:	
*********************************************************************/
using Assets.Editor.GDK.common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.GDK
{
	[InitializeOnLoad]
	class NetworkManagerProxy
	{

		private static NetworkManagerProxy _instance;
		//private NetworkManager _network;
		// The port number for the remote device.  
		private const int port = 11000;

		// ManualResetEvent instances signal completion.  
		private static ManualResetEvent connectDone =
			new ManualResetEvent(false);
		private static ManualResetEvent sendDone =
			new ManualResetEvent(false);
		private static ManualResetEvent receiveDone =
			new ManualResetEvent(false);
		// The response from the remote device.  
		private static String response = String.Empty;

		private List<MessagePack> messagePackList = new List<MessagePack>();
		private Vector2 logVec2;
		private bool isRecording = true;
		private string log = "";
        private StringBuilder logbuilder = new StringBuilder(2000);
        private CommonWindow windos;
		private string filterStr = GDKApplication.readNetworkFilter();
        internal static string opcodeProto = "";
        internal static string opcode2Proto = "";
        List<ProtoEntity> protoEntityList = new List<ProtoEntity>();
        static NetworkManagerProxy()
		{
			getInstance().init();
			//NetworkManager.Instance
			//NetworkManager.onMessageHandler = getInstance().onMessageHandler;
			//NetworkManager.onSendHandler = getInstance().onSendHandler;
		}

		private void onSendHandler(uint arg1, object arg2)
		{
			if(isRecording)
			{
				messagePackList.Add(new MessagePack(arg1, arg2));
				if (messagePackList.Count > 300)
					messagePackList.RemoveAt(0);
				updataLogStr();
			}
		}

		//private void onMessageHandler(NetworkPacket obj)
		//{
		//	if (isRecording)
		//	{
		//		messagePackList.Add(new MessagePack(obj));
		//		if (messagePackList.Count > 300)
		//			messagePackList.RemoveAt(0);
		//		updataLogStr();
		//	}
		//}
        private List<string> filterList;
        private void updataLogStr()
        {
            filterList = filterStr.Split(',').ToList();
            logbuilder = new StringBuilder(2000);
            if (messagePackList.Count == 0) return;
			for (int i = messagePackList.Count - 1; i > -1; i--)
			{
				if(filterList.Contains(messagePackList[i].opcode.ToString()) == false)
				{
					//log += (messagePackList[i].type == 0 ? "上行" : "下行") + "opcode:" + messagePackList[i].opcode + "   length:" + messagePackList[i].data.Length + "\n";
                    //log = string.Format(log + "{0} opcode:{1} length:{2}\n", (messagePackList[i].type == 0 ? "上行" : "下行") , messagePackList[i].opcode , messagePackList[i].data.Length);
                    logbuilder.AppendFormat("{0} opcode:{1} length:{2}   {3} {4}\n", 
                        (messagePackList[i].type == 0 ? "上行" : "下行"),
                        messagePackList[i].opcode,
                        messagePackList[i].data.Length ,
                        messagePackList[i].comment,
                        messagePackList[i].key 
                        );
                }
			}
			if(windos)
			{
				windos.Repaint();
			}
		}

		internal static NetworkManagerProxy getInstance()
		{
			if (_instance == null) _instance = new NetworkManagerProxy();
			return _instance;
		}
		internal void init()
		{
			var go = GameObject.Find("UGameManager");
			if (go)
			{
				//_network = go.GetComponent<NetworkManager>();
			}
    }
		public void serverInfo()
		{
			//if (_network)
			//{
			//	CommonWindow.show(_network.GameServerIP + "   " + _network.GameServerPort + "   连接状态：" + _network.IsConnect);
			//}
			//else
			//{
			//	init();
			//	if (_network == null)
			//	{
			//		CommonWindow.show("艹，就是找不到NetworkManager");
			//	}else
			//	{
			//		serverInfo();
			//	}
			//}
			//InfoWindow.show();
		}
		public void showNetWorkMessage()
        {
            opcodeProto = File.ReadAllText(Application.dataPath + "/../../../config/protos/client/opcode.proto");
            opcode2Proto = File.ReadAllText(Application.dataPath + "/../../../config/protos/client/opcode2.proto");
            windos = CommonWindow.show(() =>
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("读取所有proto"))
                {
                    var files = Directory.GetFiles(Application.dataPath + "/../../../config/protos/","*.proto", SearchOption.AllDirectories);

                    for (int i = 0; i < files.Length; i++)
                    {
                       
                        protoEntityList.Concat(getProtoEntityListFromStr(File.ReadAllText(files[i])));
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
				GUILayout.Label("只显示最近300条通信（包含隐藏掉的通信）");
				if(GUILayout.Button("清除"))
				{
					messagePackList = new List<MessagePack>();
					updataLogStr();
				}
				var btnName = isRecording?"暂停":"开始";
				if (GUILayout.Button(btnName))
				{
					isRecording = !isRecording;
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("过滤协议号");
				filterStr = GUILayout.TextArea(filterStr, GUILayout.MinWidth(300));
				if (GUILayout.Button("保存"))
				{
					GDKApplication.writeNetworkFilter(filterStr);
					filterStr = GDKApplication.readNetworkFilter();
					updataLogStr();
                }
                GUILayout.EndHorizontal();
				//GUILayout.EndScrollView();//麻痹的嵌套scrollview报错，为什么。
				//logVec2 = EditorGUILayout.BeginScrollView(logVec2);
				GUILayout.Label(logbuilder.ToString());
				//GUILayout.EndScrollView();
			});
		}

        private IEnumerable<ProtoEntity> getProtoEntityListFromStr(string v)
        {
            var l = new List<ProtoEntity>();
            var group = Regex.Matches(v, "(//[\\s|\\S]*?)message\\s*(\\S*)\\s*?{([\\s|\\S]*?)}");
            return new List<ProtoEntity>();
        }


        /**
			链接测试服务器
		*/
        public static void StartClient()
		{
			var b = true;
			if (b == true)
			{
				return;
			}
			try
			{
				//IPHostEntry ipHostInfo = Dns.GetHostEntry("uvm030");
				//IPAddress ipAddress = ipHostInfo.AddressList[0];
				//IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

				Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				client.BeginConnect("192.168.240.37", port, new AsyncCallback(ConnectCallback), client);
				connectDone.WaitOne();

				Send(client, "This is a test<EOF>");
				sendDone.WaitOne();

				Receive(client);
				receiveDone.WaitOne();


				client.Shutdown(SocketShutdown.Both);
				client.Close();

			}
			catch (Exception e)
			{
				Debug.Log(e.ToString());
			}
		}

		private static void ConnectCallback(IAsyncResult ar)
		{
			try
			{
				Socket client = (Socket)ar.AsyncState;

				client.EndConnect(ar);

				//Debug.Log("Socket connected to {0}",
				//	client.RemoteEndPoint.ToString());

				connectDone.Set();
			}
			catch (Exception e)
			{
				Debug.Log(e.ToString());
			}
		}

		private static void Receive(Socket client)
		{
			try
			{
				StateObject state = new StateObject();
				state.workSocket = client;

				client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
					new AsyncCallback(ReceiveCallback), state);
			}
			catch (Exception e)
			{
				Debug.Log(e.ToString());
			}
		}

		private static void ReceiveCallback(IAsyncResult ar)
		{
			try
			{
				StateObject state = (StateObject)ar.AsyncState;
				Socket client = state.workSocket;

				int bytesRead = client.EndReceive(ar);

				if (bytesRead > 0)
				{
					state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

					client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
						new AsyncCallback(ReceiveCallback), state);
				}
				else
				{
					if (state.sb.Length > 1)
					{
						response = state.sb.ToString();
					}
					receiveDone.Set();
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.ToString());
			}
		}

		private static void Send(Socket client, String data)
		{
			byte[] byteData = Encoding.ASCII.GetBytes(data);

			client.BeginSend(byteData, 0, byteData.Length, 0,
				new AsyncCallback(SendCallback), client);
		}

		private static void SendCallback(IAsyncResult ar)
		{
			try
			{
				Socket client = (Socket)ar.AsyncState;

				int bytesSent = client.EndSend(ar);
				/*Debug.Log("Sent {0} bytes to server.", bytesSent);*/

				sendDone.Set();
			}
			catch (Exception e)
			{
				Debug.Log(e.ToString());
			}
		}
	}

	class InfoWindow : EditorWindow
	{
		private static InfoWindow s_Window;
		
		public static void show()
		{
			s_Window = GetWindow<InfoWindow>();
			GUIContent content = new GUIContent();
			content.text = "详细信息";
			s_Window.titleContent = content;
			GUILayout.TextArea("测试文本");
		}
	}

	public class StateObject
	{
		public Socket workSocket = null;
		public const int BufferSize = 256;
		public byte[] buffer = new byte[BufferSize];
		public StringBuilder sb = new StringBuilder();
	}
    class ProtoEntity
    {
        public ProtoEntity()
        {

        }
    }
    class MessagePack
    {
        //public string pbFile = "";
        public string comment = "";
        public string key = "";
        public uint opcode = 0;
		public int type = -1;
		public byte[] data = null;
		public MessagePack(uint code, object buffer)
		{
			type = 0;
			opcode = code;
            //data = buffer == null ? new byte[0]:buffer.buffer;
            //LuaStringBuffer
            data = new byte[0];
            process();
        }
		//public MessagePack(NetworkPacket packet)
		//{
		//	type = 1;
		//	opcode = packet.Opcode;
		//	data = packet.GetRawData();
  //          //LuaByteBuffer temp = new LuaByteBuffer(data, packet.DataLength);
  //          process();
  //      }
        public void process()
        {
            if(NetworkManagerProxy.opcodeProto == "" || NetworkManagerProxy.opcode2Proto == "")
            {
                return;
            }
            var group = Regex.Match(NetworkManagerProxy.opcodeProto, "(//.*?)\n(.*?)=.*?"+opcode);
            if(group == null || group.Groups == null || group.Groups.Count < 3)
            {
                group = Regex.Match(NetworkManagerProxy.opcode2Proto, "(//.*?)\n(.*?)=.*?" + opcode);
            }

            if (group != null && group.Groups != null && group.Groups.Count == 3)
            {
                comment = group.Groups[1].ToString();
                key = group.Groups[2].ToString();
            }
        }
	}
}
	