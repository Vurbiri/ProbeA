using TMPro;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

[RequireComponent(typeof(TMP_Text))]
public class FPSCounter : MonoBehaviour
{
    [SerializeField] private float _updateInterval = 0.2f;

    [SerializeField] private FPSGraph _graph;

    private TMP_Text _thisTextFPS;
    private float _time = 0f;
    private int _frames = 0;

    private int _fps = 0;
    private int _fpsMax = int.MinValue;

    private const string text = "FPS:\n{0}\n\nAvg:\n{1:N1}\n\nMax:\n{2}\n\nMin:\n{3}\n\n";

    void Start()
    {
        _thisTextFPS = GetComponent<TMP_Text>();
    }

    void Update()
    {
        _time += Time.unscaledDeltaTime;
        ++_frames;

        if (_time >= _updateInterval)
        {
            _fps = Mathf.RoundToInt(_frames / _time);
            _fpsMax = Mathf.Max(_fps, _fpsMax);
            var(avg, min, max) = _graph.UpdateTexture(_fps, _fpsMax);
            _fpsMax = max;
            _thisTextFPS.text = string.Format(text, _fps, avg, max, min);

            _time = 0.0f;
            _frames = 0;
        }
    }
}
