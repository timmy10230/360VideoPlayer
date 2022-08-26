using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolderFileList : MonoBehaviour
{
    [SerializeField] private GameObject FolderListPanel;
    [SerializeField] private RectTransform rt;
    [SerializeField] private int nowFileCount;
    //private bool AdjustTransform = false;

    private void Start()
    {
        rt = FolderListPanel.GetComponent<RectTransform>();
        nowFileCount = FolderListPanel.transform.childCount;
        SetListScale();
    }

    private void Update()
    {
        if (nowFileCount != FolderListPanel.transform.childCount)
        {
            SetListScale();
        }
    }

    public void SetListScale()
    {
        nowFileCount = FolderListPanel.transform.childCount;
        if (nowFileCount > 8)
        {
            rt.anchoredPosition = new Vector2(0, -290 - (35 * (FolderListPanel.transform.childCount - 8)));
            rt.sizeDelta = new Vector2(380, 580 + (70 * (FolderListPanel.transform.childCount - 8)));
        }
        else if (nowFileCount <= 8)
        {
            rt.anchoredPosition = new Vector2(0, -290);
            rt.sizeDelta = new Vector2(380, 580);
        }
    }
}
