using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Unity.VideoHelper;
using UnityEngine.Video;

[System.Serializable]
public class videoList<T>
{
    public List<string> videoID;
}

[System.Serializable]
public class video360List<T>
{
    public List<string> video360ID;
}

[System.Serializable]
public class imageList<T>
{
    public List<string> imageID;
}

public class VideoImageControl : MonoBehaviour
{
    public static VideoImageControl instane;
    public GameObject imagePanel;
    public GameObject video360Panel;
    public GameObject video360LoadingPanel;
    public GameObject video360UIPanel;
    public GameObject videoPanel;
    public GameObject videoPanelButtonPanel;
    public GameObject mainPanel;
    public GameObject videoPlayer; //videoPlayer預置物
    public GameObject videoPlayerControl; //videoPlayer控制的子物件
    public GameObject video360PlayerControl; //360videoPlayer控制的子物件
    public GameObject video360Screen; //360螢幕球體
    public Image ImageDisplay; //圖片顯示的物件
    public Text mainAlert;
    private int nowPlayVideo;
    private int nowPlayVideo360;
    private int nowPlayImage;
    private int nowDownloadImage;
    private Dictionary<int, Texture2D> downloadImageTextureDic = new Dictionary<int, Texture2D>();
    //[SerializeField] private List<Texture2D> images = new List<Texture2D>();
    [SerializeField] private GameObject videoFirstPlayButton;
    [SerializeField] private GameObject videoPlayerPre; //場景生成videoPlayer物件
    [SerializeField] private videoList<string> dbVideoIDs;
    [SerializeField] private video360List<string> dbVideo360IDs;
    [SerializeField] private imageList<string> dbImageIDs;
    [SerializeField] private string getVideoID;
    [SerializeField] private string getVideo360ID;
    [SerializeField] private string getImageID;

    [Header("360 Video UI")]
    public GameObject video360PreviousPage;
    public GameObject video360NextPage;
    public GameObject EnptyObjectLeft; //LaygerGroup填充物
    public GameObject EnptyObjectRight; //LaygerGroup填充物
    public GameObject video360Listframe0;
    public GameObject video360Listframe1;
    public GameObject video360Listframe2;
    public GameObject video360Listframe3;
    public GameObject video360Listframe4;
    private int nowVideo360ListPage;
    private int video360ListPageCount;



    private void Awake()
    {
        instane = this;
    }

    //*****影片*****//

    //起始播放影片按鈕
    public void OnVideoPlayClick()
    {
        nowPlayVideo = 0;
        videoPlayerControl.GetComponent<VideoController>().PrepareForUrl(string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", dbVideoIDs.videoID[nowPlayVideo]));
        videoPlayerControl.GetComponent<VideoPlayer>().loopPointReached -= EndReached;
        videoPlayerControl.GetComponent<VideoPlayer>().loopPointReached += EndReached;
        //videoFirstPlayButton.SetActive(false);
    }

    public void OnNextVideoClick()
    {
        nowPlayVideo = (nowPlayVideo == dbVideoIDs.videoID.Count - 1) ? nowPlayVideo = 0 : nowPlayVideo += 1;
        videoPlayerControl.GetComponent<VideoPlayer>().Stop();
        videoPlayerControl.GetComponent<VideoController>().PrepareForUrl(string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", dbVideoIDs.videoID[nowPlayVideo]));
    }
    
    //影片播放介面返回按鈕
    public void OnVideoBackClick()
    {
        /*if(videoPlayer.GetComponent<VideoPlayer>() != null)
        {
            videoPlayer.GetComponent<VideoPlayer>().Stop();
        }*/
        Destroy(videoPlayerPre);
        nowPlayVideo = 0;
        mainPanel.SetActive(true);
        //videoFirstPlayButton.SetActive(true);
        videoPanel.SetActive(false);
    }

    public void OnVideoClick()
    {
        StartCoroutine(TryGetVideoID());
        videoPlayerPre = Instantiate(videoPlayer, videoPanel.transform);
        videoFirstPlayButton = videoPlayerPre.transform.GetChild(2).gameObject;
        videoPlayerControl = videoPlayerPre.transform.GetChild(1).gameObject;
        videoFirstPlayButton.GetComponent<Button>().onClick.AddListener(() => OnVideoPlayClick());
        videoFirstPlayButton.GetComponent<Button>().onClick.AddListener(() => videoFirstPlayButton.SetActive(false));
        videoPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    //*****影片*****//


    //*****360影片*****//

    public void OnVideo360Click()
    {
        mainPanel.SetActive(false);
        video360Panel.SetActive(true);
        video360LoadingPanel.SetActive(true);
        nowPlayVideo360 = 0;
        StartCoroutine(TryGetVideo360ID());
    }

    //360影片介面按鈕(按下顯示UI Panel)
    public void OnVideo360TouchPanelClick()
    {
        if (video360UIPanel.activeInHierarchy == false) video360UIPanel.SetActive(true);
        else video360UIPanel.SetActive(false);
    }

    //360影片介面播放按鈕
    public void OnVideo360PlayClick()
    {
        VideoPlayer VP = video360PlayerControl.GetComponent<VideoPlayer>();
        VP.Play();
    }

    //360影片介面暫停按鈕
    public void OnVideo360PauseClick()
    {
        VideoPlayer VP = video360PlayerControl.GetComponent<VideoPlayer>();
        VP.Pause();
    }

    //360影片介面返回按鈕
    public void OnVideo360BackClick()
    {
        VideoPlayer VP = video360PlayerControl.GetComponent<VideoPlayer>();
        VP.Stop();
        Camera.main.GetComponent<GyroControl>().enabled = false;
        mainPanel.SetActive(true);
        video360Panel.SetActive(false);
        video360Screen.SetActive(false);
    }

    //360影片介面下一頁按鈕(360影片清單UI編排)
    public void OnVideo360NextPageClick()
    {
        nowVideo360ListPage++;
        if (nowVideo360ListPage == video360ListPageCount)
        {
            video360PreviousPage.SetActive(true);
            video360NextPage.SetActive(false);
            EnptyObjectLeft.SetActive(false);
            EnptyObjectRight.SetActive(true);
            switch (dbVideo360IDs.video360ID.Count%5)
            {
                case 0:
                    video360Listframe0.SetActive(true);
                    video360Listframe1.SetActive(true);
                    video360Listframe2.SetActive(true);
                    video360Listframe3.SetActive(true);
                    video360Listframe4.SetActive(true);
                    video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 1).ToString();
                    video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 2).ToString();
                    video360Listframe2.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 3).ToString();
                    video360Listframe3.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 4).ToString();
                    video360Listframe4.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 5).ToString();
                    break;
                case 1:
                    video360Listframe0.SetActive(true);
                    video360Listframe1.SetActive(false);
                    video360Listframe2.SetActive(false);
                    video360Listframe3.SetActive(false);
                    video360Listframe4.SetActive(false);
                    video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 1).ToString();
                    break;
                case 2:
                    video360Listframe0.SetActive(true);
                    video360Listframe1.SetActive(true);
                    video360Listframe2.SetActive(false);
                    video360Listframe3.SetActive(false);
                    video360Listframe4.SetActive(false);
                    video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 1).ToString();
                    video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 2).ToString();
                    break;
                case 3:
                    video360Listframe0.SetActive(true);
                    video360Listframe1.SetActive(true);
                    video360Listframe2.SetActive(true);
                    video360Listframe3.SetActive(false);
                    video360Listframe4.SetActive(false);
                    video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 1).ToString();
                    video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 2).ToString();
                    video360Listframe2.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 3).ToString();
                    break;
                case 4:
                    video360Listframe0.SetActive(true);
                    video360Listframe1.SetActive(true);
                    video360Listframe2.SetActive(true);
                    video360Listframe3.SetActive(true);
                    video360Listframe4.SetActive(false);
                    video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 1).ToString();
                    video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 2).ToString();
                    video360Listframe2.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 3).ToString();
                    video360Listframe3.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 4).ToString();
                    break;
                default:
                    print("Error");
                    break;
            }
        }
        else
        {
            video360PreviousPage.SetActive(true);
            video360NextPage.SetActive(true);
            EnptyObjectLeft.SetActive(false);
            EnptyObjectRight.SetActive(false);
            video360Listframe0.SetActive(true);
            video360Listframe1.SetActive(true);
            video360Listframe2.SetActive(true);
            video360Listframe3.SetActive(true);
            video360Listframe4.SetActive(true);
            video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 1).ToString();
            video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 2).ToString();
            video360Listframe2.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 3).ToString();
            video360Listframe3.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 4).ToString();
            video360Listframe4.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 5).ToString();
        }
    }

    //360影片介面上一頁按鈕(360影片清單UI編排)
    public void OnVideo360PreviousClick()
    {
        nowVideo360ListPage--;
        if (nowVideo360ListPage == 1)
        {
            video360PreviousPage.SetActive(false);
            video360NextPage.SetActive(true);
            EnptyObjectLeft.SetActive(true);
            EnptyObjectRight.SetActive(false);
            video360Listframe0.SetActive(true);
            video360Listframe1.SetActive(true);
            video360Listframe2.SetActive(true);
            video360Listframe3.SetActive(true);
            video360Listframe4.SetActive(true);
            video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 1).ToString();
            video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 2).ToString();
            video360Listframe2.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 3).ToString();
            video360Listframe3.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 4).ToString();
            video360Listframe4.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 5).ToString();
        }
        else
        {
            video360PreviousPage.SetActive(true);
            video360NextPage.SetActive(true);
            EnptyObjectLeft.SetActive(false);
            EnptyObjectRight.SetActive(false);
            video360Listframe0.SetActive(true);
            video360Listframe1.SetActive(true);
            video360Listframe2.SetActive(true);
            video360Listframe3.SetActive(true);
            video360Listframe4.SetActive(true);
            video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 1).ToString();
            video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 2).ToString();
            video360Listframe2.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 3).ToString();
            video360Listframe3.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 4).ToString();
            video360Listframe4.transform.GetChild(0).GetComponent<Text>().text = (5 * (nowVideo360ListPage - 1) + 5).ToString();
        }
    }

    //360影片介面影片清單0號框按鈕
    public void OnVideo360ListFrame0Click()
    {
        video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        VideoPlayer VP = video360PlayerControl.GetComponent<VideoPlayer>();
        VP.Stop();
        VP.url = string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", dbVideo360IDs.video360ID[int.Parse(video360Listframe0.transform.GetChild(0).GetComponent<Text>().text)-1]);
        nowPlayVideo360 = int.Parse(video360Listframe0.transform.GetChild(0).GetComponent<Text>().text) - 1;
        VP.Play();
        VP.prepareCompleted -= Video360PrepareCompleted;
        VP.prepareCompleted += Video360PrepareCompleted;
    }

    //360影片介面影片清單1號框按鈕
    public void OnVideo360ListFrame1Click()
    {
        video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        VideoPlayer VP = video360PlayerControl.GetComponent<VideoPlayer>();
        VP.Stop();
        VP.url = string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", dbVideo360IDs.video360ID[int.Parse(video360Listframe1.transform.GetChild(0).GetComponent<Text>().text) - 1]);
        nowPlayVideo360 = int.Parse(video360Listframe1.transform.GetChild(0).GetComponent<Text>().text) - 1;
        VP.Play();
        VP.prepareCompleted -= Video360PrepareCompleted;
        VP.prepareCompleted += Video360PrepareCompleted;
    }

    //360影片介面影片清單2號框按鈕
    public void OnVideo360ListFrame2Click()
    {
        video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        VideoPlayer VP = video360PlayerControl.GetComponent<VideoPlayer>();
        VP.Stop();
        VP.url = string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", dbVideo360IDs.video360ID[int.Parse(video360Listframe2.transform.GetChild(0).GetComponent<Text>().text) - 1]);
        nowPlayVideo360 = int.Parse(video360Listframe2.transform.GetChild(0).GetComponent<Text>().text) - 1;
        VP.Play();
        VP.prepareCompleted -= Video360PrepareCompleted;
        VP.prepareCompleted += Video360PrepareCompleted;
    }

    //360影片介面影片清單3號框按鈕
    public void OnVideo360ListFrame3Click()
    {
        video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        VideoPlayer VP = video360PlayerControl.GetComponent<VideoPlayer>();
        VP.Stop();
        VP.url = string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", dbVideo360IDs.video360ID[int.Parse(video360Listframe3.transform.GetChild(0).GetComponent<Text>().text) - 1]);
        nowPlayVideo360 = int.Parse(video360Listframe3.transform.GetChild(0).GetComponent<Text>().text) - 1;
        VP.Play();
        VP.prepareCompleted -= Video360PrepareCompleted;
        VP.prepareCompleted += Video360PrepareCompleted;
    }

    //360影片介面影片清單4號框按鈕
    public void OnVideo360ListFrame4Click()
    {
        video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        VideoPlayer VP = video360PlayerControl.GetComponent<VideoPlayer>();
        VP.Stop();
        VP.url = string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", dbVideo360IDs.video360ID[int.Parse(video360Listframe4.transform.GetChild(0).GetComponent<Text>().text) - 1]);
        nowPlayVideo360 = int.Parse(video360Listframe4.transform.GetChild(0).GetComponent<Text>().text) - 1;
        VP.Play();
        VP.prepareCompleted -= Video360PrepareCompleted;
        VP.prepareCompleted += Video360PrepareCompleted;
    }

    //*****360影片*****//


    //*****圖片*****//

    public void OnNextImageClick()
    {
        if (nowPlayImage < dbImageIDs.imageID.Count-1)
        {
            nowPlayImage++;
            ImageDisplay.sprite = null;
            AdjustImageSize(nowPlayImage);
        }
    }

    public void OnPreviousImageClick()
    {
        if (nowPlayImage > 0)
        {
            nowPlayImage--;
            ImageDisplay.sprite = null;
            AdjustImageSize(nowPlayImage);
        }
    }

    public void OnImageBackClick()
    {
        nowPlayImage = 0;
        ImageDisplay.sprite = null;
        mainPanel.SetActive(true);
        imagePanel.SetActive(false);
    }

    public void OnImageClick()
    {
        StartCoroutine(TryGetImageID());
        mainAlert.text = "讀取圖片中";
    }

    //*****圖片*****//





    //影片結束切換下一部
    private void EndReached(VideoPlayer vp)
    {
        nowPlayVideo = (nowPlayVideo == dbVideoIDs.videoID.Count - 1) ? nowPlayVideo = 0 : nowPlayVideo += 1;
        videoPlayerControl.GetComponent<VideoPlayer>().Stop();
        videoPlayerControl.GetComponent<VideoController>().PrepareForUrl(string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", dbVideoIDs.videoID[nowPlayVideo]));
    }

    //360影片結束切換下一部
    private void Video360EndReached(VideoPlayer vp)
    {
        video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        nowPlayVideo360 = (nowPlayVideo360 == dbVideo360IDs.video360ID.Count - 1) ? nowPlayVideo360 = 0 : nowPlayVideo360 += 1;
        vp.Stop();
        vp.url = string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", dbVideo360IDs.video360ID[nowPlayVideo360]);
        vp.Play();
        vp.prepareCompleted -= Video360PrepareCompleted;
        vp.prepareCompleted += Video360PrepareCompleted;
    }

    //360影片起始準備完成後
    private void Video360PrepareCompleted(VideoPlayer VP)
    {
        print(3);
        VideoPlayer vp = video360PlayerControl.GetComponent<VideoPlayer>();
        print(vp.width+"-"+ vp.height);
        Set360VideoTexture.instance.SetTextureSize((int)vp.width, (int)vp.height);
        video360LoadingPanel.SetActive(false);
    }

    //360影片起始清單設定
    private void Video360ListSetting()
    {
        video360ListPageCount = dbVideo360IDs.video360ID.Count / 5 + 1;
        switch (video360ListPageCount)
        {
            case 1:
                EnptyObjectLeft.SetActive(true);
                EnptyObjectRight.SetActive(true);
                video360PreviousPage.SetActive(false);
                video360NextPage.SetActive(false);
                switch (dbVideo360IDs.video360ID.Count)
                {
                    case 1:
                        video360Listframe0.SetActive(true);
                        video360Listframe1.SetActive(false);
                        video360Listframe2.SetActive(false);
                        video360Listframe3.SetActive(false);
                        video360Listframe4.SetActive(false);
                        video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = "1";
                        break;
                    case 2:
                        video360Listframe0.SetActive(true);
                        video360Listframe1.SetActive(true);
                        video360Listframe2.SetActive(false);
                        video360Listframe3.SetActive(false);
                        video360Listframe4.SetActive(false);
                        video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = "1";
                        video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = "2";
                        break;
                    case 3:
                        video360Listframe0.SetActive(true);
                        video360Listframe1.SetActive(true);
                        video360Listframe2.SetActive(true);
                        video360Listframe3.SetActive(false);
                        video360Listframe4.SetActive(false);
                        video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = "1";
                        video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = "2";
                        video360Listframe2.transform.GetChild(0).GetComponent<Text>().text = "3";
                        break;
                    case 4:
                        video360Listframe0.SetActive(true);
                        video360Listframe1.SetActive(true);
                        video360Listframe2.SetActive(true);
                        video360Listframe3.SetActive(true);
                        video360Listframe4.SetActive(false);
                        video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = "1";
                        video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = "2";
                        video360Listframe2.transform.GetChild(0).GetComponent<Text>().text = "3";
                        video360Listframe3.transform.GetChild(0).GetComponent<Text>().text = "4";
                        break;
                    case 5:
                        video360Listframe0.SetActive(true);
                        video360Listframe1.SetActive(true);
                        video360Listframe2.SetActive(true);
                        video360Listframe3.SetActive(true);
                        video360Listframe4.SetActive(true);
                        video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = "1";
                        video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = "2";
                        video360Listframe2.transform.GetChild(0).GetComponent<Text>().text = "3";
                        video360Listframe3.transform.GetChild(0).GetComponent<Text>().text = "4";
                        video360Listframe4.transform.GetChild(0).GetComponent<Text>().text = "5";
                        break;
                    default:
                        print("No Value");
                        break;
                }
                break;
            default:
                video360Listframe0.SetActive(true);
                video360Listframe1.SetActive(true);
                video360Listframe2.SetActive(true);
                video360Listframe3.SetActive(true);
                video360Listframe4.SetActive(true);
                video360Listframe0.transform.GetChild(0).GetComponent<Text>().text = "1";
                video360Listframe1.transform.GetChild(0).GetComponent<Text>().text = "2";
                video360Listframe2.transform.GetChild(0).GetComponent<Text>().text = "3";
                video360Listframe3.transform.GetChild(0).GetComponent<Text>().text = "4";
                video360Listframe4.transform.GetChild(0).GetComponent<Text>().text = "5";
                video360PreviousPage.SetActive(false);
                video360NextPage.SetActive(true);
                EnptyObjectLeft.SetActive(true);
                EnptyObjectRight.SetActive(false);
                print(2);
                break;
        }
        nowVideo360ListPage = 1;
    }

    private IEnumerator TryGetVideoID()
    {
        IPSet();
        string username = UserAccount.nowUsername;
        UnityWebRequest request = UnityWebRequest.Get($"{getVideoID}?rUsername={username}");
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;

            if (startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.downloadHandler.text != "Invalid credentials")
            {
                print(request.downloadHandler.text);
                dbVideoIDs = JsonUtility.FromJson<videoList<string>>(request.downloadHandler.text);
                //Invoke("ResetText", 2);
            }
        }
        else
        {
            Debug.Log("讀取失敗");
            //Invoke("ResetText", 2);
        }
        yield return null;
    }

    private IEnumerator TryGetVideo360ID()
    {
        IPSet();
        string username = UserAccount.nowUsername;
        UnityWebRequest request = UnityWebRequest.Get($"{getVideo360ID}?rUsername={username}");
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;

            if (startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.downloadHandler.text != "Invalid credentials")
            {
                video360Screen.SetActive(true);
                dbVideo360IDs = JsonUtility.FromJson<video360List<string>>(request.downloadHandler.text);
                Camera.main.GetComponent<GyroControl>().enabled = true;
                print(1);
                Video360ListSetting();
                VideoPlayer VP = video360PlayerControl.GetComponent<VideoPlayer>();
                VP.url = string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}",dbVideo360IDs.video360ID[nowPlayVideo360]);
                VP.prepareCompleted -= Video360PrepareCompleted;
                VP.prepareCompleted += Video360PrepareCompleted;
                VP.loopPointReached -= Video360EndReached;
                VP.loopPointReached += Video360EndReached;
                //Invoke("ResetText", 2);
            }
        }
        else
        {
            Debug.Log("讀取失敗");
            //Invoke("ResetText", 2);
        }
        yield return null;
    }

    private IEnumerator TryGetImageID()
    {
        IPSet();
        string username = UserAccount.nowUsername;
        UnityWebRequest request = UnityWebRequest.Get($"{getImageID}?rUsername={username}");
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;

            if (startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.downloadHandler.text != "Invalid credentials")
            {
                print(request.downloadHandler.text);
                dbImageIDs = JsonUtility.FromJson<imageList<string>>(request.downloadHandler.text);
                nowDownloadImage = 0;
                nowPlayImage = 0;
                foreach (string id in dbImageIDs.imageID)
                {
                    StartCoroutine(DownLoadImage(id,nowDownloadImage));
                    nowDownloadImage++;
                }
                //UpdateAlertImage_txt.text = "上傳成功";
                //Invoke("ResetText", 2);
            }
        }
        else
        {
            Debug.Log("讀取失敗");
            //Invoke("ResetText", 2);
        }


        yield return null;
    }

    //下載圖片
    private IEnumerator DownLoadImage(string id, int nowDownloadNum)
    {
        //print(string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", id));
        //filePath = string.Format("{0}/Resources/{1}.jpg", Application.dataPath, id);
        UnityWebRequest uwr = new UnityWebRequest(string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", id), UnityWebRequest.kHttpVerbGET);
        uwr.downloadHandler = new DownloadHandlerTexture();
        yield return uwr.SendWebRequest();
        Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
        downloadImageTextureDic.Add(nowDownloadNum, texture);
        //images.Add(texture);
        if(nowDownloadNum == 0)
        {
            AdjustImageSize(0);
        }
        else if (nowDownloadNum == dbImageIDs.imageID.Count -1)
        {
            imagePanel.SetActive(true);
            mainPanel.SetActive(false);
            ResetAlert();
        }
    }

    private void AdjustImageSize(int imageNum)
    {
        //RectTransform ImageDisplayRect = ImageDisplay.GetComponent<RectTransform>();
        //ImageDisplay.sprite = Sprite.Create(images[imageNum], new Rect(0, 0, images[imageNum].width, images[imageNum].height), new Vector2(0.5f, 0.5f));
        ImageDisplay.sprite = Sprite.Create(downloadImageTextureDic[imageNum], new Rect(0, 0, downloadImageTextureDic[imageNum].width, downloadImageTextureDic[imageNum].height), new Vector2(0.5f, 0.5f));
        ImageDisplay.GetComponent<Image>().preserveAspect = true;
    }

    private void IPSet()
    {
        getVideoID = string.Format("http://{0}:4000/getDriveVideoID", Info.ipAddress);
        getVideo360ID = string.Format("http://{0}:4000/getDrive360VideoID", Info.ipAddress);
        getImageID = string.Format("http://{0}:4000/getDriveImageID", Info.ipAddress);
    }

    private void ResetAlert()
    {
        mainAlert.text = "";
    }
}
