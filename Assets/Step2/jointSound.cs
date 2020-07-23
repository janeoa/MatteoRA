using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jointSound : MonoBehaviour
{
    public Transform[] theta;
    private static int[] timesteps;
    private static int[] speedvalue;
    private static int minSpeed;
    private static int maxSpeed;
    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        //theta = new Transform[1];
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.time);
        audioSource.pitch = (Mathf.Sin(Time.time)+1f)/2f+1;
        theta[0].Rotate(0, ((Mathf.Sin(Time.time)+1f)/2f+1)*2, 0);

        //theta[0].localEulerAngles = new Vector3(0, thetas[0] * 57.2958f, 0);
    }
}
