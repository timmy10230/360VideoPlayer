using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GyroControl : MonoBehaviour
{
    //public Text testc_txt;

    private bool gyroEnabled;
    private Gyroscope gyro;

    private GameObject cameraContainer;
    private Quaternion rot;

    //private string valueGyro;
    //private JJSensorManager sensorManager;
    //private SensorDataListener sensorDataListener;

    /*private void Awake()
    {
        sensorManager = new JJSensorManager();
        sensorDataListener = new SensorDataListener(OnSensorDataChanged);
    }*/

    private void Start()
    {
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);

        gyroEnabled = EnableGyro();

        /*sensorManager.Open(JJSensorManager.SensorType.ACCELEROMETER_3D);
        sensorManager.Open(JJSensorManager.SensorType.AMBIENTLIGHT);
        sensorManager.Open(JJSensorManager.SensorType.COMPASS_3D);
        sensorManager.Open(JJSensorManager.SensorType.DEVICE_ORIENTATION);
        sensorManager.Open(JJSensorManager.SensorType.GRAVITY_VECTOR);
        sensorManager.Open(JJSensorManager.SensorType.GYROMETER_3D);
        sensorManager.Open(JJSensorManager.SensorType.LINEAR_ACCELEROMETER);*/
    }

    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            cameraContainer.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
            rot = new Quaternion(0, 0, 1, 0);

            return true;
        }
        return false;
    }
    private void Update()
    {
        //testc_txt.text = "GYRO :" + valueGyro;
        if (gyroEnabled)
        {
            transform.localRotation = gyro.attitude * rot;
        }
    }

    /*public void OnSensorDataChanged(int sensorType, float[] values, long timestamp)
    {
        string str = string.Join(" ", values);
        Debug.Log("OnSensorDataChanged value :" + str);
        switch (sensorType)
        {
            case (int)JJSensorManager.SensorType.ACCELEROMETER_3D:
                //testc_txt.text = str;
                break;
            case (int)JJSensorManager.SensorType.COMPASS_3D:
                //testc_txt.text = str;
                break;
            case (int)JJSensorManager.SensorType.DEVICE_ORIENTATION:
                //testc_txt.text = str;
                break;
            case (int)JJSensorManager.SensorType.GRAVITY_VECTOR:
                //testc_txt.text = str;
                break;
            case (int)JJSensorManager.SensorType.GYROMETER_3D:
                //valueGyro = str;
                break;
            case (int)JJSensorManager.SensorType.LINEAR_ACCELEROMETER:
                //testc_txt.text = str;
                break;
            case (int)JJSensorManager.SensorType.AMBIENTLIGHT:
                //testc_txt.text = str;
                break;
        }
    }*/
}