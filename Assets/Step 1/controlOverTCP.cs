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
	public Transform[] theta;
	// public Transform theta[0];
	// public Transform theta[1];
	// public Transform theta[2];
	// public Transform theta[3];
	// public Transform theta[4];
	// public Transform theta[5];
	// public Transform theta[6];
	public float[] dt;
	public float[] oldT;
	public AudioSource[] audioData;
	public AudioClip wheelSpin;
	private int frames = 0;


	public float pitchMultiplier = 1f;                                          // Used for altering the pitch of audio clips
	public float lowPitchMin = 1f;                                              // The lowest possible pitch for the low sounds
	public float lowPitchMax = 6f;                                              // The highest possible pitch for the low sounds
	public float highPitchMultiplier = 0.25f;                                   // Used for altering the pitch of high sounds


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
		theta = new Transform[7];
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
		oldT[0] = theta[0].localEulerAngles.y;
		oldT[1] = theta[1].localEulerAngles.z;
		oldT[2] = theta[2].localEulerAngles.y;
		oldT[3] = theta[3].localEulerAngles.z;
		oldT[4] = theta[4].localEulerAngles.y;
		oldT[5] = theta[5].localEulerAngles.z;
		oldT[6] = theta[6].localEulerAngles.y;

		theta[0].localEulerAngles = new Vector3(0, thetas[0] * 57.2958f, 0);
		theta[1].localEulerAngles = new Vector3(0, 0, thetas[1] * 57.2958f);
		theta[2].localEulerAngles = new Vector3(0, thetas[2] * 57.2958f, 0);
		theta[3].localEulerAngles = new Vector3(0, 0, thetas[3] * 57.2958f);
		theta[4].localEulerAngles = new Vector3(0, thetas[4] * 57.2958f, 0);
		theta[5].localEulerAngles = new Vector3(0, 0, thetas[5] * 57.2958f);
		theta[6].localEulerAngles = new Vector3(0, thetas[6] * 57.2958f, 0);

		dt[0] = theta[0].localEulerAngles.y - oldT[0];
		dt[1] = theta[1].localEulerAngles.z - oldT[1];
		dt[2] = theta[2].localEulerAngles.y - oldT[2];
		dt[3] = theta[3].localEulerAngles.z - oldT[3];
		dt[4] = theta[4].localEulerAngles.y - oldT[4];
		dt[5] = theta[5].localEulerAngles.z - oldT[5];
		dt[6] = theta[6].localEulerAngles.y - oldT[6];

		float pitch0 = Mathf.Min(lowPitchMax, ULerp(lowPitchMin, lowPitchMax, dt[0]));
		m_HighAccel.pitch = pitch0 * pitchMultiplier * highPitchMultiplier;
		m_HighAccel.dopplerLevel = 0;
		m_HighAccel.volume = 1;

	}
	private AudioSource m_LowAccel; // Source for the low acceleration sounds
	private AudioSource m_LowDecel; // Source for the low deceleration sounds
	private AudioSource m_HighAccel; // Source for the high acceleration sounds
	private AudioSource m_HighDecel; // Source for the high deceleration sounds
	private bool m_StartedSound; // flag for knowing if we have started sounds


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

	private static float ULerp(float from, float to, float value)
	{
		return (1.0f - value) * from + value * to;
	}
}