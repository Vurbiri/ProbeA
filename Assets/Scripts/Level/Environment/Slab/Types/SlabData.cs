using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlabData 
{
    [SerializeField] private Vector3 _size = Vector3.one;
    [Space]
    [SerializeField] private List<Vector3> _points;
    [SerializeField] private float _speed;
    [SerializeField] private float _pause;
    [SerializeField] private bool _isRotation;
    [SerializeField] private bool _isRandom;

    public Vector3 Size => _size;
    public List<Vector3> Points => _points;
    public float Speed => _speed;
    public float Pause => _pause;
    public bool IsRotation => _isRotation;
    public bool IsRandom => _isRandom;

    public SlabData(SlabSaveData data)
    {
        _size = data.Size.ToVector3();
        _speed = data.Speed;
        _pause = data.Pause;
        _isRotation = data.IsRotation == 1;
        _isRandom = data.IsRandom == 1;

        _points = new(data.Points.Count);
        foreach (var point in data.Points)
            _points.Add(point.ToVector3());
    }
}
