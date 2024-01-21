using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(MeshRenderer))]
public class Artefact : MonoBehaviour
{
    [SerializeField] private SFX _SFX;
    [Space]
    [SerializeField] private Material _materialOn;
    [SerializeField] private Material _materialOff;

    private BoxCollider _thisCollider;
    private MeshRenderer _thisRenderer;
    private List<Material> _materials;

    public int Id { get; private set; }

    private const int iTwoMaterial = 1;

    public void Setup(string name, int id, bool isOn)
    {
        _SFX.Setup();

        _thisCollider = GetComponent<BoxCollider>();
        _thisRenderer = GetComponent<MeshRenderer>();
        _materials = new(_thisRenderer.sharedMaterials);

        Id = id;
        gameObject.name = name + id;

        if (isOn)
            On();
        else
            Off();
    }

    private void On()
    {
        _thisCollider.enabled = true;
        _materials[iTwoMaterial] = _materialOn;
        _thisRenderer.SetSharedMaterials(_materials);
    }

    private void Off()
    {
        _thisCollider.enabled = false;
        _materials[iTwoMaterial] = _materialOff;
        _thisRenderer.SetSharedMaterials(_materials);
    }

    private void Pick()
    {
        Off();
        _SFX.Play();

        GameData.Instance.ArtefactOff(Id);
    }

    private void OnTriggerEnter(Collider other) => Pick();
}
