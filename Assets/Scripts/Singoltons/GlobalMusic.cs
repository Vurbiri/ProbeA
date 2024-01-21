using UnityEngine;

public class GlobalMusic : Singleton<GlobalMusic>
{
    [SerializeField] private AudioSource _musicGame;
    [SerializeField] private AudioSource _musicMenu;

    public void MenuPlay() => _musicMenu.Play();
    public void MenuStop() => _musicMenu.Stop();

    public void GamePlay() => _musicGame.Play();
    public void GameStop() => _musicGame.Stop();

    public void Pause()
    {
        _musicMenu.Pause();
        _musicGame.Pause();
    }

    public void UnPause()
    {
        _musicMenu.UnPause();
        _musicGame.UnPause();
    }
}
