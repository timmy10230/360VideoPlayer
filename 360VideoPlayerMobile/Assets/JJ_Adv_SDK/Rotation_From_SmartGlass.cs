﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Rotation_From_SmartGlass : MonoBehaviour
{

    public SmartGlass_Sensor smartGlass_Sensor;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        
        transform.rotation = smartGlass_Sensor.SmartGlass_Rotation;
        transform.rotation *= Quaternion.Euler(0, 0, 180); ;
        transform.Rotate( 90.0f, 0.0f, 180.0f, Space.World );
        

    }

}


