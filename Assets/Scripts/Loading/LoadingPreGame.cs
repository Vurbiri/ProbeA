using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPreGame : MonoBehaviour
{
    [SerializeField] protected int _nextScene = 1;
    [Space]
    [SerializeField] protected Slider _slider;

    private void Start() => Loading().Forget();

    private async UniTaskVoid Loading()
    {
        Message.Log("Start LoadingPreGame");

        float progress = 0f;
        LoadScene loadScene = new(_nextScene);
        loadScene.Start(OnProgressLoad);

        Localization localization = Localization.InstanceF;

        ProgressInitialize(0.1f);

        if (!localization.Initialize())
        {
            Message.Banner("Error loading Localization!", MessageType.FatalError);
            return;
        }

        ProgressInitialize(0.2f);

        await CreateStorages();


        ProgressInitialize(0.5f);

        Message.Log("End LoadingPreGame");
        loadScene.End();

        #region Local Functions
        async UniTask CreateStorages(string key = null)
        {
            if (!Storage.StoragesCreate())
                Message.Banner(localization.GetText("ErrorStorage"), MessageType.Error, 7000);
            
            ProgressInitialize(0.35f);

            SettingsGame.Instance.IsFirstStart = !await InitializeStorages();
            
            ProgressInitialize(0.4f);

            //============== local func ===============================
            async UniTask<bool> InitializeStorages()
            {
                bool isLoad = await Storage.Initialize(key);
            
                if (isLoad)
                    Message.Log("Storage Initialize");
                else
                    Message.Log("Storage Not Initialize");

                return Load(isLoad);

                //============== local func ===============================
                bool Load(bool b)
                {
                    bool result = false;

                    result |= SettingsGame.Instance.Initialize(b);
                    result |= PlayerStates.Instance.Initialize(b);
                    result |= GameData.Instance.Initialize(b);
                    return result;
                }
            }
        }
        void OnProgressLoad(float loading)
        {
            _slider.value = loading * 0.5f + progress;
        }
        void ProgressInitialize(float value)
        {
            progress = value;
            _slider.value = loadScene.Progress * 0.5f + progress;
        }
        #endregion
    }
}
