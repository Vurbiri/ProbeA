using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSize : MonoBehaviour
{
    [SerializeField] private float _verticalSize = 10.7f;
    [Space]
    [SerializeField] private float _horizontalSizeMin = 17.4f;
    [Space]
    [SerializeField] private float _timeRateUpdate = 1.5f;


    private void Start()
    {
        StartCoroutine(RatioUpdate());

        IEnumerator RatioUpdate()
        {
            Camera thisCamera = GetComponent<Camera>();
            
            float aspectRatio;
            float aspectRatioOld = 0f;

            float defaultSize = _verticalSize;
            float verticalSize;
            float horizontalSize;
            WaitForSecondsRealtime delay = new(_timeRateUpdate);

            while (true)
            {
                aspectRatio = thisCamera.aspect;

                if (aspectRatio != aspectRatioOld)
                {
                    aspectRatioOld = aspectRatio;

                    verticalSize = defaultSize;
                    horizontalSize = verticalSize * aspectRatio;
                    
                    if (horizontalSize < _horizontalSizeMin)
                        verticalSize = _horizontalSizeMin / aspectRatio;

                    thisCamera.orthographicSize = verticalSize;
                }

                yield return delay;
            }
        }
    }
}
