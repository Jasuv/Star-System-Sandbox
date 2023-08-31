using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalGravitation : MonoBehaviour
{
    public List<CelestialBody> bodies; //The list of all bodies in simulation
                                       //(bodies are added by the PathPrediction script)

    public bool play = false; //Keeps track if the simulation is paused or not

    //Called by the "play" button
    public void PausePlay()
    {
        play = !play;
    }

    //Starts the simulation if "play"
    public void FixedUpdate()
    {
        if (play) SimulateGravity();
    }

    //Nested for loops to calcualte next position for each body
    public void SimulateGravity()
    {
        foreach (CelestialBody body in bodies)
        {
            foreach (CelestialBody other in bodies)
            {
                if (other != body)
                {
                    Vector3 oldPos = body.transform.position;
                    Vector3 newPos = oldPos + Constants.physicsDeltaTime * body.velocity;
                    body.velocity += Constants.physicsDeltaTime * CalculateAcceleration(body, other);
                    body.transform.position = newPos;
                }
            }
        }
    }

    //Newton's Universal Law of Gravity equation in action
    private Vector3 CalculateAcceleration(CelestialBody body, CelestialBody other)
    {
        float totalMass = (body.mass * other.mass);
        float distance = ((body.transform.position) - other.transform.position).magnitude;
        if (distance == 0) distance = 0.1f; //Temporary fix to prevent divide by 0
        Vector3 direction = ((body.transform.position) - other.transform.position) / distance;
        Vector3 gravityForce = -Constants.gravityConstant * (totalMass / distance) * direction;
        return (gravityForce / body.mass) * Constants.physicsDeltaTime;
    }
}