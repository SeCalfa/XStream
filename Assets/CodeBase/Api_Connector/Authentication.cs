using System.Collections;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Authentication : MonoBehaviour
{
    [SerializeField]
    private Requests requests;
    [Space]
    [SerializeField]
    private TMP_InputField loginField;
    [SerializeField]
    private TMP_InputField passwordField;
    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private Button logoutButton;

    public TokenModel TokenModel { get; private set; }
    public UserProfileInfoModel UserProfileInfoModel { get; private set; }

    private void Awake()
    {
        requests.Authentication = this;

        loginButton.onClick.AddListener(Login);
        logoutButton.onClick.AddListener(Logout);
    }

    private void Login()
    {
        LoginModel model = new LoginModel(loginField.text, passwordField.text);

        StartCoroutine(PostAuthentication(Constants.Login, JsonUtility.ToJson(model)));
    }

    private void Logout()
    {
        TokenModel = null;
        UserProfileInfoModel = null;

        loginField.text = "";
        passwordField.text = "";
        requests.ClearTextBox();
        loginButton.interactable = true;
        logoutButton.interactable = false;

        requests.RequestButtonsDisable();
        Debug.Log("You logged out. Current token cleared");
    }

    IEnumerator PostAuthentication(string url, string jsonData)
    {
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            TokenModel = JsonUtility.FromJson<TokenModel>(www.downloadHandler.text);

            if(TokenModel.token == null)
            {
                Debug.LogWarning("Invalid login or password");
            }
            else
            {
                Debug.Log("You logged in");
                Debug.Log($"Current token: {TokenModel.token}");

                StartCoroutine(GetUserProfileInfo());

                loginButton.interactable = false;
                logoutButton.interactable = true;

                requests.RequestButtonsEnable();
            }
        }

        www.Dispose();
    }

    private IEnumerator GetUserProfileInfo()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(Constants.UserProfileInfo))
        {
            request.SetRequestHeader("Authorization", "Bearer " + TokenModel.token);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                UserProfileInfoModel = JsonUtility.FromJson<UserProfileInfoModel>(request.downloadHandler.text);

                if(UserProfileInfoModel.userName == null)
                {
                    Debug.LogWarning("User profile info not filled");
                }
                else
                {
                    Debug.Log("User profile info filled");
                }
            }
        }
    }
}
