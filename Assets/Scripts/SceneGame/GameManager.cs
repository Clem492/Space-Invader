using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance;

    public bool IsPaused = false;
    public bool isExploding = false;
    private InputSystem_Actions controls;

    [SerializeField]
    public int score = 0;
    private int hightScore = 0;

    public int lives = 3;

    [SerializeField]
    private TextMeshProUGUI _scoreUI;
    [SerializeField]
    private TextMeshProUGUI _hightScore;
    [SerializeField]
    private TextMeshProUGUI lifeText;
    [SerializeField]
    private Image lifeOne, lifeTwo;
    [SerializeField]
    private TextMeshProUGUI gameOverText;


    [SerializeField] RectTransform coinPanel;
    [SerializeField] RectTransform rulesPanel;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject shield1,shield2,shield3,shield4;
    private GameMenu currentMenu;

    [SerializeField] GameObject carreInvisible;
    private Vector3 startPositionCarreInvisible;
    private bool skip;

    private Level currentLevel;

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
        startPositionCarreInvisible = carreInvisible.transform.position;
        currentLevel = Level.level1;
    }

    private void Start()
    {
        skip = false;
        GetHightScore();
        ResetUIScreen();
        CoinMenu();
        Debug.Log(currentLevel);
    }

    private void Update()
    {
        WhatMenu();
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

    private enum GameMenu { CoinMenu, RulesMenu, Game}

    private enum Level { level1, level2, level3, level4 }
    
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
        ResetUIScreen();
        lifeText.text = "" + lives;
        if (lives == 0)
        {
            IsPaused = true;
            StartCoroutine(GameOver());
        }
    }

    private void ResetUIScreen()
    {
        lifeText.text = ""+ lives;
        if (lives == 3)
        {
            
            lifeOne.enabled = true;
            lifeTwo.enabled = true;
        }
        if (lives == 2)
        {
            lifeTwo.enabled = false;
        }
        if (lives == 1)
        {
            lifeOne.enabled = false;
        }
        
    }

    public IEnumerator GameOver()
    {
        
        gameOverText.GetComponent<TextManager>().enabled = true;
        SaveScore();
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadScene("game");
    }



    public void SaveScore()
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
        if (hightScore.ToString().Length <= 2)
        {
            _hightScore.text = "00 " + hightScore.ToString();
            return;
        }
        if (hightScore.ToString().Length == 3)
        {
            _hightScore.text = "0" + hightScore.ToString();
            return;
        }
        _hightScore.text = hightScore.ToString();
    }


    private void CoinMenu()
    {
        currentMenu = GameMenu.CoinMenu;
        IsPaused = true;
        coinPanel.gameObject.SetActive(true);
        rulesPanel.gameObject.SetActive(false);


    }

    private void RuleMenu()
    {
        currentMenu = GameMenu.RulesMenu;
        IsPaused = true;
        coinPanel.gameObject.SetActive(false);
        rulesPanel.gameObject.SetActive(true);
    }

    private void Game()
    {
        currentMenu = GameMenu.Game;
        IsPaused = false;
        coinPanel.gameObject.SetActive(false);
        rulesPanel.gameObject.SetActive(false);
    }

    private void WhatMenu()
    {
        if (skip)
        {
            carreInvisible.transform.Translate(2, 0, 0);
        }
        if (currentMenu == GameMenu.CoinMenu)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {



                StartCoroutine(Skip());
                
                
            }
        }
        else if (currentMenu == GameMenu.RulesMenu)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(Skip());
            }
        }
    }

    private IEnumerator Skip()
    {
        skip = true;
        yield return new WaitUntil(() => carreInvisible.transform.position.x >= 0);
        skip = false;
        carreInvisible.transform.position = startPositionCarreInvisible;
        if (currentMenu == GameMenu.CoinMenu)
        {
            RuleMenu();
        }
        else if (currentMenu == GameMenu.RulesMenu)
        {
            Game();
        }
    }

    

    public void WhatLevel(ref float _stepdistance/*, ref float missilInterval*/)
    {
        if (currentLevel == Level.level1)
        {
            //mouvement des enemeie basique
            _stepdistance = 0.5f;
            //TODO : changer aussi la valeur de l'intervale missile 
        }
        else if (currentLevel == Level.level2)
        {
            //mouvement un peux plus grand que le précédent
            _stepdistance = 1f;
            
        }
        else if (currentLevel == Level.level3)
        {
            //mouvement un peux plus grand que le précédent
            _stepdistance = 1.5f;
           
        }
        else if (currentLevel == Level.level4)
        {
            //mouvement un peux plus grand que le précédent
            _stepdistance = 2f;
           
        }
        
    }

    public void NextLevel()
    {
        if (currentLevel == Level.level1)
        {
            //mouvement des enemeie basique
            currentLevel = Level.level2;
        }
        else if  (currentLevel == Level.level2)
        {
            //mouvement un peux plus grand que le précédent
            currentLevel = Level.level3;
        }
        else if (currentLevel == Level.level3)
        {
            //mouvement un peux plus grand que le précédent
            currentLevel = Level.level4;
        }
        else if (currentLevel == Level.level4)
        {
            //mouvement un peux plus grand que le précédent
            currentLevel = Level.level1;
        }
        Debug.Log(currentLevel);
    }
}
