using LightShaft.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class YoutubeVideoList<T>
{
    public List<string> youtubeVideoUrl;
}

[System.Serializable]
public class YoutubeVideo360List<T>
{
    public List<string> youtube360VideoUrl;
}

public class YoutubeVideoControl : MonoBehaviour
{
    public static YoutubeVideoControl instane;
    public GameObject videoPanel;
    public GameObject video360Panel;
    public GameObject video360UIPanel;
    public GameObject videoPanelButtonPanel;
    public GameObject mainPanel;
    public GameObject youtubePlayer;
    public GameObject youtubePlayer360;
    public GameObject video360Screen; //360�ù��y��
    public Text mainPanelAlertText;
    [SerializeField] private YoutubeVideoList<string> dbVideoUrls;
    [SerializeField] private YoutubeVideo360List<string> dbVideo360Urls;
    [SerializeField] private string getVideoUrl;
    [SerializeField] private string getVideo360Url;
    private int nowPlayedVideo;
    private int nowPlayedVideo360;

    [Header("360 Video UI")]
    /*public SmartGlass_Sensor sgs;
    public Rotation_From_SmartGlass rfsg;*/
    public GameObject video360PreviousPage;
    public GameObject video360NextPage;
    public GameObject EnptyObjectLeft; //LaygerGroup��R��
    public GameObject EnptyObjectRight; //LaygerGroup��R��
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

    //*****�v��*****//

    public void OnVideoClick()
    {
        StartCoroutine(TryGetVideoUrl());
    }

    public void OnPreviousVideoClick()
    {
        if (nowPlayedVideo == 0)
        {
            nowPlayedVideo = dbVideoUrls.youtubeVideoUrl.Count - 1;
            youtubePlayer.GetComponent<YoutubePlayer>().Play(dbVideoUrls.youtubeVideoUrl[nowPlayedVideo]);
        }
        else
        {
            nowPlayedVideo--;
            youtubePlayer.GetComponent<YoutubePlayer>().Play(dbVideoUrls.youtubeVideoUrl[nowPlayedVideo]);
        }
    }

    public void OnNextVideoClick()
    {
        if(nowPlayedVideo != dbVideoUrls.youtubeVideoUrl.Count - 1)
        {
            nowPlayedVideo++;
            youtubePlayer.GetComponent<YoutubePlayer>().Play(dbVideoUrls.youtubeVideoUrl[nowPlayedVideo]);
        }
        else
        {
            nowPlayedVideo = 0;
            youtubePlayer.GetComponent<YoutubePlayer>().Play(dbVideoUrls.youtubeVideoUrl[nowPlayedVideo]);
        }
    }

    public void OnVideoBackClick()
    {
        nowPlayedVideo = 0;
        youtubePlayer.GetComponent<YoutubePlayer>().Stop();
        mainPanel.SetActive(true);
        videoPanel.SetActive(false);
    }

    //*****�v��*****//


    //*****360�v��*****//

    public void OnVideo360Click()
    {
        StartCoroutine(TryGetVideo360Url());
    }

    //360�v���������s(���U���UI Panel)
    public void OnVideo360TouchPanelClick()
    {
        if (video360UIPanel.activeInHierarchy == false) {
            video360UIPanel.SetActive(true);
            StartCoroutine("HideYoutube360UI");
        }
        else video360UIPanel.SetActive(false);
    }

    //360�v������������s
    public void OnVideo360PlayClick()
    {
        YoutubePlayer YP = youtubePlayer360.GetComponent<YoutubePlayer>();
        AudioSource AS = youtubePlayer360.GetComponent<AudioSource>();
        YP.Play();
        AS.Pause();
        AS.volume = 1;
        StopCoroutine("HideYoutube360UI");
        StartCoroutine("HideYoutube360UI");
    }

    //360�v�������Ȱ����s
    public void OnVideo360PauseClick()
    {
        YoutubePlayer YP = youtubePlayer360.GetComponent<YoutubePlayer>();
        AudioSource AS = youtubePlayer360.GetComponent<AudioSource>();
        YP.Pause();
        AS.Pause();
        StopCoroutine("HideYoutube360UI");
        StartCoroutine("HideYoutube360UI");
    }

    //360�v��������^���s
    public void OnVideo360BackClick()
    {
        YoutubePlayer YP = youtubePlayer360.GetComponent<YoutubePlayer>();
        YP.Stop();
        Camera.main.GetComponent<GyroControl>().enabled = false;
        //rfsg.enabled = false;
        mainPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        video360Panel.SetActive(false);
        video360Screen.SetActive(false);
    }

    //360�v�������U�@�����s(360�v���M��UI�s��)
    public void OnVideo360NextPageClick()
    {
        nowVideo360ListPage++;
        if (nowVideo360ListPage == video360ListPageCount)
        {
            video360PreviousPage.SetActive(true);
            video360NextPage.SetActive(false);
            EnptyObjectLeft.SetActive(false);
            EnptyObjectRight.SetActive(true);
            switch (dbVideo360Urls.youtube360VideoUrl.Count % 5)
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

    //360�v�������W�@�����s(360�v���M��UI�s��)
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

    //360�v�������v���M��0���ث��s
    public void OnVideo360ListFrame0Click()
    {
        //video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        YoutubePlayer YP = youtubePlayer360.GetComponent<YoutubePlayer>();
        YP.Stop();
        YP.Play(dbVideo360Urls.youtube360VideoUrl[int.Parse(video360Listframe0.transform.GetChild(0).GetComponent<Text>().text) - 1]);
        nowPlayedVideo360 = int.Parse(video360Listframe0.transform.GetChild(0).GetComponent<Text>().text) - 1;
    }

    //360�v�������v���M��1���ث��s
    public void OnVideo360ListFrame1Click()
    {
        //video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        YoutubePlayer YP = youtubePlayer360.GetComponent<YoutubePlayer>();
        YP.Stop();
        YP.Play(dbVideo360Urls.youtube360VideoUrl[int.Parse(video360Listframe1.transform.GetChild(0).GetComponent<Text>().text) - 1]);
        nowPlayedVideo360 = int.Parse(video360Listframe1.transform.GetChild(0).GetComponent<Text>().text) - 1;
    }

    //360�v�������v���M��2���ث��s
    public void OnVideo360ListFrame2Click()
    {
        //video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        YoutubePlayer YP = youtubePlayer360.GetComponent<YoutubePlayer>();
        YP.Stop();
        YP.Play(dbVideo360Urls.youtube360VideoUrl[int.Parse(video360Listframe2.transform.GetChild(0).GetComponent<Text>().text) - 1]);
        nowPlayedVideo360 = int.Parse(video360Listframe2.transform.GetChild(0).GetComponent<Text>().text) - 1;
    }

    //360�v�������v���M��3���ث��s
    public void OnVideo360ListFrame3Click()
    {
        //video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        YoutubePlayer YP = youtubePlayer360.GetComponent<YoutubePlayer>();
        YP.Stop();
        YP.Play(dbVideo360Urls.youtube360VideoUrl[int.Parse(video360Listframe3.transform.GetChild(0).GetComponent<Text>().text) - 1]);
        nowPlayedVideo360 = int.Parse(video360Listframe3.transform.GetChild(0).GetComponent<Text>().text) - 1;
    }

    //360�v�������v���M��4���ث��s
    public void OnVideo360ListFrame4Click()
    {
        //video360LoadingPanel.SetActive(true);
        video360UIPanel.SetActive(false);
        YoutubePlayer YP = youtubePlayer360.GetComponent<YoutubePlayer>();
        YP.Stop();
        YP.Play(dbVideo360Urls.youtube360VideoUrl[int.Parse(video360Listframe4.transform.GetChild(0).GetComponent<Text>().text) - 1]);
        nowPlayedVideo360 = int.Parse(video360Listframe4.transform.GetChild(0).GetComponent<Text>().text) - 1;
    }

    //*****360�v��*****//



    //360�v���_�l�M��]�w
    private void Video360ListSetting()
    {
        video360ListPageCount = dbVideo360Urls.youtube360VideoUrl.Count / 5 + 1;
        switch (video360ListPageCount)
        {
            case 1:
                EnptyObjectLeft.SetActive(true);
                EnptyObjectRight.SetActive(true);
                video360PreviousPage.SetActive(false);
                video360NextPage.SetActive(false);
                switch (dbVideo360Urls.youtube360VideoUrl.Count)
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

    private IEnumerator TryGetVideoUrl()
    {
        IPSet();
        string username = UserAccount.nowUsername;
        UnityWebRequest request = UnityWebRequest.Get($"{getVideoUrl}?rUsername={username}");
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
                dbVideoUrls = JsonUtility.FromJson<YoutubeVideoList<string>>(request.downloadHandler.text);
                videoPanel.SetActive(true);
                mainPanel.SetActive(false);
                nowPlayedVideo = 0;
                youtubePlayer.GetComponent<YoutubePlayer>().Play(dbVideoUrls.youtubeVideoUrl[0]);

                //Invoke("ResetText", 2);
            }
        }
        else
        {
            Debug.Log("Ū������");
            mainPanelAlertText.text = "Ū������";
            //Invoke("ResetText", 2);
        }
        yield return null;
    }

    private IEnumerator TryGetVideo360Url()
    {
        IPSet();
        string username = UserAccount.nowUsername;
        UnityWebRequest request = UnityWebRequest.Get($"{getVideo360Url}?rUsername={username}");
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
                print(request.downloadHandler.text);
                Camera.main.GetComponent<GyroControl>().enabled = true;
                //rfsg.enabled = true;
                dbVideo360Urls = JsonUtility.FromJson<YoutubeVideo360List<string>>(request.downloadHandler.text);
                video360Panel.SetActive(true);
                mainPanel.SetActive(false);
                nowPlayedVideo360 = 0;
                print(dbVideo360Urls.youtube360VideoUrl[0]);
                youtubePlayer360.GetComponent<YoutubePlayer>().Play(dbVideo360Urls.youtube360VideoUrl[0]);
                Video360ListSetting();

                //Invoke("ResetText", 2);
            }
        }
        else
        {
            Debug.Log("Ū������");
            mainPanelAlertText.text = "Ū������";
            //Invoke("ResetText", 2);
        }
        yield return null;
    }

    private IEnumerator HideYoutube360UI()
    {
        yield return new WaitForSeconds(3f);
        video360UIPanel.SetActive(false);
    }

    private void IPSet()
    {
        getVideoUrl = string.Format("http://{0}:4000/getYoutubeVideoUrl", Info.ipAddress);
        getVideo360Url = string.Format("http://{0}:4000/getYoutube360VideoUrl", Info.ipAddress);
    }
}
