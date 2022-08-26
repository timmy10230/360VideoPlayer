using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpdateList : MonoBehaviour, IDropHandler
{
    [SerializeField] private GameObject UpdatelistPanel;
    [SerializeField] private RectTransform rt;
    [SerializeField] private int nowFileCount;
    //private bool AdjustTransform = false;

    private void Start()
    {
        rt = UpdatelistPanel.GetComponent<RectTransform>();
        nowFileCount = UpdatelistPanel.transform.childCount;
        SetListScale();
    }

    private void Update()
    {
        if(nowFileCount!= UpdatelistPanel.transform.childCount)
        {
            SetListScale();
        }
    }

    public void SetListScale()
    {
        nowFileCount = UpdatelistPanel.transform.childCount;
        if (nowFileCount > 8)
        {
            rt.anchoredPosition = new Vector2(0, -290 - (35 * (UpdatelistPanel.transform.childCount - 8)));
            //UpdatelistPanel.transform.position = new Vector3(0, -290 - (35 * (UpdatelistPanel.transform.childCount - 8)), 0);
            rt.sizeDelta = new Vector2(380, 580 + (70 * (UpdatelistPanel.transform.childCount - 8)));
        }
        else if (nowFileCount <= 8)
        {
            rt.anchoredPosition = new Vector2(0, -290);
            rt.sizeDelta = new Vector2(380, 580);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("OnDrop");
        if (eventData.pointerDrag.transform.tag == "FolderFile")
        {
            //eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            eventData.pointerDrag.transform.tag = "UpdateFile";
            Destroy(eventData.pointerDrag.gameObject.GetComponent<FolderListFile>());
            Destroy(eventData.pointerDrag.gameObject.GetComponent<CanvasGroup>());
            eventData.pointerDrag.gameObject.AddComponent<DragController>();
            eventData.pointerDrag.gameObject.GetComponent<DragController>().currentTransform = eventData.pointerDrag.GetComponent<RectTransform>();
            eventData.pointerDrag.gameObject.GetComponent<Image>().maskable = true;
            eventData.pointerDrag.gameObject.transform.parent = UpdatelistPanel.transform;
        }
    }
}