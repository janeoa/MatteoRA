using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class controlOverTCP : MonoBehaviour
{
	public float sounddefidor = 5.0f;
	public float[] thetas;
	public Transform theta0;
	public Transform theta1;
	public Transform theta2;
	public Transform theta3;
	public Transform theta4;
	public Transform theta5;
	public Transform theta6;
	public float[] dt;
	public float[] oldT;
	public AudioSource[] audioData;
	public AudioClip wheelSpin;
	private int frames = 0;

	#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener;
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient;
	#endregion

	// Use this for initialization
	void Start()
	{
		thetas = new float[7];
		dt = new float[7];
		oldT = new float[7];
		//audioData = new AudioSource[7];

		// Start TcpServer background thread 		
		tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
		tcpListenerThread.IsBackground = true;
		tcpListenerThread.Start();

		//audioData[] = GetComponent<AudioSource>();
		
	}

	// Update is called once per frame
	void Update()
	{
		oldT[0] = theta0.localEulerAngles.y;
		oldT[1] = theta1.localEulerAngles.z;
		oldT[2] = theta2.localEulerAngles.y;
		oldT[3] = theta3.localEulerAngles.z;
		oldT[4] = theta4.localEulerAngles.y;
		oldT[5] = theta5.localEulerAngles.z;
		oldT[6] = theta6.localEulerAngles.y;

		theta0.localEulerAngles = new Vector3(0, thetas[0] * 57.2958f, 0);
		theta1.localEulerAngles = new Vector3(0, 0, thetas[1] * 57.2958f);
		theta2.localEulerAngles = new Vector3(0, thetas[2] * 57.2958f, 0);
		theta3.localEulerAngles = new Vector3(0, 0, thetas[3] * 57.2958f);
		theta4.localEulerAngles = new Vector3(0, thetas[4] * 57.2958f, 0);
		theta5.localEulerAngles = new Vector3(0, 0, thetas[5] * 57.2958f);
		theta6.localEulerAngles = new Vector3(0, thetas[6] * 57.2958f, 0);

		dt[0] = theta0.localEulerAngles.y - oldT[0];
		dt[1] = theta1.localEulerAngles.z - oldT[1];
		dt[2] = theta2.localEulerAngles.y - oldT[2];
		dt[3] = theta3.localEulerAngles.z - oldT[3];
		dt[4] = theta4.localEulerAngles.y - oldT[4];
		dt[5] = theta5.localEulerAngles.z - oldT[5];
		dt[6] = theta6.localEulerAngles.y - oldT[6];


		//if (Math.Abs(dt[0]) > 0.0f) 
		//{
		//if (frames == 10)
		//{
		//	frames = 0;
		//	for (int i = 0; i < 1; i++)
		//	{
		//		audioData[i].pitch =  /*Time.time **/ (Math.Abs(dt[i]) / sounddefidor + 2.0f > 5.0) ? 5.0f : Math.Abs(dt[i]) / sounddefidor + 2.0f;
		//		//Debug.Log(Math.Abs(dt[i]) * 500 / 15.0f + 1.0f);
		//		//audioData[i].volume = /*Time.time **/ (Math.Abs(dt[i]) > 0.001f) ? 0.1f : 0.0f;
		//		//if (Math.Abs(dt[i]) > 0.0f) //audioData[0].Play();
		//		//audioData[i].PlayOneShot(wheelSpin);
		//		//audioData[i].volume = (Math.Abs(dt[i]) > 0) ? 0.1f : 0.0f;
		//	}
  //      }
  //      else
  //      {
		//	frames++;
  //      }
			//audioData.Play(0);
			//}
	}

	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests()
	{
		try
		{
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7979);
			tcpListener.Start();
			Debug.Log("Server is listening");
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				using (connectedTcpClient = tcpListener.AcceptTcpClient())
				{
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream())
					{
						int length;
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
						{
							var templength = length;
							while (templength > 0)
							{
								var incommingData = new byte[56];
								Array.Copy(bytes, 0, incommingData, length - templength, length - templength + 56); //12*8

								var floats = new float[7];

								for (int numsi = 0; numsi < 56; numsi += 8)
								{
									floats[numsi / 8] = Convert.ToSingle(BitConverter.ToDouble(incommingData, numsi));
								}

								templength -= 56;
								//Debug.Log("length is: " + length + "\n" + "Floats = [" + String.Join(", ", new List<float>(floats).ConvertAll(i => i.ToString()).ToArray()) + "]");

								thetas = floats;
							}
						}
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("SocketException " + socketException.ToString());
		}
	}


	public static string ByteArrayToString(byte[] ba)
	{
		StringBuilder hex = new StringBuilder(ba.Length * 2);
		foreach (byte b in ba)
			hex.AppendFormat("{0:x2}", b);
		return hex.ToString();
	}


	public static byte[] StringToByteArray(String hex)
	{
		int NumberChars = hex.Length;
		byte[] bytes = new byte[NumberChars / 2];
		for (int i = 0; i < NumberChars; i += 2)
			bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
		return bytes;
	}
}