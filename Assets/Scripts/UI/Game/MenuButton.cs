using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour
{

    private void Awake()
    {
        Button thisButton = GetComponent<Button>();
        Game game = Game.InstanceF;

        thisButton.interactable = false;
        game.EventStartGame += () => thisButton.interactable = true;
        game.EventLevelCompleted += () => thisButton.interactable = false;
        game.EventGameOver += _ => thisButton.interactable = false;
    }
}
