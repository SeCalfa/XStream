using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Requests : MonoBehaviour
{
    [SerializeField]
    private InputField outputArea;

    [Header("GetDisplayName")]
    [SerializeField]
    private Button getDisplayNameButton;

    public Authentication Authentication { get; set; }

    private void Awake()
    {
        getDisplayNameButton.onClick.AddListener(() => StartCoroutine(GetDisplayName()));

        RequestButtonsDisable();
    }

    private IEnumerator GetDisplayName()
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{Constants.GetDisplayName}/{Authentication.UserProfileInfoModel.serverId}"))
        {
            request.SetRequestHeader("Authorization", "Bearer " + Authentication.TokenModel.token);

            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
                outputArea.text = request.error;
            else
                outputArea.text = request.downloadHandler.text;
        }
    }

    public void RequestButtonsEnable()
    {
        getDisplayNameButton.interactable = true;
    }

    public void RequestButtonsDisable()
    {
        getDisplayNameButton.interactable = false;
    }

    public void ClearTextBox()
    {
        outputArea.text = string.Empty;
    }
}
