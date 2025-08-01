using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class Gameplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI attempsLeft;
    public TextMeshProUGUI currentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;

    [Header("Input")]
    public TMP_InputField guessIputField;
    public Button submitButton;
    public Button newgameButton;

    [Header("Game Settings")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttempts = 12;

    private int targetNumber;
    private int currentAttempts;
    private bool isPlayerTrun;
    private bool gameActive;

    private int computerMinGuess;
    private int computerMaxGuess;
    private List<int> computerGuesses;

    void InitializeUI()
    {
        submitButton.onClick.AddListener(SubmitGuess);
        newgameButton.onClick.AddListener(StartNewGame);
        guessIputField.onSubmit.AddListener(delegate { SubmitGuess(); });
    }

    void SubmitGuess()
    { 
        if (!gameActive || !isPlayerTrun)  return;
        
        string input = guessIputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        int guess;
        if (!int.TryParse(input, out guess))
        {
            gameState.text = "<sprite=15> Please enter a valid number.";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text = $"<sprite=15> Please enter a number between {minNumber} and {maxNumber}";
            return;
        }
        ProcessGuess(guess, true);
        guessIputField.text = "";
    }

    void ProcessGuess(int guess, bool isPlayerTurn)
    {
        currentAttempts++;
        string playerName = isPlayerTurn ? "Player" : "Computer";

        gameLog.text += $"{playerName} guessed: {guess}\n";
        
        if (guess == targetNumber)
        {
            //win
            gameLog.text += $"<sprite=\"Symbols\" index=23> {playerName} wins!\n";
            EndGame();
        }
        else if (currentAttempts >= maxAttempts)
        {
            //Lose
            gameLog.text += $"<sprite=10> Game Over! The correct number was {targetNumber}\n";
            EndGame();
        }
        else
        {
            //Wrong guess - give hint
            string hint = guess < targetNumber ? "Too Lower!" : "Too Higher!";
            gameLog.text += $"<sprite=\"Symbols\" index=24> {hint}\n";

            //switch Player
            isPlayerTrun = !isPlayerTrun;
            currentPlayer.text = isPlayerTrun ? "Player Turn" : "Computer Turn";
            attempsLeft.text = $"Attempts Left: {maxAttempts - currentAttempts}";

            if (!isPlayerTrun)
            {
                guessIputField.interactable = false;
                submitButton.interactable = false;
                StartCoroutine(ComputerTurn(guess < targetNumber));
            }
            else
            {
                guessIputField.interactable = true;
                submitButton.interactable = true;
                guessIputField.Select();
                guessIputField.ActivateInputField();
            }

        }
    }

    IEnumerator ComputerTurn(bool targetIsHigher)
    {
        yield return new WaitForSeconds(2f); //wait to simulate thinking
        if (!gameActive) yield break;
        if (computerGuesses.Count >0)
        {
            int lastGuess = computerGuesses[computerGuesses.Count - 1];
            if (targetIsHigher)
            {
                computerMinGuess = lastGuess + 1;
            }
            else
            {
                computerMaxGuess = lastGuess - 1;
            }
        }
        //AI uses Binary Search strategy
        //Debug.Log($"{computerMinGuess} - {computerMaxGuess}");
        int computerGuess = (computerMinGuess + computerMaxGuess) / 2;

        computerGuesses.Add(computerGuess);

        //int computerGuess = Random.Range(minNumber, maxNumber + 1);
        ProcessGuess(computerGuess, false);
    }


    void EndGame()
    {
        gameActive = false;
        guessIputField.interactable = false;
        submitButton.interactable = false;
        currentPlayer.text = "";
        gameState.text = "Game Over! Press New Game to Start Again";
        Canvas.ForceUpdateCanvases();
    }


    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1 );
        currentAttempts = 0;
        isPlayerTrun = true;
        gameActive = true;

        currentPlayer.text = "Player Turn";
        attempsLeft.text = $"Attempts Left: {maxAttempts}";
        gameLog.text = "=== Game Log ===\n";
        gameState.text = "Game In Progress";

        guessIputField.interactable = true;
        submitButton.interactable = true;
        guessIputField.text = "";
        guessIputField.Select();
        guessIputField.ActivateInputField();

        computerMinGuess = minNumber;
        computerMaxGuess = maxNumber;
        computerGuesses = new List<int>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeUI();
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
