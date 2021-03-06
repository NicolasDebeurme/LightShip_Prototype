using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using static Enums;
using System;

public class LobbyView : View
{
    //Private
    [SerializeField]
    private TextMeshProUGUI[] playersUI;

    [SerializeField]
    private GameObject inLobby;
    [SerializeField]
    private GameObject outLobby;

    //Public
    public TMP_Dropdown _dropDownBtn;

    public Button _startButton;

    [HideInInspector]
    public Roles playerRole = Roles.None;

    public InputField SessionIDField;

    //Events
    public event OnLobbyButtonPressedDelegate LobbyButtonPressed;
    public delegate void OnLobbyButtonPressedDelegate(LobbyButton buttonType);

    public event StartButtonDelegate StartButtonTriggered;
    public delegate void StartButtonDelegate();

    public event PlayerRoleChangeDelegate PlayerRoleChange;
    public delegate void PlayerRoleChangeDelegate(Roles playerRole);


    public override void Initialize()
    {
        List <TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        foreach (string role in Enum.GetNames(typeof(Roles)))
        {
            newOptions.Add(new TMP_Dropdown.OptionData(role));
        }
            
        _dropDownBtn.AddOptions(newOptions);
    }

    #region Public methods
    public void OnStart()
    {
        StartButtonTriggered?.Invoke();
    }

    public void OnDropDownButtonDown(int role)
    {
        playerRole = (Roles)role;

        PlayerRoleChange?.Invoke(playerRole);
    }

    public void OnLobbyButton(int buttonType)
    {
        LobbyButtonPressed?.Invoke((LobbyButton)buttonType);

    }
    public void UpdateUI(Dictionary<Guid, Roles> players)
    {
        int count = 0;
        if(players != null)
        {
            foreach (var player in players)
            {
                if(player.Key == NetworkingManager._self.Identifier)
                    playersUI[count].text = "You are " + player.Value.ToString();
                else
                    playersUI[count].text = player.Key + " is " + player.Value.ToString();

                playersUI[count].transform.parent.GetComponent<Image>().color = new Color32(0, 173, 0, 183);
                count++;
            }
        }

        for(int i= count; i<playersUI.Length; i++)
        {
            playersUI[i].text = "Waiting for player ...";
            playersUI[i].transform.parent.GetComponent<Image>().color = new Color32(173, 0, 0, 183);
        }
    }

    public void ChangeLobbyState()
    {
        if (inLobby.activeSelf)
        {
            inLobby.SetActive(false);
            outLobby.SetActive(true);
        }
        else
        {
            outLobby.SetActive(false);
            inLobby.SetActive(true);
        }
    }
    #endregion

    #region Private methods

    #endregion
}

