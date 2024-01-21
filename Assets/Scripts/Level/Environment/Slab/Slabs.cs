#if UNITY_EDITOR
using NaughtyAttributes;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

public class Slabs : MonoBehaviour
{
    [SerializeField] private Slab _prefab;
    [SerializeField] private string _name = "Slab_";
    [SerializeField] private Material _materialTop;
    [SerializeField] private Material _materialSide;
#if UNITY_EDITOR
    [SerializeField] private List<SlabData> _slabsData;
#endif

    private event Action EventStop;

    public void Create(List<SlabSaveData> saveData, Color color)
    {
        if (saveData == null || saveData.Count == 0)
            return;

        List<SlabData> slabsData = new(saveData.Count);
        foreach (var s in saveData)
            slabsData.Add(new(s));

        _materialTop.SetColor(color);
        _materialSide.SetColor(color);

        string name;
        SlabData data;
        Slab slab;
        for (int i =0; i < slabsData.Count; i++) 
        { 
            List<Material> materials = new(2);
            if (i == 0)
            {
                materials.Add(_materialTop);
                materials.Add(_materialSide);
            }
            else
            {
                materials.Add(Instantiate(_materialTop));
                materials.Add(Instantiate(_materialSide));
            }
            name = _name + i;
            data = slabsData[i];
            slab = Instantiate(_prefab, data.Points[0], Quaternion.identity, transform);
            slab.gameObject.name = name;
            slab.Setup(name, data, materials);
            EventStop += slab.OnStop;
        }
    }

    public void Stop()
    {
        EventStop?.Invoke();
        EventStop = null;
    }


#if UNITY_EDITOR
    public void CreateTest(List<SlabSaveData> saveData, Color color)
    {
        List<SlabData> slabsData = new(saveData.Count);
        foreach (var s in saveData)
            slabsData.Add(new(s));

        _materialTop.SetColor(color);
        _materialSide.SetColor(color);

        SlabData data;
        Slab slab;
        for (int i = 0; i < slabsData.Count; i++)
        {
            List<Material> materials = new(2);
            if (i == 0)
            {
                materials.Add(_materialTop);
                materials.Add(_materialSide);
            }
            else
            {
                materials.Add(Instantiate(_materialTop));
                materials.Add(Instantiate(_materialSide));
            }
            data = slabsData[i];
            slab = Instantiate(_prefab, data.Points[0], Quaternion.identity, transform);
            slab.gameObject.name = _name + i;
            slab.SetupTest(_name + i, data, materials);
        }
    }
    [Button]
    private void Create()
    {
        Delete();
        SlabData data;
        Slab slab;
        for (int i = 0; i < _slabsData.Count; i++)
        {
            List<Material> materials = new(2);
            if (i == 0)
            {
                materials.Add(_materialTop);
                materials.Add(_materialSide);
            }
            else
            {
                materials.Add(Instantiate(_materialTop));
                materials.Add(Instantiate(_materialSide));
            }
            data = _slabsData[i];
            slab = Instantiate(_prefab, data.Points[0], Quaternion.identity, transform);
            slab.gameObject.name = _name + i;
            slab.SetupTest(_name + i, data, materials);
        }
    }
    [Button]
    public void Delete()
    {
        while(transform.childCount > 0) 
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
    public List<SlabSaveData> Get()
    {
        List<SlabSaveData> slabs = new(_slabsData.Count);
        foreach (var s in _slabsData)
            slabs.Add(new(s));
        return slabs;
    }
    public void LoadData(List<SlabSaveData> data)
    {
        _slabsData.Clear();
        if (data == null || data.Count == 0)
            return;

        foreach (var s in data)
            _slabsData.Add(new(s));
    }
    public void ClearData()
    {
        _slabsData.Clear();
    }
#endif
}
