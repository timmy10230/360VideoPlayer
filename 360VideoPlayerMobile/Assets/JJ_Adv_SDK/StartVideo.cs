using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class StartVideo : MonoBehaviour
{
    public VideoPlayer MyVideo;
    public Button StartButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //StartButton.onClick.AddListener(StartButtonOnClick);
    }

    public void StartButtonOnClick() {
        
        MyVideo.Play();
        StartButton.gameObject.SetActive(false);
    }
}
