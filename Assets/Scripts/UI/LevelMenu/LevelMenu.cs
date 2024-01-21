using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MenuNavigation
{
    [Space]
    [SerializeField] private TMP_Text _caption;
    [SerializeField] private string _captionKey = "LevelComplete";
    [Space]
    [SerializeField] private TMP_Text _scorePlus;
    [SerializeField] private TMP_Text _expPlus;
    [SerializeField] private TMP_Text _score;
    [Space]
    [SerializeField] private int _sceneGame = 2;

    private GameData _gameData;
    private Localization _localization;

    
    protected override void Awake()
    {
        base.Awake();
        
        _gameData = GameData.InstanceF;
        _localization = Localization.InstanceF;

        SetTextLevelCompleted();
        _localization.EventSwitchLanguage += SetTextLevelCompleted;

        _expPlus.text = _gameData.ExpReward.ToString();
        _scorePlus.text  = _gameData.ScoreReward.ToString();
        _score.text = _gameData.Score.ToString();
    }

    public void OnToGame()
    {
        SceneManager.LoadSceneAsync(_sceneGame);
    }
        

    private void SetTextLevelCompleted()
    {
        _caption.text = string.Format(_localization.GetText(_captionKey), _gameData.LevelCurrent - 1);
    }

    private void OnDestroy()
    {
        if(Localization.Instance != null)
            _localization.EventSwitchLanguage -= SetTextLevelCompleted;
    }
}
