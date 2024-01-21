using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameEndMenu : MenuNavigation
{
    [Space]
    [SerializeField] private LeaderboardGUI _leaderboard;
    [SerializeField] private GameObject _leaderboardPanel;
    [Space]
    [SerializeField] private TMP_Text _caption;
    [SerializeField] private string _keyGameComplete = "GameComplete";
    [SerializeField] private string _keyGameOver = "GameOver";
    [Space]
    [SerializeField] private TMP_Text _score;

    public void GameComplete(int score) => GameEnd(score, _keyGameComplete);
    public void GameOver(int score) => GameEnd(score, _keyGameOver);

    private void GameEnd(int score, string key)
    {
        AddScoreAsync().Forget();
        _caption.text = Localization.Instance.GetText(key);

        async UniTaskVoid AddScoreAsync()
        {
            _score.text = score.ToString();

            if (YandexSDK.Instance.IsLeaderboard)
                await _leaderboard.TrySetScoreAndReward(score);
        }
    }

    public void SetActive(bool isActive) => gameObject.SetActive(isActive);

    protected override void OnEnable()
    {
        base.OnEnable();

        if(YandexSDK.Instance.IsLeaderboard)
            Show();
        else
            Hide();
    }

    private void Show()
    {
        _leaderboard.Show();
        _leaderboardPanel.SetActive(true);
    }

    private void Hide() 
    {
        _leaderboard.Hide();
        _leaderboardPanel.SetActive(false);
    }
}
