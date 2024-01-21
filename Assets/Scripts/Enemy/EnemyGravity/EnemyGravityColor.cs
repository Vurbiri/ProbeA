using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class EnemyGravityColor : MonoBehaviour
{
    [SerializeField] private EnemyGravityField _gravityField;

    private MeshRenderer _thisMeshRenderer;

    private void Awake()
    {
        _thisMeshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEventColorUpdate(float value)
    {
        value = Mathf.Clamp01(value);
        _thisMeshRenderer.material.color = new(value, value, value, 1f);
    }

    private void OnEnable()
    {
        _gravityField.EventColorUpdate += OnEventColorUpdate;
    }

    // Update is called once per frame
    private void OnDisable()
    {
        _gravityField.EventColorUpdate -= OnEventColorUpdate;
    }
}
