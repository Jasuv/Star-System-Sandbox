using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/*
 * Currently this is super unoptimized...
 * I got everything running on the CPU and the method
 * for creating the visuals for the lines is terrible.
 * I have to change to using only LineRenderers
 */

public class PathPrediction : MonoBehaviour
{
    //To keep track of the planet console changes and camera movement
    [SerializeField] PlanetConsole planetConsle;
    [SerializeField] CameraController cameraController;

    [SerializeField] Transform bodies; //Contains the current bodies in the simulation
    [SerializeField] GameObject lineObject; //The visual for the trajectory lines

    private Scene trajectoryScene; //Seperate scene to simulate trajectories
    private List<CelestialBody> fauxBodies; //Cloned list of simulation bodies
    private List<Path> paths;

    //The length of the trajectory lines
    public int maxIterations;
    public bool showlines = true;

    //Refreshes the trajectory sim if true
    private bool refresh = true;
    
    public void Awake()
    {
        fauxBodies = new List<CelestialBody>();
        paths = new List<Path>();
        trajectoryScene = SceneManager.CreateScene("body trajectories");
        FindBodies();
    }

    //Adds every new body in simulation to "UniversalGravitation.bodies"
    public void FindBodies() 
    {
        GetComponent<UniversalGravitation>().bodies.Clear();
        foreach (Transform body in bodies)
        {
            GetComponent<UniversalGravitation>().bodies.Add(body.GetComponent<CelestialBody>());
        }
    }
    
    //Checks if all conditions are met to update all the trajectories
    private void FixedUpdate()
    {
        if (refresh && (cameraController.isMoving == true || planetConsle.reset == true))
            ResetTrajectoryScene();
    }

    //Resets the trajectory scene to update lines
    public void ResetTrajectoryScene() 
    {
        FindBodies();

        //Deletes all gameobjects because "Clear()" doesn't delete them
        foreach (CelestialBody simBody in fauxBodies) 
        {
            Destroy(simBody.gameObject);
        }
        fauxBodies.Clear();

        //Deletes all "linesObjects" and faux UniversalGravitation script
        GameObject[] destroying = GameObject.FindGameObjectsWithTag("deletion");
        foreach (GameObject obj in destroying) 
        {
            Destroy(obj);
        }
        paths.Clear();


        CreateFauxScene();
    }

    //Recreates the entire sandbox but invisable and in the background
    public void CreateFauxScene()
    {
        foreach (Transform body in bodies)
        {
            //Creates invisible clones of all the bodies
            GameObject bodyClone = Instantiate(body.gameObject, body.transform.position, body.rotation);
            bodyClone.GetComponent<Renderer>().enabled = false;
            Destroy(bodyClone.GetComponent<SphereCollider>());
            bodyClone.name = "sim " + body.name;
            SceneManager.MoveGameObjectToScene(bodyClone, trajectoryScene);
            fauxBodies.Add(bodyClone.GetComponent<CelestialBody>());

            //Creates an accompanying path
            Path newPath = new Path();
            paths.Add(newPath);
        }
        //Creates the faux UniversalGravitation script for the scene
        GameObject newSolarSusScript = new GameObject("sim solar script");
        newSolarSusScript.AddComponent<UniversalGravitation>();
        newSolarSusScript.GetComponent<UniversalGravitation>().bodies = fauxBodies;
        SceneManager.MoveGameObjectToScene(newSolarSusScript, trajectoryScene);
        newSolarSusScript.tag = "deletion";
        SimulateTrajectory(newSolarSusScript.GetComponent<UniversalGravitation>());
    }

    //Actually simulates the trajectory scene
    public void SimulateTrajectory(UniversalGravitation sussyBaka)
    {
        //The faux UniversalGravitation script at work (creates the paths)
        int pointsCount = 0;
        List<Color> lineColor = new List<Color>();
        for (int i = 0; i < maxIterations; i++)
        {
            sussyBaka.SimulateGravity();
            foreach (CelestialBody simBody in sussyBaka.GetComponent<UniversalGravitation>().bodies)
            {
                paths[pointsCount].points.Add(simBody.GetComponent<CelestialBody>().transform.position);
                lineColor.Add(simBody.GetComponent<CelestialBody>().color); //IDK what this does
                pointsCount++;
            }
            pointsCount = 0;
        }

        //Creates the visuals for the paths
        int itr = new int();
        foreach (Path path in paths)
        {
            GameObject newLine = Instantiate(lineObject, transform.position, transform.rotation);
            newLine.tag = "deletion";
            SceneManager.MoveGameObjectToScene(newLine, trajectoryScene);

            //Sets up the LineRenderer
            newLine.GetComponent<LineRenderer>().positionCount = maxIterations;
            SetColor(newLine.GetComponent<LineRenderer>(), lineColor[itr]);
            for (int i = 0; i < path.points.Count; i++)
            {
                newLine.GetComponent<LineRenderer>().SetPosition(i, path.points[i]);
            }
            if (showlines)
                newLine.GetComponent<LineRenderer>().widthMultiplier = 10;
            else
                newLine.GetComponent<LineRenderer>().widthMultiplier = 0;
            itr++;
        }


        refresh = true;
        planetConsle.reset = false;
    }

    //Sets path colors
    private void SetColor(LineRenderer line, Color color) 
    {
        line.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        Gradient tempGrad = new Gradient();
        GradientColorKey[] tempColorKeys = new GradientColorKey[2];
        tempColorKeys[0] = new GradientColorKey(color, 0);
        tempColorKeys[1] = new GradientColorKey(color, 1);
        tempGrad.colorKeys = tempColorKeys;
        line.colorGradient = tempGrad;
    }
}

//Custom path script that basically mimics the Unity's Line Renderer for some reason
public class Path
{
    public List<Vector3> points = new List<Vector3>();
    public Color color;
}




















/*{
    public Transform bodies;

    private SolarSystemScript mainSolarSusScript;
    private Scene simScene;
    private List<CelestialBodyScript> simBodies;
   
    public List<Path> paths;
    public GameObject lineObject;
    public int maxIterations;

    public void Awake()
    {
        simBodies = new List<CelestialBodyScript>();
        paths = new List<Path>();
        mainSolarSusScript = GetComponent<SolarSystemScript>();
        foreach (Transform body in bodies)
        {
            mainSolarSusScript.bodies.Add(body.GetComponent<CelestialBodyScript>());
        }
        CreatePhysicsScene();
    }

    private void CreatePhysicsScene() 
    {
        simScene = SceneManager.CreateScene("trajectory");
        foreach (Transform body in bodies) 
        {
            GameObject bodyClone = Instantiate(body.gameObject, body.position, body.rotation);
            //bodyClone.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(bodyClone, simScene);
            bodyClone.name = "sussy";
            simBodies.Add(bodyClone.GetComponent<CelestialBodyScript>());
            Path newPath = new Path();
            paths.Add(newPath);
        }
        GameObject newSolarSusScript = new GameObject("sussy baka");
        newSolarSusScript.AddComponent<SolarSystemScript>();
        newSolarSusScript.GetComponent<SolarSystemScript>().bodies = simBodies;
        SceneManager.MoveGameObjectToScene(newSolarSusScript, simScene);
        SimulateTrajectory(newSolarSusScript.GetComponent<SolarSystemScript>());
    }

    public void SimulateTrajectory(SolarSystemScript sussyBaka) 
    {
        int pointsCount = 0;
        for (int i = 0; i < maxIterations; i++) 
        {
            sussyBaka.SimulateGravity();
            foreach (CelestialBodyScript simBody in sussyBaka.GetComponent<SolarSystemScript>().bodies) 
            {
                print(simBody.GetComponent<CelestialBodyScript>().rig.position + " body is " + simBody.name);
                paths[pointsCount].points.Add(simBody.GetComponent<CelestialBodyScript>().rig.position);
                pointsCount++;
            }
            pointsCount = 0;
        }
        foreach (Path path in paths)
        {
            GameObject newLine = Instantiate(lineObject, transform.position, transform.rotation);
            SceneManager.MoveGameObjectToScene(newLine, simScene);
            newLine.GetComponent<LineRenderer>().positionCount = maxIterations;
            for (int i = 0; i < path.points.Count; i++)
            {
                newLine.GetComponent<LineRenderer>().SetPosition(i, path.points[i]);
            }
        }
    }
}

public class Path
{
    public List<Vector3> points = new List<Vector3>();
    public Color color;
}*/

























/*{
    public SolarSystemScript solarScript;
    public Transform bodiesList;
    public List<SimCelestialBody> simBodies;
    public List<Path> paths;
    public GameObject lineObject;
    public int maxIterations;

    private void Start()
    {
        solarScript = GetComponent<SolarSystemScript>();
        paths = new List<Path>();
        simBodies = new List<SimCelestialBody>();
        CreateSimulation();
    }

    public void FixedUpdate()
    {
        if (!solarScript.start)
            CreateSimulation();
    }

    public void CreateSimulation()
    {
        ClearSimulation();
        for (int i = 0; i < bodiesList.childCount; i++)
        {
            SimCelestialBody bodyClone = new SimCelestialBody(bodiesList.GetChild(i).GetComponent<CelestialBodyScript>().position,
                                                              bodiesList.GetChild(i).GetComponent<CelestialBodyScript>().velocity,
                                                              bodiesList.GetChild(i).GetComponent<CelestialBodyScript>().surfaceGravity,
                                                              bodiesList.GetChild(i).GetComponent<CelestialBodyScript>().radius,
                                                              bodiesList.GetChild(i).GetComponent<CelestialBodyScript>().mass);
            simBodies.Add(bodyClone);
            Path newPath = new Path();
            paths.Add(newPath);
        }
        SimulateTrajectory();
    }

    public void ClearSimulation() 
    {
        simBodies.Clear();
        paths.Clear();
        if (this.gameObject.transform.childCount > 0)
        {
            for (int i = 0; i < this.gameObject.transform.childCount; i++)
            {
                Destroy(this.gameObject.transform.GetChild(i).gameObject);
            }
        }
    }

    private void SimulateTrajectory()
    {
        for (int k = 0; k < maxIterations; k++)
        {
            for(int i = 0; i < simBodies.Count; i++)
            {
                for (int j = 0; j < simBodies.Count; j++)
                {
                    if (simBodies[j] != simBodies[i])
                    {
                        Vector3 oldPos = simBodies[i].position;
                        Vector3 posPrime = oldPos + StaticVariables.physicsTimeScale * simBodies[i].velocity;
                        simBodies[i].velocity += StaticVariables.physicsTimeScale * CalculateAcceleration(simBodies[i], simBodies[j]);
                        simBodies[i].position = posPrime;
                    }
                }
                paths[i].points.Add(simBodies[i].position);
            }
        }
        foreach (Path path in paths) 
        {
            GameObject newLine = Instantiate(lineObject, transform.position, transform.rotation);
            newLine.transform.parent = this.gameObject.transform;
            newLine.GetComponent<LineRenderer>().positionCount = path.points.Count;
            for (int i = 0; i < path.points.Count; i++)
            {
                newLine.GetComponent<LineRenderer>().SetPosition(i, path.points[i]);
            }
        }
    }
    
    private Vector3 CalculateAcceleration(SimCelestialBody body, SimCelestialBody otherBody)
    {
        float totalMass = (body.mass * otherBody.mass);
        float distance = ((body.position) - otherBody.position).magnitude;
        Vector3 direction = ((body.position) - otherBody.position) / distance;
        Vector3 gravityForce = -StaticVariables.gravityConstant * (totalMass / distance) * direction;
        return (gravityForce / body.mass) * StaticVariables.physicsTimeScale;
    }
}

public class SimCelestialBody
{
    public Vector3 position;
    public Vector3 velocity;
    public float surfaceGravity;
    public float radius;
    public float mass;

    public SimCelestialBody(Vector3 inPos, Vector3 inVeloc, float inSurfGravity, float inRad, float inMass)
    {
        position = inPos;
        velocity = inVeloc;
        surfaceGravity = inSurfGravity;
        radius = inRad;
        mass = inMass;
    }
}

public class Path
{
    public List<Vector3> points = new List<Vector3>();
    public Color color;
}*/