using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SmartGlass_Sensor : MonoBehaviour
{

    private JJSensorManager sensor_Manager;
    private SensorDataListener sensor_Data_Listener;

    public Quaternion SmartGlass_Rotation;



    void Awake() {

        sensor_Manager = new JJSensorManager();
        sensor_Data_Listener = new SensorDataListener( On_Sensor_Data_Changed );

        sensor_Manager.AddSensorDataListener( sensor_Data_Listener );

    }



    // Start is called before the first frame update
    void Start()
    {
        
        sensor_Manager.Open( JJSensorManager.SensorType.ACCELEROMETER_3D );
        // sensor_Manager.Open(JJSensorManager.SensorType.AMBIENTLIGHT);
        sensor_Manager.Open( JJSensorManager.SensorType.COMPASS_3D );
        sensor_Manager.Open( JJSensorManager.SensorType.DEVICE_ORIENTATION );
        sensor_Manager.Open( JJSensorManager.SensorType.GRAVITY_VECTOR );
        sensor_Manager.Open( JJSensorManager.SensorType.GYROMETER_3D );
        sensor_Manager.Open( JJSensorManager.SensorType.LINEAR_ACCELEROMETER );

    }



    // Update is called once per frame
    void Update()
    {
        
    }



    void OnApplicationQuit()
    {

        sensor_Manager.Close( JJSensorManager.SensorType.ACCELEROMETER_3D );
        // sensor_Manager.Close( JJSensorManager.SensorType.AMBIENTLIGHT );
        sensor_Manager.Close( JJSensorManager.SensorType.COMPASS_3D );
        sensor_Manager.Close( JJSensorManager.SensorType.DEVICE_ORIENTATION );
        sensor_Manager.Close( JJSensorManager.SensorType.GRAVITY_VECTOR );
        sensor_Manager.Close( JJSensorManager.SensorType.GYROMETER_3D );
        sensor_Manager.Close( JJSensorManager.SensorType.LINEAR_ACCELEROMETER );

        sensor_Manager.Release();
        
    }



    public void On_Sensor_Data_Changed(
            int sensorType,
            float[] values,
            long timestamp
        )
    {

        if ( sensorType == ( int ) JJSensorManager.SensorType.DEVICE_ORIENTATION )
        {
            SmartGlass_Rotation.Set(
                values[ 0 ],    // x.
                values[ 1 ],    // y.
                values[ 2 ],    // z.
                values[ 3 ]     // w.
            );
        }
        
    }

}


