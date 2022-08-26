using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Set360VideoTexture : MonoBehaviour
{
    public VideoPlayer vp360;
    public GameObject screen360;
    public Shader shader;
    Material material;
    RenderTexture texture;

    public static Set360VideoTexture instance;

    void Awake()
    {
        instance = this;
    }

    public void SetTextureSize(int width, int height)
    {
        print(4);
        material = new Material(shader);
        texture = new RenderTexture(width, height,0);
        vp360.targetTexture = texture;
        material.SetTexture("_MainTex", texture);
        screen360.GetComponent<MeshRenderer>().material = material;
        print("width" + width + "height" + height);
    }
}
