using System.Collections;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [SerializeField] private float _minSpeedRotation = 10f;
    [SerializeField] private float _maxSpeedRotation = 20f;
    [SerializeField] private Vector3 _direction = Vector3.one;

    private void Start()
    {
        StartCoroutine(Rotate());

        IEnumerator Rotate()
        {
            Transform thisTransform = transform;

            float x = Random.Range(_minSpeedRotation, _maxSpeedRotation) * _direction.x;
            float y = Random.Range(_minSpeedRotation, _maxSpeedRotation) * _direction.y;
            float z = Random.Range(_minSpeedRotation, _maxSpeedRotation) * _direction.z;
            Vector3 rotate = new Vector3(x, y, z) * Time.fixedDeltaTime;

            WaitForFixedUpdate delay = new();
            
            while (true)
            {
                thisTransform.Rotate(rotate);
                yield return delay;
            }
        }
    }
}
