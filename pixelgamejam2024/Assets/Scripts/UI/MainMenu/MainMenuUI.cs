using TheraBytes.BetterUi;
using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Managers")] 
    [SerializeField] private MultiplayerManager _multiplayer;
    [SerializeField] private LobbyUI _lobby;
    
    [Header("UI")]
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private BetterButton _joinButton;
    [SerializeField] private BetterButton _hostButton;

    private void Awake()
    {
        ShowMenu();
        _joinButton.interactable = false;
        _hostButton.interactable = true;
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
    }

    private void HideMenu()
    {
        gameObject.SetActive(false);
        _lobby.gameObject.SetActive(true);
    }

    public void InputFieldUpdated()
    {
        _joinButton.interactable = !string.IsNullOrEmpty(_inputField.text);
    }

    public void ButtonJoin()
    {
        LoadingManager.Instance.Wait(true);
        _multiplayer.SetLobbyCodeAndStart(_inputField.text);
        HideMenu();
    }

    public void ButtonHost()
    {
        LoadingManager.Instance.Wait(true);
        _multiplayer.HostNewLobby();
        HideMenu();
    }
}
