using TMPro;
using UnityEngine;

public class GameEndMenu : MenuNavigation
{
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
        _caption.text = Localization.Instance.GetText(key);

        _score.text = score.ToString();
    }

    public void SetActive(bool isActive) => gameObject.SetActive(isActive);


}
