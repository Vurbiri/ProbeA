#if UNITY_EDITOR
using NaughtyAttributes;
#endif
using System.Collections.Generic;
using UnityEngine;

public class Artefacts : MonoBehaviour
{
    [SerializeField] private Artefact _prefab;
    [SerializeField] private string _name = "Artefact_";
#if UNITY_EDITOR
    [SerializeField] private List<Vector3> _artefactPosition;
#endif

    private GameData GameData => GameData.Instance;

    public void Create(List<ArtefactSaveData> saveData)
    {
        if (saveData == null || saveData.Count == 0)
            return;

        int count = saveData.Count;
        if (GameData.ModeStart != GameModeStart.LevelContinue)
            GameData.CreateArtefactsData(count);
        for (int i = 0; i < count; i++)
            Construction(saveData[i]);

        #region Local Functions
        void Construction(ArtefactSaveData art)
        {
            Artefact artefact = Instantiate(_prefab, art.Position.ToVector3(), Quaternion.identity, transform);
            artefact.Setup(_name, art.Id, GameData.Artefacts.BinaryContains(art.Id));

        }
        #endregion
    }

#if UNITY_EDITOR

    [Button]
    private void Create()
    {
        Delete();
        for (int i = 0; i < _artefactPosition.Count; i++)
            Construction(_artefactPosition[i], i);

        #region Local Functions
        void Construction(Vector3 pos, int index)
        {
            Artefact art = Instantiate(_prefab, pos, Quaternion.identity, transform);
            art.Setup(_name, index, true);
        }
        #endregion
    }

    public void CreateTest(List<ArtefactSaveData> saveData)
    {
        if (saveData == null || saveData.Count == 0)
            return;

        for (int i = 0; i < saveData.Count; i++)
            Construction(saveData[i]);

        #region Local Functions
        void Construction(ArtefactSaveData art)
        {
            Artefact artefact = Instantiate(_prefab, art.Position.ToVector3(), Quaternion.identity, transform);
            artefact.Setup(_name, art.Id, true);

        }
        #endregion
    }

    [Button]
    public void Delete()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    public List<ArtefactSaveData> Get()
    {
        int count = _artefactPosition.Count;
        List<ArtefactSaveData> artefacts = new(count);
        for (int i = 0; i < count; i++)
            artefacts.Add(new(i, _artefactPosition[i].ToArray()));
        return artefacts;
    }
    public void LoadData(List<ArtefactSaveData> data)
    {
        _artefactPosition.Clear();
        if (data == null || data.Count == 0)
            return;

        foreach (var d in data)
            _artefactPosition.Add(d.Position.ToVector3());
    }
    public void ClearData()
    {
        _artefactPosition.Clear();
    }
#endif
}
