using UnityEngine;

public class MusicGame : MonoBehaviour
{
    private void Start() => GlobalMusic.Instance.GamePlay();

    private void OnDisable()
    {
        if(GlobalMusic.Instance != null)
            GlobalMusic.Instance.GameStop();
    }
}
