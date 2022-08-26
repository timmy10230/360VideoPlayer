using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Video360Manager : MonoBehaviour
{
    VideoPlayer VP;
    // Start is called before the first frame update
    void Start()
    {
        VP = Set360VideoTexture.instance.vp360;
        VP.prepareCompleted += (x) => Set360VideoTexture.instance.SetTextureSize((int)VP.width, (int)VP.height);
    }
}
