using LightShaft.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YoutubeLight;
using System;
using UnityEngine.Networking;

[Serializable]
public class YoutubeVideoList<T>
{
    public List<string> youtubeVideoUrl;
}

[Serializable]
public class YoutubeVideo360List<T>
{
    public List<string> youtube360VideoUrl;
}

public class YoutubeManager : MonoBehaviour
{
    public GameObject youtubeVideoUrlPre;
    public GameObject youtubeVideo360UrlPre;
    public GameObject youtubeEditPageVideoUpdateList;
    public GameObject youtubeEditPageVideo360UpdateList;
    public GameObject youtubePlayer;
    public GameObject youtubePlayer360;
    public Text videoAlert;
    public Text video360Alert;
    public InputField youtubeVideoUrl;
    public InputField youtubeVideo360Url;

    [SerializeField] private YoutubeVideoList<string> videoUrls;
    [SerializeField] private YoutubeVideo360List<string> video360Urls;
    [SerializeField] private string updateVideoUrl = "http://127.0.0.1:4000/updateYoutubeVideoUrl";
    [SerializeField] private string updateVideo360Url = "http://127.0.0.1:4000/updateYoutube360VideoUrl";
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject mainPanel;

    public void OnLoginPanelUseYoutubeClick()
    {
        mainPanel.SetActive(true);
        loginPanel.SetActive(false);
    }

    public void OnAddYoutubeVideoUrlClick()
    {
        if (TryNormalizeYoutubeUrlLocal(youtubeVideoUrl.text) == true)
        {
            GameObject urlObject = Instantiate(youtubeVideoUrlPre, youtubeEditPageVideoUpdateList.transform);
            urlObject.GetComponent<DragController>().currentTransform = urlObject.GetComponent<RectTransform>();
            urlObject.name = youtubeVideoUrl.text;
            urlObject.GetComponent<Button>().onClick.AddListener(() => PlayVideo(urlObject.name));
            urlObject.transform.GetChild(0).GetComponent<Text>().text = youtubeVideoUrl.text;
            urlObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => Destroy(urlObject));
            youtubeVideoUrl.text = null;
        }
        else
        {
            videoAlert.text = "非Youtube影片網址";
            Invoke("ResetTxt", 3);
        }
    }

    public void OnYouTubeVideoBackClick()
    {
        for (int i = 0; i < youtubeEditPageVideoUpdateList.transform.childCount; i++)
        {
            GameObject go = youtubeEditPageVideoUpdateList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        loginPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void OnYoutubeUpdateClick()
    {
        videoUrls.youtubeVideoUrl.Clear();
        for (int i = 0; i < youtubeEditPageVideoUpdateList.transform.childCount; i++)
        {
            videoUrls.youtubeVideoUrl.Add(SplitUrl(youtubeEditPageVideoUpdateList.transform.GetChild(i).name));
        }
        print(youtubeEditPageVideoUpdateList.transform.childCount);
        StartCoroutine(TryUpdateVideoUrl());

    }

    public void OnAddYoutubeVideo360UrlClick()
    {
        if (TryNormalizeYoutubeUrlLocal(youtubeVideo360Url.text) == true)
        {
            GameObject urlObject = Instantiate(youtubeVideo360UrlPre, youtubeEditPageVideo360UpdateList.transform);
            urlObject.GetComponent<DragController>().currentTransform = urlObject.GetComponent<RectTransform>();
            urlObject.name = youtubeVideo360Url.text;
            urlObject.GetComponent<Button>().onClick.AddListener(() => PlayVideo360(urlObject.name));
            urlObject.transform.GetChild(0).GetComponent<Text>().text = youtubeVideo360Url.text;
            urlObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => Destroy(urlObject));
            youtubeVideo360Url.text = null;
        }
        else
        {
            video360Alert.text = "非Youtube影片網址";
            Invoke("ResetTxt", 3);
        }
    }

    public void OnYouTubeVideo360BackClick()
    {
        for (int i = 0; i < youtubeEditPageVideo360UpdateList.transform.childCount; i++)
        {
            GameObject go = youtubeEditPageVideo360UpdateList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        loginPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void OnYoutubeUpdate360Click()
    {
        video360Urls.youtube360VideoUrl.Clear();
        for (int i = 0; i < youtubeEditPageVideo360UpdateList.transform.childCount; i++)
        {
            video360Urls.youtube360VideoUrl.Add(SplitUrl(youtubeEditPageVideo360UpdateList.transform.GetChild(i).name));
        }
        print(youtubeEditPageVideo360UpdateList.transform.childCount);
        StartCoroutine(TryUpdateVideo360Url());

    }

    private void PlayVideo(string youtubeUrl)
    {
        youtubePlayer.GetComponent<YoutubePlayer>().youtubeUrl = youtubeUrl;
        youtubePlayer.GetComponent<YoutubePlayer>().Play(youtubeUrl);
    }

    private void PlayVideo360(string youtubeUrl)
    {
        youtubePlayer360.GetComponent<YoutubePlayer>().youtubeUrl = youtubeUrl;
        youtubePlayer360.GetComponent<YoutubePlayer>().Play(youtubeUrl);
    }

    private IEnumerator TryUpdateVideoUrl()
    {
        string username = UserAccount.nowUsername;
        string rYoutubeVideo = JsonUtility.ToJson(videoUrls);
        foreach (var item in videoUrls.youtubeVideoUrl)
        {
            print(item);
        }
        UnityWebRequest request = UnityWebRequest.Get($"{updateVideoUrl}?rUsername={username}&rYoutubeVideoUrl={rYoutubeVideo}");
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
                print("上傳成功");
                videoAlert.text = "上傳成功";
                Invoke("ResetTxt", 2);
            }
        }
        else
        {
            Debug.Log("上傳失敗");
            videoAlert.text = "上傳失敗";
            Invoke("ResetTxt", 2);
        }


        yield return null;
    }

    private IEnumerator TryUpdateVideo360Url()
    {
        string username = UserAccount.nowUsername;
        string rYoutubeVideo360 = JsonUtility.ToJson(video360Urls);
        foreach (var item in video360Urls.youtube360VideoUrl)
        {
            print(item);
        }
        UnityWebRequest request = UnityWebRequest.Get($"{updateVideo360Url}?rUsername={username}&rYoutube360VideoUrl={rYoutubeVideo360}");
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
                print("上傳成功");
                video360Alert.text = "上傳成功";
                Invoke("ResetTxt", 2);
            }
        }
        else
        {
            Debug.Log("上傳失敗");
            videoAlert.text = "上傳失敗";
            Invoke("ResetTxt", 2);
        }


        yield return null;
    }

    //刪除Url'&'符號後字串
    private string SplitUrl(String url)
    {
        string[] Array = url.Split('&');
        return Array[0];
    }

    //確定是否為Youtube影片
    private bool TryNormalizeYoutubeUrlLocal(string url)
    {
        url = url.Trim();
        url = url.Replace("youtu.be/", "youtube.com/watch?v=");
        url = url.Replace("www.youtube", "youtube");
        url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");

        if (url.Contains("/v/"))
        {
            url = "https://youtube.com" + new System.Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
        }

        url = url.Replace("/watch#", "/watch?");
        IDictionary<string, string> query = HTTPHelperYoutube.ParseQueryString(url);

        string v;


        if (!query.TryGetValue("v", out v))
        {
            print(false);
            return false;
        }

        print(true);
        return true;
    }

    private void ResetTxt()
    {
        videoAlert.text = "";
        //video360Alert.text = "";
    }
}
