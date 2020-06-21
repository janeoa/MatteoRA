using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class negativeBiasShadow : MonoBehaviour
{
    public Light lght;
    public float setBias = -1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lght.shadowBias = setBias;
    }
}
