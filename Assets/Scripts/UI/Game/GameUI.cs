using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameEndMenu _gameEndMenu;
    [Space]
    [SerializeField] private TMP_Text _timer;
    [Space]
    [SerializeField] private ScreenOnOff _screenOnOff;
    [SerializeField] private GameObject _screenNoise;
    [SerializeField] private float _timeNoise = 5f;
    [Space]
    [SerializeField] private int _sceneMainMenu = 1;
    [SerializeField] private int _sceneLevelMenu = 3;

    private Game _game;
    private readonly WaitForSecondsRealtime _pauseOneSecond = new(1f);

    private void Awake()
    {
        _game = Game.InstanceF;

        _pauseMenu.SetActive(false);
        _gameEndMenu.SetActive(false);
        _screenNoise.SetActive(false);

        _game.EventTimer += OnTimer;

        _game.EventStartGame += _screenOnOff.Open;
        _game.EventPause += OnPause;
        _game.EventLevelCompleted += OnLevelCompleted;
        _game.EventGameCompleted += OnGameCompleted;
        _game.EventGameOver += OnGameOver;
    }

    private void OnTimer(string time)
    {
        _timer.text = time;
    }

    private void OnPause(bool pause)
    {
        _screenOnOff.Switch(pause);
        _pauseMenu.SetActive(pause);
    }

    private void OnLevelCompleted()
    {
        StartCoroutine(LevelCompletedCoroutine());

        IEnumerator LevelCompletedCoroutine()
        {
            LoadScene menu = new(_sceneLevelMenu);
            menu.Start();
            yield return _pauseOneSecond;
            _screenOnOff.Close();
            yield return _pauseOneSecond;
            menu.End();
        }
    }

    private void OnGameCompleted(int score)
    {
        _gameEndMenu.GameComplete(score);
        StartCoroutine(GameOverCoroutine());

        IEnumerator GameOverCoroutine()
        {
            yield return _pauseOneSecond;
            _gameEndMenu.SetActive(true);
            _screenOnOff.Close();
        }
    }

    private void OnGameOver(int score)
    {
        _gameEndMenu.GameOver(score);
        StartCoroutine(GameOverCoroutine());

        IEnumerator GameOverCoroutine()
        {
            yield return _pauseOneSecond;
            _gameEndMenu.SetActive(true);
            _screenNoise.SetActive(true);

            yield return new WaitForSecondsRealtime(_timeNoise);
            _screenOnOff.Close();

            yield return _pauseOneSecond;
            _screenNoise.SetActive(false);
        }
    }

    public void OnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(_sceneMainMenu);
    }

    private void OnDestroy()
    {
        if(Game.Instance != null)
        {
            _game.EventTimer -= OnTimer;

            _game.EventStartGame -= _screenOnOff.Open;
            _game.EventPause -= OnPause;
            _game.EventLevelCompleted -= OnLevelCompleted;
            _game.EventGameCompleted -= OnGameCompleted;
            _game.EventGameOver -= OnGameOver;
        }
    }
}
