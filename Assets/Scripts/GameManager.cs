using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject cellsParent;
    [SerializeField] TextMeshProUGUI currentPlayerName;
    private List<Button> cells;

    [Space]
    [Header("Video Audio UI Prefab")]
    [SerializeField] VideoPlayerCustom videoPrefab; // UI prefabs
    [SerializeField] AudioPlayer audioPrefab;
    [Space]
    [SerializeField] ProgressBar progressBar;
    [Space]
    [SerializeField] DownloadMediaHandler downloadMediaHandler;

    //Game Play fields
    private int currentPlayer = 1;
    bool mediaPlaying = false;
    GameObject currentCell;
    int currentCellIndex;
    private Dictionary<int, string> moves = new Dictionary<int, string>();

    private int[] board = new int[9]; // For a 3x3 Tic-Tac-Toe board
    IMoveLogger moveLogger;

    private void OnEnable()
    {
        DownloadMediaHandler.OnDownloadComplete += HandleDownloadComplete;
    }

    private void OnDisable()
    {
        DownloadMediaHandler.OnDownloadComplete -= HandleDownloadComplete;
    }

    private void HandleDownloadComplete(string assetType, object obj)
    {
        // Trigger media playback when download is complete
        if (currentCell != null && currentCellIndex != -1)
        {
            if (currentPlayer == 1)
            {
                VideoClip videoClip = obj as VideoClip;
                videoPrefab.GetComponent<VideoPlayer>().clip = videoClip;
                PlayMedia(videoPrefab);
            }
            else
            {
                AudioClip audioClip = obj as AudioClip;
                audioPrefab.GetComponent<AudioSource>().clip = audioClip;
                PlayMedia(audioPrefab);
            }
        }
    }
    private void Awake() => Initilzation();
    void Initilzation()
    {
        cells = cellsParent.GetComponentsInChildren<Button>().ToList();
        int count = 0;
        cells.ForEach(cell =>
        {
            int index = count++;
            cell.onClick.AddListener(() => OnCellClicked(cell.gameObject, index));
        });
        ClearCells();
        currentPlayer = 1;
        DisplayPlayerName();
        moveLogger = new MoveLogger();
    }
    private void ClearCells() => cells.ForEach(cell => cell.GetComponentInChildren<TextMeshProUGUI>().text = "");
    public void OnCellClicked(GameObject cell, int index)
    {
        if (IsCellOccupied(index) || mediaPlaying) return;

        SetCurrentCell(cell, index);

        downloadMediaHandler.DownloadMedia(currentPlayer);
    }
    #region Media Play Back
    private bool IsCellOccupied(int index) => moves.ContainsKey(index);
    private void SetCurrentCell(GameObject cell, int index)
    {
        currentCell = cell;
        currentCellIndex = index;
    }

    private void PlayMedia(IMediaPlayer mediaPlayer)
    {
        mediaPlaying = true;
        SetMediaParent(mediaPlayer);
        ResetMediaRectTransform(mediaPlayer);
        mediaPlayer.gameObject.SetActive(true);
        mediaPlayer.Play();

        mediaPlayer.OnPlaybackCompleted += OnMediaPlaybackCompleted;
    }

    private void SetMediaParent(IMediaPlayer mediaPlayer) => mediaPlayer.transform.parent = currentCell.transform;

    private void ResetMediaRectTransform(IMediaPlayer mediaPlayer)
    {
        RectTransform rect = mediaPlayer.GetComponent<RectTransform>();
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    private void OnMediaPlaybackCompleted()
    {
        UpdateCellAfterPlayback();
        SwitchPlayer();
        mediaPlaying = false;
        ClearCurrentCell();

        DisplayPlayerName();
        UnsubscribeFromMediaPlayback(videoPrefab);
        UnsubscribeFromMediaPlayback(audioPrefab);
    }

    private void UpdateCellAfterPlayback()
    {
        if (currentCell == null || currentCellIndex == -1) return;

        moves[currentCellIndex] = currentPlayer == 1 ? "X" : "O";
        currentCell.GetComponentInChildren<TextMeshProUGUI>().text = moves[currentCellIndex];
        board[currentCellIndex] = currentPlayer;
        moveLogger.LogMove(currentPlayer, currentCellIndex);
        CheckForWinner();
    }
    private void UnsubscribeFromMediaPlayback(IMediaPlayer mediaPlayer)
    {
        if (mediaPlayer != null)
        {
            mediaPlayer.OnPlaybackCompleted -= OnMediaPlaybackCompleted;
        }
    }

    #endregion
    private void SwitchPlayer() => currentPlayer = currentPlayer == 1 ? 2 : 1;

    private void DisplayPlayerName()
    {
        var localizedString = LocalizationSettings.StringDatabase.GetLocalizedString("EN-HI", "Player"+currentPlayer);
        currentPlayerName.text = localizedString;
    }
    private void ClearCurrentCell()
    {
        currentCell = null;
        currentCellIndex = -1;
    }

    private void CheckForWinner()
    {
        // all possible winning combinations
        int[][] winningCombinations = new int[][]
        {
        new int[] { 0, 1, 2 }, // Top 
        new int[] { 3, 4, 5 }, // Middle
        new int[] { 6, 7, 8 }, // Bottom 
        new int[] { 0, 3, 6 }, // Left 
        new int[] { 1, 4, 7 }, // Middle 
        new int[] { 2, 5, 8 }, // Right 
        new int[] { 0, 4, 8 }, // from top left bottom right
        new int[] { 2, 4, 6 }  // from top right bottom left
        };
        //Log Move
        moveLogger.SaveMoves();

        foreach (int[] combination in winningCombinations)
        {
            int a = combination[0];
            int b = combination[1];
            int c = combination[2];

            if (board[a] != 0 && board[a] == board[b] && board[a] == board[c])
            {
                int winner = board[a]; // 1 = Player 1 (X), 2 = Player 2 (O)
                DisplayWinner(winner);
                return;
            }
        }

        // Check for a draw
        if (IsBoardFull())
            DisplayWinner(0);
    }

    private bool IsBoardFull()
    {
        foreach (int cell in board)
        {
            if (cell == 0) return false;
        }
        return true;
    }

    private void DisplayWinner(int winner)
    {
        if (winner == 0)
        {
            Debug.Log("It's a draw!");
            // Implement draw UI logic
        }
        else
        {
            Debug.Log("Player " + winner + " wins!");
            // Implement winner UI logic
        }

        // Optionally, reset the game or prompt for a new game
        ResetGame();
    }

    private void ResetGame()
    {
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = 0;
        }
        Invoke("ClearCells", 3);
    }
}
