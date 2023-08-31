using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;
    public float surfaceGravity;
    public float radius;
    public float mass;
    public Color color;

    void Start() 
    {
        color = Color.gray;
    }

    void Update()
    {
        position = transform.position;
        surfaceGravity = Constants.gravityConstant * (mass / (radius * radius));
        transform.localScale = new Vector3(radius, radius, radius);
        transform.rotation = new Quaternion(0, 0, 0, 0);

        if (this.gameObject.GetComponent<Renderer>().enabled == true) 
        {
            this.gameObject.GetComponent<Renderer>().material.color = color;
            this.gameObject.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
        }
    }
}
