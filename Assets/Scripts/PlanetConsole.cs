using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlanetConsole : MonoBehaviour
{
    public UniversalGravitation solarScript;
    public CameraController cameraScript;
    public PathPrediction predictionScript;
    public GameObject strInput;
    public GameObject intInput;
    public Button bodyName;
    public Button velocityX;
    public Button velocityY;
    public Button mass;
    public Button radius;
    public Button destroyer;
    public Text bodyNameTxt;
    public Text positionTxt;
    public Text velocityXTxt;
    public Text velocityYTxt;
    public Text surfaceGravityTxt;
    public Text radiusTxt;
    public Text massTxt;
    public Dropdown colorList;
    public bool reset = false;

    private string selected;

    private void Awake()
    {
        strInput.SetActive(false);
        intInput.SetActive(false);
    }

    private void Update()
    {
        if (cameraScript.selectedUI != null)
        {
            bodyNameTxt.text = "" + cameraScript.selectedUI.name;
            Vector2 newBodyPos = new Vector2(cameraScript.selectedUI.GetComponent<CelestialBody>().position.x, 
                                             cameraScript.selectedUI.GetComponent<CelestialBody>().position.z);
            positionTxt.text = "" + newBodyPos;
            velocityXTxt.text = "" + cameraScript.selectedUI.GetComponent<CelestialBody>().velocity.x;
            velocityYTxt.text = "" + cameraScript.selectedUI.GetComponent<CelestialBody>().velocity.z;
            surfaceGravityTxt.text = "" + cameraScript.selectedUI.GetComponent<CelestialBody>().surfaceGravity;
            radiusTxt.text = "" + cameraScript.selectedUI.GetComponent<CelestialBody>().radius;
            massTxt.text = "" + cameraScript.selectedUI.GetComponent<CelestialBody>().mass / 100;
        }

        if (solarScript.play)
            destroyer.interactable = false;
        else
            destroyer.interactable = true;
    }

    public void SelectName() 
    {
        strInput.SetActive(true);
        strInput.transform.position = bodyName.transform.position;
    }

    public void Select(int choice) 
    {
        if (choice == 0)
        {
            selected = "velx";
            intInput.SetActive(true);
            intInput.transform.position = velocityX.transform.position;
        }
        if (choice == 1)
        {
            selected = "vely";
            intInput.SetActive(true);
            intInput.transform.position = velocityY.transform.position;
        }
        if (choice == 2)
        {
            selected = "mass";
            intInput.SetActive(true);
            intInput.transform.position = mass.transform.position;
        }
        if (choice == 3)
        {
            selected = "rad";
            intInput.SetActive(true);
            intInput.transform.position = radius.transform.position;
        }
    }

    public void GetString(string newName) 
    {
        cameraScript.selectedUI.name = newName;
        strInput.GetComponent<InputField>().text = "";
        strInput.SetActive(false);
        reset = true;
}

    public void GetInt(string inNum)
    {
        if (int.TryParse(inNum, out int value))
        {
            if (selected == "velx")
            {
                cameraScript.selectedUI.GetComponent<CelestialBody>().velocity.x = int.Parse(inNum);
                intInput.GetComponent<InputField>().text = "";
                intInput.SetActive(false);
                reset = true;
            }
            else if (selected == "vely")
            {
                cameraScript.selectedUI.GetComponent<CelestialBody>().velocity.z = int.Parse(inNum);
                intInput.GetComponent<InputField>().text = "";
                intInput.SetActive(false);
                reset = true;
            }
            else if (selected == "mass")
            {
                cameraScript.selectedUI.GetComponent<CelestialBody>().mass = int.Parse(inNum) * 100;
                intInput.GetComponent<InputField>().text = "";
                intInput.SetActive(false);
                reset = true;
            }
            else if (selected == "rad")
            {
                cameraScript.selectedUI.GetComponent<CelestialBody>().radius = int.Parse(inNum);
                intInput.GetComponent<InputField>().text = "";
                intInput.SetActive(false);
                reset = true;
            }
            else
            {
                Debug.Log("you fucked up somewhere dumbass");
            }
        }
        else 
        {
            Debug.Log("you tried inputting a string buddy");
        }
    }

    public void ColorMe(int i) 
    {
        if (i == 0)
            cameraScript.selectedUI.GetComponent<CelestialBody>().color = Color.gray;
        if (i == 1)
            cameraScript.selectedUI.GetComponent<CelestialBody>().color = Color.red;
        if (i == 2)
            cameraScript.selectedUI.GetComponent<CelestialBody>().color = Color.yellow;
        if (i == 3)
            cameraScript.selectedUI.GetComponent<CelestialBody>().color = Color.green;
        if (i == 4)
            cameraScript.selectedUI.GetComponent<CelestialBody>().color = Color.blue;
        if (i == 5)
            cameraScript.selectedUI.GetComponent<CelestialBody>().color = Color.magenta;
    }

    public void DestroyBody() 
    {
        Destroy(cameraScript.selectedUI);
        cameraScript.selectedUI = null;
        reset = true;
    }
}
