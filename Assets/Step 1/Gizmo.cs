using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo : MonoBehaviour
{
    public float gizmosize = 0.05f;
    public Color gizmoColor = Color.yellow;

    void OnDrawGizmos() {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmosize);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
