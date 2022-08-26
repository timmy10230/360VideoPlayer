using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    [SerializeField] private string authenticationEndpoint = "http://127.0.0.1:4000/account";
    [SerializeField] private string register = "http://127.0.0.1:4000/register";


    [SerializeField] private Text alertText;
    [SerializeField] private Button loginButton;
    [SerializeField] private InputField nameInputFiled;
    [SerializeField] private InputField passwordInputFiled;
    [SerializeField] private Text regAlertText;
    [SerializeField] private Button registerButton;
    [SerializeField] private InputField regNameInputFiled;
    [SerializeField] private InputField regPasswordInputFiled;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject mainPanel;

    public void OnLoginClick()
    {
        alertText.text = "登入中...";
        loginButton.interactable = false;
        StartCoroutine(TryLogin());
    }

    private IEnumerator TryLogin()
    {

        string username = nameInputFiled.text;
        string password = passwordInputFiled.text;

        UnityWebRequest request = UnityWebRequest.Get($"{authenticationEndpoint}?rUsername={username}&rPassword={password}");
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;

            if(startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }
        
        if(request.result == UnityWebRequest.Result.Success)
        {
            if (request.downloadHandler.text != "Invalid credentials" && request.downloadHandler.text != "Account not exists")
            {
                alertText.text = "登入成功";
                loginButton.interactable = true;
                UserAccount returnedAccount = JsonUtility.FromJson<UserAccount>(request.downloadHandler.text);
                alertText.text = $"{returnedAccount._id} 歡迎" + returnedAccount.username;
                UserAccount.nowUsername = returnedAccount.username;
                mainPanel.SetActive(true);
                loginPanel.SetActive(false);
                //Debug.Log(request.downloadHandler.text);
            }
            else if(request.downloadHandler.text == "Invalid credentials")
            {
                alertText.text = "密碼錯誤";
                loginButton.interactable = true;
                //Debug.Log(request.downloadHandler.text);
            }
            else if(request.downloadHandler.text == "Account not exists")
            {
                alertText.text = "帳號不存在";
                loginButton.interactable = true;
            }
        }
        else
        {
            alertText.text = "無法連接到伺服器";
            loginButton.interactable = true;
            Debug.Log("Unable to connect");
        }


        yield return null;
    }

    public void OnRegisterClick()
    {
        regAlertText.text = "註冊中...";
        registerButton.interactable = false;
        StartCoroutine(TryRegister());
    }

    private IEnumerator TryRegister()
    {
        string regUsername = regNameInputFiled.text;
        string regPassword = regPasswordInputFiled.text;

        UnityWebRequest registerRequest = UnityWebRequest.Get($"{register}?rUsername={regUsername}&rPassword={regPassword}");
        var handler = registerRequest.SendWebRequest();

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

        if (registerRequest.result == UnityWebRequest.Result.Success)
        {
            if (registerRequest.downloadHandler.text != "Invalid credentials" && registerRequest.downloadHandler.text != "Account exists")
            {
                regAlertText.text = "註冊成功";
                registerButton.interactable = true;
            }
            else if(registerRequest.downloadHandler.text == "Invalid credentials")
            {
                regAlertText.text = "請輸入帳號密碼";
                registerButton.interactable = true;
                //Debug.Log(request.downloadHandler.text);
            }
            else if (registerRequest.downloadHandler.text == "Account exists")
            {
                regAlertText.text = "帳號已存在";
                registerButton.interactable = true;
                //Debug.Log(request.downloadHandler.text);
            }
        }
        else
        {
            regAlertText.text = "無法連接到伺服器";
            registerButton.interactable = true;
            Debug.Log("Unable to connect");
        }


        yield return null;
    }

    public void ResetText()
    {
        nameInputFiled.text = "";
        passwordInputFiled.text = "";
        regNameInputFiled.text = "";
        regPasswordInputFiled.text = "";
        alertText.text = "";
    }
}
