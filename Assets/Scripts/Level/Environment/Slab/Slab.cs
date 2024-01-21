using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Slab : MonoBehaviour
{
    [SerializeField] private SlabCreate _slabCreate;

    Rigidbody _thisRigidbody;
    private const float angleR = 720;
    private Coroutine _coroutine;

    public void Setup(string name, SlabData data, List<Material> materials)
    {
        _thisRigidbody = GetComponent<Rigidbody>();
        _thisRigidbody.position = data.Points[0];
        _slabCreate.Create(name, data.Size, materials);
        _coroutine = StartCoroutine(Move(data.Points.ToArray(), data.Speed, data.Pause, data.IsRotation, data.IsRandom));
    }

    public IEnumerator Move(Vector3[] points, float speed, float pause, bool isRotation, bool isRandom)
    {
        int length = points.Length;
        bool onlyRotation = length < 2 && isRotation;
        if (length < 2 && !isRotation)
            yield break;

        float speedMove = speed * Time.fixedDeltaTime;
        float angle;
        Quaternion speedRotation = Quaternion.identity;
        int endIndex = 0;
        Vector3 target = _thisRigidbody.position;
        Vector3 currentEulerAngles;
        Quaternion currentRotation = Quaternion.identity;
        WaitForFixedUpdate waitForFixedUpdate = new();
        WaitForSeconds waitForSeconds = new(pause);

        if (onlyRotation)
            CalkRotation();

        while (true)
        {
            if (!onlyRotation && Vector3.Distance(_thisRigidbody.position, target) < 0.001f)
            {
                endIndex = EndIndex();
                target = points[endIndex];

                if (isRotation)
                    CalkRotation();

                yield return waitForSeconds;
                yield return waitForFixedUpdate;
            }

            if (!onlyRotation)
                _thisRigidbody.MoveTowards(target, speedMove);
            if (isRotation)
                _thisRigidbody.Rotation(speedRotation);

            yield return waitForFixedUpdate;
        }

        int EndIndex()
        {
            if (!isRandom)
                return points.Next(endIndex);

            int index = endIndex;
            while (index == endIndex)
                index = Random.Range(0, length);
            return index;
        }

        void CalkRotation()
        {
            currentEulerAngles = _thisRigidbody.rotation.eulerAngles;
            currentEulerAngles.y = angleR;
            currentRotation.eulerAngles = currentEulerAngles;
            _thisRigidbody.rotation = currentRotation;
            angle = angleR * Time.fixedDeltaTime * speed;
            if (!onlyRotation)
                angle /= Vector3.Distance(_thisRigidbody.position, target);
            speedRotation = Quaternion.Euler(Vector3.up *  angle);
        }
    }

    public void OnStop()
    {
        if (_coroutine == null) return;
        StartCoroutine(OnStopCoroutine());

        IEnumerator OnStopCoroutine()
        {
            yield return new WaitForSeconds(1f);
            StopCoroutine(_coroutine);
        }
    }

#if UNITY_EDITOR
    public void SetupTest(string name, SlabData data, List<Material> materials)
    {
        transform.position = data.Points[0];
        _slabCreate.Create(name, data.Size, materials);
    }
#endif
}
