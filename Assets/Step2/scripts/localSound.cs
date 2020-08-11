using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
// using Sort;
// using String;

public class localSound : MonoBehaviour
{

    public Transform tra;
    public AudioSource ass;
    public bool onZ = false;

    public float[] last10pos;
    public float[] last10vel;
    public float[] last10dts;

    public float averageSpeed;
    public float meadianSpeed;
    // Start is called before the first frame update
    void Start()
    {
        last10pos = new float[10];
        last10vel = new float[10];
        last10dts = new float[10];
        
        tra = transform;
        ass = GetComponent<AudioSource>();;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //last10pos = last10pos.Skip(1).ToArray();
        if(onZ){
            last10pos = Pushpopfloatarr(last10pos, tra.localEulerAngles.z);
        }else{
            last10pos = Pushpopfloatarr(last10pos, tra.localEulerAngles.y);
        }
        last10dts = Pushpopfloatarr(last10dts, Time.deltaTime);
        last10vel = Pushpopfloatarr(last10vel, (last10pos[last10pos.Length-1]-last10pos[last10pos.Length-2])/last10dts[last10dts.Length-1]);        

        float localSum = 0;
        for(int i=0; i<last10vel.Length; i++){
            localSum+=last10vel[i];
        }
        averageSpeed = localSum/last10vel.Length;
        float averageRad = averageSpeed * Mathf.PI / 180.0f;
        
        ass.volume = Mathf.Sign(Mathf.Abs(averageSpeed)-0.01f);
        ass.pitch = Mathf.Abs(averageRad);

        // var sorter = new QuickSort<float>();
        // float[] toSort = last10vel;
        // sorter.Sort(toSort);
        // meadianSpeed = toSort[5];
        
        //File.AppendAllText("Saved_data.csv", string.Concat(Time.time.ToString(),", ", last10pos[last10pos.Length-1],", ", averageSpeed,", ", meadianSpeed, "\n"));
    }


    private float[] Pushpopfloatarr(float[] input, float nin){
        float[] epta = new float[input.Length];
        //float[] epta = new float[10];

        for(int i=0; i<input.Length-1; i++){
            epta[i] = input[i+1];
        }
        epta[input.Length-1] = nin;

        return epta;
    }

    
}
