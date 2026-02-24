using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VitalRouter;
using Cysharp.Threading.Tasks;
using TMPro;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

using xpTURN.Text;

/// <summary>
/// Chat View - Displays chat UI and handles user input
/// Receives network events through VitalRouter
/// </summary>
[Routes]
public partial class ChatView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField _messageInput = null!;
    [SerializeField] private Button _sendButton = null!;
    [SerializeField] private Button _connectButton = null!;
    [SerializeField] private TextMeshProUGUI _statusText = null!;
    [SerializeField] private TextMeshProUGUI _chatLogText = null!;
    [SerializeField] private ScrollRect _scrollRect = null!;

    private ChatViewModel _viewModel = null!;
    private ICommandSubscribable _subscribable = null!;
    private ILogger _logger = null!;

    [Inject]
    public void Construct(ChatViewModel viewModel, ICommandSubscribable subscribable, ILogger logger)
    {
        _viewModel = viewModel;
        _subscribable = subscribable;
        _logger = logger;

        // Register event handlers
        this.MapTo(_subscribable);
    }

    private void Start()
    {
        SetupUI();
        UpdateConnectionUI();
    }

    private void SetupUI()
    {
        _sendButton.onClick.AddListener(OnSendClicked);
        _connectButton.onClick.AddListener(OnConnectClicked);
        _messageInput.onSubmit.AddListener(OnMessageSubmit);

        _chatLogText.text = "";
        _messageInput.interactable = false;
        _sendButton.interactable = false;
    }

    private void OnSendClicked()
    {
        SendMessage();
    }

    private void OnMessageSubmit(string _)
    {
        SendMessage();
    }

    private void SendMessage()
    {
        var message = _messageInput.text.Trim();
        if (string.IsNullOrEmpty(message))
            return;

        _messageInput.text = "";
        _messageInput.ActivateInputField();

        _viewModel.SendMessageAsync(message).Forget();
    }

    private void OnConnectClicked()
    {
        if (_viewModel.IsConnected)
        {
            _viewModel.DisconnectAsync().Forget();
        }
        else
        {
            _viewModel.ConnectAsync().Forget();
        }
    }

    /// <summary>
    /// Handles network state change events
    /// </summary>
    [Route]
    UniTask On(NetworkStateChangedEvent evt)
    {
        _logger.XLogInformation($"Network state changed: {evt.State} - {evt.Message}");

        UpdateConnectionUI();
        AppendSystemMessage($"[System] {evt.Message}");

        return UniTask.CompletedTask;
    }

    /// <summary>
    /// Handles incoming chat messages
    /// </summary>
    [Route]
    UniTask On(ChatMessageReceivedEvent evt)
    {
        _logger.XLogInformation($"Message from {evt.SenderName}: {evt.Message}");

        AppendChatMessage(evt.SenderName, evt.Message);

        return UniTask.CompletedTask;
    }

    /// <summary>
    /// Handles message sent confirmation
    /// </summary>
    [Route]
    UniTask On(ChatMessageSentEvent evt)
    {
        if (!evt.Success)
        {
            _logger.XLogWarning($"Failed to send message: {evt.MessageId}");
            AppendSystemMessage("[System] Failed to send message");
        }

        return UniTask.CompletedTask;
    }

    private void UpdateConnectionUI()
    {
        var isConnected = _viewModel.IsConnected;

        _messageInput.interactable = isConnected;
        _sendButton.interactable = isConnected;

        var connectButtonText = _connectButton.GetComponentInChildren<TextMeshProUGUI>();
        if (connectButtonText != null)
        {
            connectButtonText.text = isConnected ? "Disconnect" : "Connect";
        }

        _statusText.text = _viewModel.ConnectionState switch
        {
            NetworkState.Connected => "<color=green>Connected</color>",
            NetworkState.Connecting => "<color=yellow>Connecting...</color>",
            NetworkState.Reconnecting => "<color=yellow>Reconnecting...</color>",
            NetworkState.Error => "<color=red>Error</color>",
            _ => "<color=#808080>Disconnected</color>"
        };
    }

    private void AppendChatMessage(string sender, string message)
    {
        var timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        _chatLogText.text += XString.Format($"<color=#888888>[{timestamp}]</color> <b>{sender}:</b> {message}\n");
        ScrollToBottom();
    }

    private void AppendSystemMessage(string message)
    {
        var timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        _chatLogText.text += XString.Format($"<color=#888888>[{timestamp}]</color> <color=#AAAAAA><i>{message}</i></color>\n");
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        if (_scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void OnDestroy()
    {
        _viewModel?.Dispose();
    }
}