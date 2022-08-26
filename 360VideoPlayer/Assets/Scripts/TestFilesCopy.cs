﻿using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class TestFilesCopy : AdaptiveWindowGUI
{
    private GoogleDriveFiles.CopyRequest request;
    private string result;
    private string fileId = string.Empty;
    private string copyName = string.Empty;

    protected override void OnWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label($"Loading: {request.Progress:P2}");
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Source File ID:", GUILayout.Width(90));
            fileId = GUILayout.TextField(fileId);
            GUILayout.Label("Copy Name:", GUILayout.Width(80));
            copyName = GUILayout.TextField(copyName);
            if (GUILayout.Button("Copy", GUILayout.Width(100))) CopyFile();
            GUILayout.EndHorizontal();
        }

        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.Label(result);
        }
    }

    private void CopyFile ()
    {
        var file = new UnityGoogleDrive.Data.File() { Id = fileId, Name = string.IsNullOrEmpty(copyName) ? null : copyName };
        request = GoogleDriveFiles.Copy(file);
        request.Fields = new List<string> { "name, size, createdTime" };
        request.Send().OnDone += BuildResultString;
    }

    private void BuildResultString (UnityGoogleDrive.Data.File file)
    {
        result = string.Format("Copied File Name: {0} Size: {1:0.00}MB Created: {2:dd.MM.yyyy HH:MM:ss}",
                file.Name,
                file.Size * .000001f,
                file.CreatedTime);
    }
}
