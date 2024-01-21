using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SlabCreate : MonoBehaviour
{
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private NavMeshObstacle _obstacle;
    [SerializeField] private float _borderSize = 0.075f;
    [SerializeField] private float _density = 2f;

    public void Create(string name, Vector3 size, List<Material> materials)
    {
        if (size == Vector3.zero)
            return;

        Vector2 size2d = size.To2D();
        float height = size.y;
        CustomMesh customMesh = new(name, 2, 0, 1);

        Construction();

        GetComponent<MeshFilter>().sharedMesh = customMesh.ToMesh();

        #region Local Functions

        void Construction()
        {
            customMesh.AddUV(new(-size2d / 2f, size2d), 0);
            Polygon top = new(size);
            customMesh.AddPolygon(top, top, 0);
            PolygonsLoop loop = new(new(size2d), top);
            for (int i = 0; i < loop.Count; i++)
                customMesh.AddPolygon(loop[i], top, 1, -1, 0);

            ColliderAndNavSetup();
            MaterialSetup();
        }
        void ColliderAndNavSetup()
        {
            Vector3 center = new(0, height/2f, 0);

            _collider.size = size;
            _collider.center = center;

            _obstacle.size = size;
            _obstacle.center = center;
        }
        void MaterialSetup()
        {
            materials[0].SetTailing(size2d);
            materials[0].SetSeed(0.1f, 789.98f);
            materials[0].SetDensity(_density);
            Vector2 border = new(_borderSize / (size2d.x * _density), _borderSize / (size2d.y * _density));
            materials[0].SetBorder(border);
            border = new(_borderSize / size.y, _borderSize / size.x);
            materials[1].SetBorder(border);
            GetComponent<MeshRenderer>().SetSharedMaterials(materials);
        }
        #endregion
    }
}
