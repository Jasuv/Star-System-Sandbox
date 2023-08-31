using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static float gravityConstant = 1f; //The universal gravitational constant
                                              //(it's like 6.67430 * 10^-11 irl)
    public static float physicsDeltaTime = .1f; // Used instead of Unity's Time.deltaTime
                                                // better control over simulation speed
}