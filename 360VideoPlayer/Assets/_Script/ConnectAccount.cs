using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;
using UnityEngine.UI;
using UnityEngine.Networking;
using Unity.VideoHelper;
using UnityEngine.Video;
using System;

[System.Serializable]
public class video360List<T>
{
    public List<string> video360ID;
}

[System.Serializable]
public class videoList<T>
{
    public List<string> videoID;
}

[System.Serializable]
public class imageList<T>
{
    public List<string> imageID;
}

public class ConnectAccount : MonoBehaviour
{
    [SerializeField] private videoList<string> videoIDs;
    [SerializeField] private videoList<string> correctVideoFormatIDs;
    [SerializeField] private videoList<string> updateVideoIDs;
    [SerializeField] private video360List<string> video360IDs;
    [SerializeField] private video360List<string> correctVideo360FormatIDs;
    [SerializeField] private video360List<string> updateVideo360IDs;
    [SerializeField] private imageList<string> imageIDs;
    [SerializeField] private imageList<string> correctImageFormatIDs;
    [SerializeField] private imageList<string> updateImageIDs;
    //public List<string> videoID = new List<string>();
    public Text AccountInfo_txt;
    public Text Alert_txt;
    public Text UpdateAlertVideo_txt;
    public Text UpdateAlertVideo360_txt;
    public Text UpdateAlertImage_txt;
    public Image ImageDisplay;
    public GameObject VideoPlayer;
    public GameObject Video360Player;
    public GameObject LoginPage;
    public GameObject MainPage;
    public GameObject EditPage;
    public GameObject editPageVideoPannel;
    public GameObject editPageVideo360Pannel;
    public GameObject editPageImagePannel;
    public GameObject editPageFileObject;
    public GameObject editPageVideoFileList;
    public GameObject editPageVideo360FileList;
    public GameObject editPageImageFileList;
    public GameObject editPageViedoUpdateList;
    public GameObject editPageViedo360UpdateList;
    public GameObject editPageImageUpdateList;
    public InputField videoURL;
    public InputField video360URL;
    public InputField imageURL;
    private GoogleDriveAbout.GetRequest request;
    private GoogleDriveFiles.ListRequest VideoListRequest;
    private GoogleDriveFiles.ListRequest Video360ListRequest;
    private GoogleDriveFiles.ListRequest ImageListRequest;
    private GoogleDriveSettings settings;
    private int nowDownloadImage;
    [SerializeField] private Dictionary<int, Texture2D> downloadImageTextureDic = new Dictionary<int, Texture2D>();
    //[SerializeField] private Texture[] ttt = new Texture[10];
    [SerializeField] private string VideoFolderURL = "";
    [SerializeField] private string Video360FolderURL = "";
    [SerializeField] private string ImageFolderURL = "";

    [SerializeField] private string updateVideoID = "http://127.0.0.1:4000/updateDriveVideoID";
    [SerializeField] private string updateVideo360ID = "http://127.0.0.1:4000/updateDrive360VideoID";
    [SerializeField] private string updateImageID = "http://127.0.0.1:4000/updateDriveImageID";

    void Start()
    {
        VideoPlayer.GetComponent<VideoController>().OnStartedPlaying.AddListener(()=>VideoPlayer.GetComponent<VideoPresenter>().LoadingIndicator.gameObject.SetActive(false));
        Video360Player.GetComponent<VideoController>().OnStartedPlaying.AddListener(() => VideoPlayer.GetComponent<VideoPresenter>().LoadingIndicator.gameObject.SetActive(false));
    }

    public void LogInGoogle()
    {
        UpdateInfo();
        settings = GoogleDriveSettings.LoadFromResources();
        request.Send().OnDone += (a) => MainPage.SetActive(true);
        request.Send().OnDone += (a) => LoginPage.SetActive(false);
        request.Send().OnDone += (a) => AccountInfo_txt.text = string.Format("使用者名稱: {0}\nGoogle drive 帳號: {1}", UserAccount.nowUsername, request.ResponseData.User.EmailAddress);
    }

    public void LogOutGoogle()
    {
        if (settings.IsAnyAuthTokenCached())
        {
            settings.DeleteCachedAuthTokens();
        }
    }

    /*public void RefreshAccount()
    {
        UpdateInfo();
    }*/

    //上傳資料到資料庫(編輯頁面編輯按鈕)
    public void UploadFileID()
    {
        VideoListRequest = GoogleDriveFiles.List();
        VideoListRequest.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
        Video360ListRequest = GoogleDriveFiles.List();
        Video360ListRequest.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
        ImageListRequest = GoogleDriveFiles.List();
        ImageListRequest.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
        if (!string.IsNullOrEmpty(videoURL.text))
        {
            string videoID = SplitFolderURL(videoURL.text);
            print(videoID);
            VideoListRequest.Q = string.Format("'{0}' in parents", videoID);
            VideoListRequest.Send().OnDone += BuildResultsVideo;
        }
        if (!string.IsNullOrEmpty(video360URL.text))
        {
            string video360ID = SplitFolderURL(video360URL.text);
            print(video360ID);
            Video360ListRequest.Q = string.Format("'{0}' in parents", video360ID);
            Video360ListRequest.Send().OnDone += BuildResultsVideo360;
        }
        if (!string.IsNullOrEmpty(imageURL.text))
        {
            string imageID = SplitFolderURL(imageURL.text);
            print(imageID);
            ImageListRequest.Q = string.Format("'{0}' in parents", imageID);
            ImageListRequest.Send().OnDone += BuildResultsImage;
        }
    }

    //上傳影片到資料庫
    public void UploadVideoID()
    {
        updateVideoIDs.videoID.Clear();
        for (int i = 0; i < editPageViedoUpdateList.transform.childCount; i++)
        {
            updateVideoIDs.videoID.Add(editPageViedoUpdateList.transform.GetChild(i).transform.name);
        }
        StartCoroutine(TryUpdateVideoID());
    }

    //上傳360影片到資料庫
    public void UploadVideo360ID()
    {
        updateVideo360IDs.video360ID.Clear();
        for (int i = 0; i < editPageViedo360UpdateList.transform.childCount; i++)
        {
            updateVideo360IDs.video360ID.Add(editPageViedo360UpdateList.transform.GetChild(i).transform.name);
        }
        StartCoroutine(TryUpdateVideo360ID());
    }

    //上傳圖片到資料庫
    public void UploadImageID()
    {
        updateImageIDs.imageID.Clear();
        for (int i = 0; i < editPageImageUpdateList.transform.childCount; i++)
        {
            updateImageIDs.imageID.Add(editPageImageUpdateList.transform.GetChild(i).transform.name);
        }
        StartCoroutine(TryUpdateImageID());
    }

    //影片重製按鈕
    public void ResetVideoFolderFile()
    {
        if (VideoFolderURL != null)
        {
            VideoListRequest = GoogleDriveFiles.List();
            VideoListRequest.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
            string videoID = SplitFolderURL(videoURL.text);
            VideoListRequest.Q = string.Format("'{0}' in parents", videoID);
            VideoListRequest.Send().OnDone += GetResetVideo;
        }
        else
        {
            UpdateAlertVideo_txt.text = "未輸入影片網址";
            Invoke("ResetText", 2);
        }
    }

    //360影片重製按鈕
    public void ResetVideo360FolderFile()
    {
        if (Video360FolderURL != null)
        {
            Video360ListRequest = GoogleDriveFiles.List();
            Video360ListRequest.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
            string video360ID = SplitFolderURL(video360URL.text);
            Video360ListRequest.Q = string.Format("'{0}' in parents", video360ID);
            Video360ListRequest.Send().OnDone += GetResetVideo360;
        }
        else
        {
            UpdateAlertVideo360_txt.text = "未輸入360°影片網址";
            Invoke("ResetText", 2);
        }
    }

    //圖片重製按鈕
    public void ResetImageFolderFile()
    {
        if (ImageFolderURL != null)
        {
            ImageListRequest = GoogleDriveFiles.List();
            ImageListRequest.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
            string imageID = SplitFolderURL(imageURL.text);
            ImageListRequest.Q = string.Format("'{0}' in parents", imageID);
            ImageListRequest.Send().OnDone += GetResetImage;
        }
        else
        {
            UpdateAlertImage_txt.text = "未輸入圖片網址";
            Invoke("ResetText", 2);
        }
    }

    //切換到編輯頁
    public void GetFileID()
    {
        VideoListRequest = GoogleDriveFiles.List();
        VideoListRequest.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
        Video360ListRequest = GoogleDriveFiles.List();
        Video360ListRequest.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
        ImageListRequest = GoogleDriveFiles.List();
        ImageListRequest.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
        if (!string.IsNullOrEmpty(videoURL.text) && VideoFolderURL != videoURL.text)
        {
            editPageVideoPannel.SetActive(true);
            string videoID = SplitFolderURL(videoURL.text);
            VideoListRequest.Q = string.Format("'{0}' in parents", videoID);
            VideoListRequest.Send().OnDone += GetResultsVideo;
        }
        else if(VideoFolderURL == videoURL.text)
        {
            editPageVideoPannel.SetActive(true);
            MainPage.SetActive(false);
            EditPage.SetActive(true);
        }
        if (!string.IsNullOrEmpty(video360URL.text) && Video360FolderURL != video360URL.text)
        {
            editPageVideoPannel.SetActive(true);
            string video360ID = SplitFolderURL(video360URL.text);
            Video360ListRequest.Q = string.Format("'{0}' in parents", video360ID);
            Video360ListRequest.Send().OnDone += GetResultsVideo360;
        }
        else if (Video360FolderURL == video360URL.text)
        {
            editPageVideoPannel.SetActive(true);
            MainPage.SetActive(false);
            EditPage.SetActive(true);
        }
        if (!string.IsNullOrEmpty(imageURL.text) && ImageFolderURL != imageURL.text)
        {
            editPageVideoPannel.SetActive(true);
            string imageID = SplitFolderURL(imageURL.text);
            ImageListRequest.Q = string.Format("'{0}' in parents", imageID);
            ImageListRequest.Send().OnDone += GetResultsImage;
        }
        else if (ImageFolderURL == imageURL.text)
        {
            editPageVideoPannel.SetActive(true);
            MainPage.SetActive(false);
            EditPage.SetActive(true);
        }
    }

    private void BuildResultsVideo(UnityGoogleDrive.Data.FileList fileList)
    {
        print("---------video----------");
        videoIDs.videoID.Clear();
        foreach (var file in fileList.Files)
        {
            videoIDs.videoID.Add(file.Id);
            //print(file.Id);
        }
        StartCoroutine(TryUpdateVideoID());
    }

    private void BuildResultsVideo360(UnityGoogleDrive.Data.FileList fileList)
    {
        print("---------360video----------");
        video360IDs.video360ID.Clear();
        foreach (var file in fileList.Files)
        {
            video360IDs.video360ID.Add(file.Id);
            //print(file.Id);
        }
        StartCoroutine(TryUpdateVideo360ID());
    }

    private void BuildResultsImage(UnityGoogleDrive.Data.FileList fileList)
    {
        print("---------image----------");
        imageIDs.imageID.Clear();
        foreach (var file in fileList.Files)
        {
            imageIDs.imageID.Add(file.Id);
            //print(file.Id);
        }
        StartCoroutine(TryUpdateImageID());
    }

    private void GetResultsVideo(UnityGoogleDrive.Data.FileList fileList)
    {
        print("---------video----------");
        for (int i = 0; i < editPageVideoFileList.transform.childCount; i++)
        {
            GameObject go = editPageVideoFileList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        for (int i = 0; i < editPageViedoUpdateList.transform.childCount; i++)
        {
            GameObject go = editPageViedoUpdateList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        VideoFolderURL = videoURL.text;
        VideoPlayer.GetComponent<VideoPresenter>().Thumbnails.Clear();
        videoIDs.videoID.Clear();
        correctVideoFormatIDs.videoID.Clear();
        foreach (var file in fileList.Files)
        {
            videoIDs.videoID.Add(file.Id);
            /*GameObject FileObject = Instantiate(editPageFileObject, editPageViedoFileList.transform);
            FileObject.name = file.Id;
            FileObject.transform.GetChild(0).transform.GetComponent<Text>().text = file.Name;*/
            ConfirmVideoFormat(file);
        }
        MainPage.SetActive(false);
        EditPage.SetActive(true);
    }

    private void GetResultsVideo360(UnityGoogleDrive.Data.FileList fileList)
    {
        print("---------360video----------");
        for (int i = 0; i < editPageVideo360FileList.transform.childCount; i++)
        {
            GameObject go = editPageVideo360FileList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        for (int i = 0; i < editPageViedo360UpdateList.transform.childCount; i++)
        {
            GameObject go = editPageViedo360UpdateList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        Video360FolderURL = video360URL.text;
        Video360Player.GetComponent<VideoPresenter>().Thumbnails.Clear();
        video360IDs.video360ID.Clear();
        correctVideo360FormatIDs.video360ID.Clear();
        foreach (var file in fileList.Files)
        {
            video360IDs.video360ID.Add(file.Id);
            /*GameObject FileObject = Instantiate(editPageFileObject, editPageViedoFileList.transform);
            FileObject.name = file.Id;
            FileObject.transform.GetChild(0).transform.GetComponent<Text>().text = file.Name;*/
            ConfirmVideo360Format(file);
        }
        MainPage.SetActive(false);
        EditPage.SetActive(true);
    }

    private void GetResultsImage(UnityGoogleDrive.Data.FileList fileList)
    {
        print("---------image----------");
        for (int i = 0; i < editPageImageFileList.transform.childCount; i++)
        {
            GameObject go = editPageImageFileList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        for (int i = 0; i < editPageImageUpdateList.transform.childCount; i++)
        {
            GameObject go = editPageImageUpdateList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        ImageFolderURL = imageURL.text;
        imageIDs.imageID.Clear();
        correctImageFormatIDs.imageID.Clear();
        nowDownloadImage = 0;
        foreach (var file in fileList.Files)
        {
            imageIDs.imageID.Add(file.Id);
            //GameObject FileObject = Instantiate(editPageFileObject, editPageImageFileList.transform);
            //FileObject.name = file.Id;
            //FileObject.transform.GetChild(0).transform.GetComponent<Text>().text = file.Name;
            ConfirmImageFormat(file);
        }
        MainPage.SetActive(false);
        EditPage.SetActive(true);
    }

    private void GetResetVideo(UnityGoogleDrive.Data.FileList fileList)
    {
        for (int i = 0; i < editPageVideoFileList.transform.childCount; i++)
        {
            GameObject go = editPageVideoFileList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        for (int i = 0; i < editPageViedoUpdateList.transform.childCount; i++)
        {
            GameObject go = editPageViedoUpdateList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        VideoPlayer.GetComponent<VideoPresenter>().Thumbnails.Clear();
        videoIDs.videoID.Clear();
        correctVideoFormatIDs.videoID.Clear();
        foreach (var file in fileList.Files)
        {
            videoIDs.videoID.Add(file.Id);
            ConfirmVideoFormat(file);
        }
        UpdateAlertVideo_txt.text = "重新整理完成";
        Invoke("ResetText", 2);
    }

    private void GetResetVideo360(UnityGoogleDrive.Data.FileList fileList)
    {
        for (int i = 0; i < editPageVideo360FileList.transform.childCount; i++)
        {
            GameObject go = editPageVideo360FileList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        for (int i = 0; i < editPageViedo360UpdateList.transform.childCount; i++)
        {
            GameObject go = editPageViedo360UpdateList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        Video360Player.GetComponent<VideoPresenter>().Thumbnails.Clear();
        video360IDs.video360ID.Clear();
        correctVideo360FormatIDs.video360ID.Clear();
        foreach (var file in fileList.Files)
        {
            video360IDs.video360ID.Add(file.Id);
            ConfirmVideo360Format(file);
        }
        UpdateAlertVideo360_txt.text = "重新整理完成";
        Invoke("ResetText", 2);
    }

    private void GetResetImage(UnityGoogleDrive.Data.FileList fileList)
    {
        for (int i = 0; i < editPageImageFileList.transform.childCount; i++)
        {
            GameObject go = editPageImageFileList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        for (int i = 0; i < editPageImageUpdateList.transform.childCount; i++)
        {
            GameObject go = editPageImageUpdateList.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        imageIDs.imageID.Clear();
        correctImageFormatIDs.imageID.Clear();
        nowDownloadImage = 0;
        foreach (var file in fileList.Files)
        {
            imageIDs.imageID.Add(file.Id);
            ConfirmImageFormat(file);
        }
        UpdateAlertImage_txt.text = "重新整理完成";
        Invoke("ResetText", 2);
    }

    private void UpdateInfo()
    {
        AuthController.CancelAuth();

        request = GoogleDriveAbout.Get();
        request.Fields = new List<string> { "user", "storageQuota" };
        request.Send();
    }

    private IEnumerator TryUpdateVideoID()
    {
        string username = UserAccount.nowUsername;
        string rdriveID = JsonUtility.ToJson(updateVideoIDs);
        UnityWebRequest request = UnityWebRequest.Get($"{updateVideoID}?rUsername={username}&rGoogledriveVideoID={rdriveID}");
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
                UpdateAlertVideo_txt.text = "上傳成功";
                Invoke("ResetText", 2);
            }
        }
        else
        {
            Debug.Log("上傳失敗");
            UpdateAlertVideo_txt.text = "上傳失敗";
            Invoke("ResetText", 2);
        }


        yield return null;
    }

    private IEnumerator TryUpdateVideo360ID()
    {
        string username = UserAccount.nowUsername;
        string rdriveID = JsonUtility.ToJson(updateVideo360IDs);
        UnityWebRequest request = UnityWebRequest.Get($"{updateVideo360ID}?rUsername={username}&rGoogledrive360VideoID={rdriveID}");
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
                UpdateAlertVideo360_txt.text = "上傳成功";
                Invoke("ResetText", 2);
            }
        }
        else
        {
            Debug.Log("上傳失敗");
            UpdateAlertVideo360_txt.text = "上傳失敗";
            Invoke("ResetText", 2);
        }


        yield return null;
    }

    private IEnumerator TryUpdateImageID()
    {
        string username = UserAccount.nowUsername;
        string rdriveID = JsonUtility.ToJson(updateImageIDs);
        UnityWebRequest request = UnityWebRequest.Get($"{updateImageID}?rUsername={username}&rGoogledriveImageID={rdriveID}");
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
                UpdateAlertImage_txt.text = "上傳成功";
                Invoke("ResetText",2);
            }
        }
        else
        {
            Debug.Log("上傳失敗");
            UpdateAlertImage_txt.text = "上傳失敗";
            Invoke("ResetText", 2);
        }


        yield return null;
    }

    private string SplitFolderURL(string url)
    {
        string[] urlArray = url.Split('/','?');
        return urlArray[urlArray.Length - 2];
    }

    private void ConfirmVideoFormat(UnityGoogleDrive.Data.File file)
    {
        string videoName = file.Name;
        string videoFormet;
        int nameL = file.Name.Length;
        if (nameL > 4)
        {
            videoFormet = videoName.Substring(videoName.Length - 4);
        }
        else if (nameL > 5)
        {
            videoFormet = videoName.Substring(videoName.Length - 5);
        }
        else if (nameL > 6)
        {
            videoFormet = videoName.Substring(videoName.Length - 6);
        }
        else if (nameL > 7)
        {
            videoFormet = videoName.Substring(videoName.Length - 7);
        }
        else videoFormet = "";
        if (videoFormet == ".asf"|| videoFormet == ".avi" || videoFormet == ".mv4" || videoFormet == ".mkv" || videoFormet == ".mp4" || videoFormet == ".mpg" || videoFormet == ".mpeg" || videoFormet == ".ogv" || videoFormet == ".vp8" || videoFormet == ".webm" || videoFormet == ".wmv")
        {
            correctVideoFormatIDs.videoID.Add(file.Id);
            GameObject FileObject = Instantiate(editPageFileObject, editPageVideoFileList.transform);
            FileObject.name = file.Id;
            FileObject.transform.GetChild(0).transform.GetComponent<Text>().text = file.Name;
            FileObject.GetComponent<Button>().onClick.AddListener(() => VideoPlayer.GetComponent<VideoController>().PrepareForUrl(string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", file.Id)));
            FileObject.GetComponent<Button>().onClick.AddListener(() => VideoPlayer.GetComponent<VideoPresenter>().LoadingIndicator.gameObject.SetActive(true));
            VideoPlayer.GetComponent<VideoPresenter>().Thumbnails.Add(FileObject.transform);
        }
    }

    private void ConfirmVideo360Format(UnityGoogleDrive.Data.File file)
    {
        string video360Name = file.Name;
        string video360Formet;
        int nameL = file.Name.Length;
        if (nameL > 4)
        {
            video360Formet = video360Name.Substring(video360Name.Length - 4);
        }
        else if (nameL > 5)
        {
            video360Formet = video360Name.Substring(video360Name.Length - 5);
        }
        else if (nameL > 6)
        {
            video360Formet = video360Name.Substring(video360Name.Length - 6);
        }
        else if (nameL > 7)
        {
            video360Formet = video360Name.Substring(video360Name.Length - 7);
        }
        else video360Formet = "";
        if (video360Formet == ".asf" || video360Formet == ".avi" || video360Formet == ".mv4" || video360Formet == ".mkv" || video360Formet == ".mp4" || video360Formet == ".mpg" || video360Formet == ".mpeg" || video360Formet == ".ogv" || video360Formet == ".vp8" || video360Formet == ".webm" || video360Formet == ".wmv")
        {
            correctVideo360FormatIDs.video360ID.Add(file.Id);
            GameObject FileObject = Instantiate(editPageFileObject, editPageVideo360FileList.transform);
            FileObject.name = file.Id;
            FileObject.transform.GetChild(0).transform.GetComponent<Text>().text = file.Name;
            FileObject.GetComponent<Button>().onClick.AddListener(() => Video360Player.GetComponent<VideoController>().PrepareForUrl(string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", file.Id)));
            FileObject.GetComponent<Button>().onClick.AddListener(() => Video360Player.GetComponent<VideoPresenter>().LoadingIndicator.gameObject.SetActive(true));
            Video360Player.GetComponent<VideoPresenter>().Thumbnails.Add(FileObject.transform);
        }
    }

    private void ConfirmImageFormat(UnityGoogleDrive.Data.File file)
    {
        string imageName = file.Name;
        string imageFormet;
        int nameL = file.Name.Length;
        if (nameL > 4)
        {
            imageFormet = imageName.Substring(imageName.Length - 4);
        }
        else if (nameL > 5)
        {
            imageFormet = imageName.Substring(imageName.Length - 5);
        }
        else imageFormet = "";
        if (imageFormet == ".bmp" || imageFormet == ".jpeg" || imageFormet == ".jpg" || imageFormet == ".png" || imageFormet == ".webp" || imageFormet == ".svg")
        {
            correctImageFormatIDs.imageID.Add(file.Id);
            GameObject FileObject = Instantiate(editPageFileObject, editPageImageFileList.transform);
            FileObject.name = file.Id;
            FileObject.transform.GetChild(0).transform.GetComponent<Text>().text = file.Name;
            StartCoroutine(DownLoadImage(file.Id, nowDownloadImage, FileObject));        
            nowDownloadImage++;
        }
    }

    private void LoadImg(int nowDownloadNum)
    {
        Texture2D texture = downloadImageTextureDic[nowDownloadNum];
        ImageDisplay.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        ImageDisplay.GetComponent<Image>().preserveAspect = true;
    }

    //下載圖片
    private IEnumerator DownLoadImage(string id, int nowDownloadNum, GameObject fileObject)
    {
        UnityWebRequest uwr = new UnityWebRequest(string.Format("https://docs.google.com/uc?export=download&confirm=t&id={0}", id), UnityWebRequest.kHttpVerbGET);
        uwr.downloadHandler = new DownloadHandlerTexture();
        yield return uwr.SendWebRequest();
        Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
        downloadImageTextureDic.Add(nowDownloadNum, texture);
        fileObject.GetComponent<Button>().onClick.AddListener(() => LoadImg(nowDownloadNum));
        //ttt[nowDownloadNum] = downloadImageTextureDic[nowDownloadNum];
        print("---"+nowDownloadNum);
    }

    public void ResetText()
    {
        UpdateAlertVideo_txt.text = "";
        UpdateAlertVideo360_txt.text = "";
        UpdateAlertImage_txt.text = "";
        Alert_txt.text = "";
    }

    public void EditBack()
    {
        MainPage.gameObject.SetActive(true);
        VideoPlayer.GetComponent<VideoPlayer>().Stop();
        VideoPlayer.transform.GetChild(0).gameObject.SetActive(false);
        VideoPlayer.transform.GetChild(1).gameObject.SetActive(false);
        Video360Player.GetComponent<VideoPlayer>().Stop();
        Video360Player.transform.GetChild(0).gameObject.SetActive(false);
        Video360Player.transform.GetChild(1).gameObject.SetActive(false);
        ImageDisplay.sprite = null;
        editPageVideoPannel.SetActive(false);
        editPageVideo360Pannel.SetActive(false);
        editPageImagePannel.SetActive(false);
        EditPage.gameObject.SetActive(false);
    }
}
