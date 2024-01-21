using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PauseMenuText : TextFormatLocalization
{
    private void Start()
    {
       Setup(GameData.Instance.LevelCurrent);
    }
}
