using UnityEngine;

public class LeaderboardMenu : MenuNavigation
{
    [Space]
    [SerializeField] private LeaderboardGUI _leaderboard;

    protected override void OnEnable()
    {
        base.OnEnable();

        _leaderboard.Show();
    }

    private void OnDisable()
    {
        _leaderboard.Hide();
    }

}
