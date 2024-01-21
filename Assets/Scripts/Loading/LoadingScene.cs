using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] protected int _nextScene = 2;
    [SerializeField] protected Slider _slider;

    private void Start()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_nextScene);
        StartCoroutine(LoadScene());

        IEnumerator LoadScene()
        {
            while (!asyncOperation.isDone)
            {
                _slider.value = asyncOperation.progress * 1.1f; 
                yield return null;
            }
        }
    }

}
