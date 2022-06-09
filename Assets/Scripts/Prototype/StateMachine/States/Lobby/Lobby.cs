using Niantic.ARDK;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Configuration;
using Niantic.ARDK.AR.Networking;
using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.MultipeerNetworkingEventArgs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Enums;

public class Lobby : State
{
    private LobbyView _view =null;

    public Lobby(GameStateSystem gameStateSystem) : base(gameStateSystem)
    {
    }

    public override IEnumerator Start()
    {
        _view =UIManager.Show<LobbyView>();

        _view.LobbyButtonPressed += OnLobbyButtonPressed;

        yield break ;
    }

    public override void NextState()
    {
        GameStateSystem._playerRole = _view.playerRole;
        GameStateSystem.SetState(new GoToPlace(GameStateSystem));
    }

    private void CreateAndRunSharedAR()
    {
        var _arNetworking = ARNetworkingFactory.Create(GameManager._instance.runtimeEnv) ;
        var _networking = _arNetworking.Networking;

        var _session = _arNetworking.ARSession;


        var _sessionConfigData = ARWorldTrackingConfigurationFactory.Create();
        _sessionConfigData.WorldAlignment = WorldAlignment.Gravity;
        _sessionConfigData.PlaneDetection = PlaneDetection.Horizontal;

        _sessionConfigData.IsAutoFocusEnabled = false;
        _sessionConfigData.IsDepthEnabled = false;
        _sessionConfigData.IsLightEstimationEnabled = false;
        _sessionConfigData.IsSharedExperienceEnabled = true;

        GameStateSystem._gameInfo = new GameInfo(_arNetworking,_networking,_session,_sessionConfigData);


        var sessionID = _view.SessionIDField.text;

        var sessionIdAsBytes = Encoding.UTF8.GetBytes(sessionID);

        _networking.Join(sessionIdAsBytes);

        GameManager._instance.GameInitialized(GameStateSystem._gameInfo);

        GameManager._instance.OnGameInitialized += WaitAndUpdateLobby;
    }


    private void OnLobbyButtonPressed(LobbyButton buttonType)
    {
        switch(buttonType)
        {
            case (LobbyButton.Join):
                if (GameStateSystem._gameInfo._session == null)
                    CreateAndRunSharedAR();
                break;

            case (LobbyButton.Create):
                if (GameStateSystem._gameInfo._session == null)
                    CreateAndRunSharedAR();
                break;

            case(LobbyButton.Leave):
                GameManager._instance.StopSharedAR();
                break;

            default:
                throw new Exception("No such buttonType");

        }
    }

    private void WaitAndUpdateLobby(GameInfo gameInfo)
    {

    }
}
