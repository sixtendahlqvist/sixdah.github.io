using System;
using System.Threading;
using AuroraPunks.Services;
using AuroraPunks.AuroraSteam;
using AuroraPunks.AuroraUtilities;
using AuroraPunks.ScriptableValues;
using Cysharp.Threading.Tasks;
using Hertzole.CecilAttributes;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirage;
using System.Net;
using System.Net.Sockets;


namespace BlockEm
{
    [DefaultExecutionOrder(EXECUTION_ORDER)]
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        [Scene]
        private string gameScene = default;

        [Header("References")]
        [SerializeField]
        private GameSettings gameSettings = default;
        [SerializeField]
        private PlayerInput mainMenuInput = default;
        [SerializeField]
        private ReactiveBool canJoinMidGameBool = default;
        [SerializeField]
        private ReactiveBool hasInternet = default;
        [SerializeField]
        private ReactiveLobbyCreateStatus lobbyCreateStatus = default;

        [Header("Events")]
        [SerializeField]
        private RuntimeBoolEvent introSkipped = default;
        [SerializeField]
        private LobbyTypeEvent clickedHostButtonEvent = default;
        [SerializeField]
        private LobbyTypeEvent clickedCompetetiveButtonEvent = default;
        [SerializeField]
        private StartLocalEvent onStartLocal = default;
        [SerializeField]
        private ClickLobbyEvent onClickedLobby = default;
        [SerializeField]
        private JoinLobbyEvent onJoinLobby = default;
        [SerializeField]
        private RuntimeEvent onCreateServerFailed = default;
        [SerializeField]
        private RuntimeEvent onClientStartSceneSwitch = default;
        [SerializeField]
        private LobbyErrorEvent onCreateLobbyError = default;
        [SerializeField]
        private LobbyErrorEvent onJoinLobbyError = default;

        [ResetStatic]
        private static bool firstBoot;

        private bool isHosting = false;
        private bool isTransitioningToNextScene = false;
        private bool clickedLobby = false;

        private readonly InputAction anyAction = new InputAction(binding: "/*/<button>");

        private readonly ReusableCancellationToken cancelToken = new ReusableCancellationToken();

        public const int EXECUTION_ORDER = -10000;

        private void Start()
        {
            if (!firstBoot)
            {
                firstBoot = true;
                anyAction.Enable();
            }
            else
            {
                // True means the intro was skipped.
                introSkipped.InvokePooled(this, () => true);
            }

            isHosting = false;
            canJoinMidGameBool.Value = true;

            // We would normally use TimeManager, but we can't because it depends on network stuff, which we don't have in the main menu.
            Time.timeScale = 1f;
            gameSettings.IsLevelEditor = false;
            gameSettings.IsLocal = false;

            PlayerNetworkInputHandler.SetActivePlayerInput(mainMenuInput);
            PlayerNetworkInputHandler.SetActionMap(mainMenuInput, ActionMap.UI);
        }

        private void OnEnable()
        {
            anyAction.started += InputAnyAction;

            clickedHostButtonEvent.OnInvoked += OnClickedHostButton;
            //clickedCompetetiveButtonEvent.OnInvoked += OnClickedCompetitiveButton;
            onStartLocal.OnInvoked += OnStartLocal;
            onClickedLobby.OnInvoked += OnClickedLobby;
            onClientStartSceneSwitch.OnInvoked += OnClientStartSceneSwitch;

            LobbyService.OnEnteredLobby += OnEnteredLobby;
        }

        private void OnDisable()
        {
            anyAction.started -= InputAnyAction;
            anyAction.Disable();

            clickedHostButtonEvent.OnInvoked -= OnClickedHostButton;
            //clickedCompetetiveButtonEvent.OnInvoked -= OnClickedCompetitiveButton;

            onStartLocal.OnInvoked -= OnStartLocal;
            onClickedLobby.OnInvoked -= OnClickedLobby;
            onClientStartSceneSwitch.OnInvoked -= OnClientStartSceneSwitch;

            LobbyService.OnEnteredLobby -= OnEnteredLobby;
        }

        private void OnDestroy()
        {
            cancelToken.Cancel();
            cancelToken.Dispose();
        }

        private void InputAnyAction(InputAction.CallbackContext obj)
        {
            // False means the intro was not skipped.

            if (TitleAnimator.completedAnimation == false)
                return;

            introSkipped.InvokePooled(this, () => false);
            anyAction.Disable();
        }

        private void OnClickedHostButton(object sender, LobbyJoinEventArgs e)
        {
            CreateOnlineGameAsyncVoid(e.LobbyType).Forget();
        }

        private void OnStartLocal(object sender, LocalLobbyEventArgs e)
        {
            clickedLobby = true;
            CreateLocalGameAsyncVoid().Forget();
        }

        private void OnClickedLobby(object sender, GenericEventArgs<IServiceLobby> e)
        {
            clickedLobby = true;
            JoinLobbyAsyncVoid(e.Value).Forget();
        }

        private void OnClientStartSceneSwitch(object sender, EventArgs e)
        {
            // If we're not hosting, we're joining a game.
            // The host handles the transition itself.
            if (!isHosting)
            {
                TransitionManager.TransitionIn(BlockEmNetworkSceneManager.AllowSceneChange);
            }
        }

        private void OnEnteredLobby(EnterLobbyEventArgs obj)
        {
            // We don't want to show this message if we initiated the lobby join in game.
            // Only show it if we've been invited.
            if (clickedLobby)
            {
                return;
            }

            onJoinLobby.Invoke(this, LobbyJoinResult.Success);
        }

        private async UniTaskVoid CreateLocalGameAsyncVoid()
        {
            gameSettings.IsLocal = true;
            isHosting = true;
            canJoinMidGameBool.Value = false;

            await TransitionManager.TransitionInAsync(cancelToken.Token);

            TryCreateServerAsyncVoid(false, this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid CreateOnlineGameAsyncVoid(Steam.LobbyType lobbyType)
        {
            lobbyCreateStatus.Value = LobbyCreateStatus.None;

            if (!hasInternet.Value)
            {
                onCreateLobbyError.InvokePooled(this, () => LobbyError.NoInternet);
                lobbyCreateStatus.Value = LobbyCreateStatus.Failed;
                return;
            }

            if (!Steam.SteamManager.IsInitialized)
            {
                onCreateLobbyError.InvokePooled(this, () => LobbyError.InvalidSteam);
                lobbyCreateStatus.Value = LobbyCreateStatus.Failed;
                return;
            }

            gameSettings.IsLocal = false;
            isHosting = true;

            lobbyCreateStatus.Value = LobbyCreateStatus.Creating;

            try
            {
                var createResult = await LobbyService.CreateLobbyAsync(lobbyType, gameSettings.MaxPlayers);
                clickedLobby = false;
                if (createResult.createdResult != Steam.LobbyCreatedResult.Ok)
                {
                    DebugLogger.LogError($"Error when creating lobby: {createResult.createdResult}");
                    onCreateLobbyError.InvokePooled(this, () =>
                    {
                        switch (createResult.createdResult)
                        {
                            case Steam.LobbyCreatedResult.Fail:
                            case Steam.LobbyCreatedResult.Timeout:
                            case Steam.LobbyCreatedResult.LimitExceeded:
                            case Steam.LobbyCreatedResult.AccessDenied:
                            return LobbyError.InvalidSteam;
                            case Steam.LobbyCreatedResult.NoConnection:
                            return LobbyError.NoInternet;
                            default:
                            return LobbyError.InvalidSteam;
                        }
                    });
                    lobbyCreateStatus.Value = LobbyCreateStatus.Failed;
                    return;
                }

                Console.WriteLine(createResult.lobby.LobbyID.ID.ToString());
                createResult.lobby.SetData("name", UserService.Name);
                createResult.lobby.SetData("ip", UserService.UserConnectionIP);

                lobbyCreateStatus.Value = LobbyCreateStatus.Created;

                isTransitioningToNextScene = true;
                TryCreateServerAsyncVoid(true, this.GetCancellationTokenOnDestroy()).Forget();
                await TransitionManager.TransitionInAsync(cancelToken.Token);
                isTransitioningToNextScene = false;
            }
            catch (Exception e)
            {
                onCreateServerFailed.Invoke(this);
                Debug.LogError($"Failed to create lobby: {e}");
            }
        }

        private async UniTaskVoid JoinLobbyAsyncVoid(IServiceLobby lobby)
        {
            if (!hasInternet.Value)
            {
                onJoinLobbyError.InvokePooled(this, () => LobbyError.NoInternet);
                return;
            }

            if (!Steam.SteamManager.IsInitialized)
            {
                onCreateLobbyError.InvokePooled(this, () => LobbyError.InvalidSteam);
                return;
            }

            var result = await LobbyService.JoinLobbyAsync(lobby.LobbyID);

            clickedLobby = false;

            if (result != LobbyJoinResult.Success)
            {
                DebugLogger.LogError("Error when joining lobby.");
                onJoinLobbyError.Invoke(this, LobbyError.InvalidSteam);
                return;
            }

            onJoinLobby.Invoke(this, result);
        }

        private async UniTaskVoid TryCreateServerAsyncVoid(bool isListening, CancellationToken cancellationToken)
        {
            BlockEmNetworkManager nm = BlockEmNetworkManager.Instance;
            nm.Server.Listening = isListening;

            try
            {
                nm.Server.StartServer(nm.Client);

                while (isTransitioningToNextScene)
                {
                    await UniTask.Yield(cancellationToken);
                }

                String ip = GetLocalIPAddress();
                Debug.Log(ip);


                nm.NetworkSceneManager.ServerLoadSceneNormal(gameScene);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                onCreateServerFailed.Invoke(this);
            }
        }
        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }

    }

}
