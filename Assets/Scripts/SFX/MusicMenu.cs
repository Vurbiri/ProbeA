using UnityEngine;

public class MusicMenu : MonoBehaviour
{
    private void Start() => GlobalMusic.Instance.MenuPlay();

    private void OnDisable()
    {
        if (GlobalMusic.Instance != null)
            GlobalMusic.Instance.MenuStop();
    }
}
