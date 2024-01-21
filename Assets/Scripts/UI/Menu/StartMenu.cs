using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MenuNavigation
{
    [Space]
    [SerializeField] private LeaderboardGUI _leaderboard;
    [Space]
    [SerializeField] private int _sceneGame = 4;

    private GameData _gameData;

    private void Start()
    {
        _gameData = GameData.InstanceF;

        if(_gameData.ModeStart == GameModeStart.GameNew)
        {
            SceneManager.LoadSceneAsync(_sceneGame);
            gameObject.SetActive(false);
        }
    }

    public void OnContinue()
    {
        SceneManager.LoadSceneAsync(_sceneGame);
    }

    public void OnNew()
    {
        NewAsync().Forget();

        async UniTaskVoid NewAsync()
        {
            LoadScene game = new(_sceneGame);
            game.Start();

            if (YandexSDK.Instance.IsLeaderboard)
                await _leaderboard.TrySetScoreAndReward(_gameData.Score, false);
            _gameData.CreateNewGameData();
            _gameData.Save(true);

            game.End();
        }
    }
}
