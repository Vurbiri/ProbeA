using UnityEngine;

[CreateAssetMenu(fileName = "Column_", menuName = "ProbeA/Column", order = 51)]
public class ColumnShape : ScriptableObject
{
    [SerializeField] private float _sideSizeBase = 1.0f;
    [SerializeField] private float _sideSizeTop = 1.2f;
    [SerializeField] private float _totalHeight = 6f;
    [SerializeField] private float _baseHeight = 1f;

    public float SideSizeBase => _sideSizeBase;
    public float SideSizeTop => _sideSizeTop;
    public float TotalHeight => _totalHeight;
    public float BaseHeight => _baseHeight;
}
