using System;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance;

    public bool IsPaused = false;

    private InputSystem_Actions controls;

    [SerializeField]
    private int score = 0;
    private int hightScore = 0;

    private int lives = 3;

    [SerializeField]
    private TextMeshProUGUI _scoreUI;
    [SerializeField]
    private TextMeshProUGUI _hightScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        controls = new InputSystem_Actions();
        controls.UI.Pause.performed += ctx => Pause();
    }
    private void OnEnable()
    {
        controls.UI.Pause.Enable();
    }
    private void OnDisable()
    {
        controls.UI.Pause.Disable();
    }
    private void Pause()
    {
        IsPaused = !IsPaused;
    }

    
    public void AddScore(int points)
    {
        score += points;
       if(score.ToString().Length <= 2)
        {
            _scoreUI.text = "00 " +score.ToString();
            return;
        }
        if (score.ToString().Length == 3)
        {
            _scoreUI.text= "0" +score.ToString();
            return;
        }
        _scoreUI.text = score.ToString();
    }
  

    private void ResetScore() => score = 0;

    public void LoseLife()
    {
        lives--;
        if (lives == 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        //TODO: Implémenter le GameOver
        SaveScore();
    }

    public void CompletedLevel()
    {
        //TODO : Implémenter le completed levels
    }

    private void SaveScore()
    {
        if (score > hightScore)
        {
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.Save();
        }
        
    }

    private void GetHightScore()
    {
        hightScore = PlayerPrefs.GetInt("Score", score);

    }
}
