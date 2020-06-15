using UnityEngine;
using System.Collections;

public class controlThetas : MonoBehaviour
{
    public float[] thetas;
    public Transform theta0;
    public Transform theta1;
    public Transform theta2;
    public Transform theta3;
    public Transform theta4;
    public Transform theta5;
    public Transform theta6;

    public float baseturnRate = .01f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (float value in thetas)
        {
            print(value);
        }

        // Since we can't resize builtin arrays
        // we have to recreate the array to resize it
        thetas = new float[7];

        // assign the second element
        thetas[0] = 5.0F;
        thetas[1] = 5.0F;
        thetas[2] = 5.0F;
        thetas[3] = 5.0F;
        thetas[4] = 5.0F;
        thetas[5] = 5.0F;
        thetas[6] = 5.0F;


        //thetas = 
        //for (int i = 0; i < 7; i++) { thetas[i] = 40f; print(i); }
        //Debug.Log(thetas.GetType());
        //Debug.Log(thetas[1]);
    }

    // Update is called once per frame
    void Update()
    {
        theta0.Rotate(0, thetas[0] * baseturnRate, 0);
        theta1.Rotate(0, 0, thetas[1] * baseturnRate);
        theta2.Rotate(0, thetas[2] * baseturnRate, 0);
        theta3.Rotate(0, 0, thetas[3] * baseturnRate);
        theta4.Rotate(0, thetas[4] * baseturnRate, 0);
        theta5.Rotate(0, 0, thetas[5] * baseturnRate);
        theta6.Rotate(0, thetas[6] * baseturnRate, 0);



    }
}
