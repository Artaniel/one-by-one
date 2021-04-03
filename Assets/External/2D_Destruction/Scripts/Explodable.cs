using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Explodable : MonoBehaviour
{
    public System.Action<List<GameObject>> OnFragmentsGenerated;

    public bool allowRuntimeFragmentation = false;
    public int extraPoints = 0;
    public int subshatterSteps = 0;

    public string fragmentLayer = "Default";
    public string sortingLayerName = "Default";
    public int orderInLayer = 3;

    public enum ShatterType
    {
        Triangle,
        Voronoi
    }
    public ShatterType shatterType;
    public List<GameObject> fragments = new List<GameObject>();
    private List<List<Vector2>> polygons = new List<List<Vector2>>();
   
    /// <summary>
    /// Creates fragments if necessary and destroys original gameobject
    /// </summary>
    public void Explode()
    {
        //if fragments were not created before runtime then create them now
        if (fragments.Count == 0 && allowRuntimeFragmentation)
        {
            GenerateFragments();
        }
        //otherwise unparent and activate them
        else
        {
            foreach (GameObject frag in fragments)
            {
                frag.transform.parent = null;
                var mRend = frag.GetComponent<MeshRenderer>();
                if (mRend.sharedMaterial == null)
                {
                    var sRend = GetComponent<SpriteRenderer>();
                    mRend.sharedMaterial = sRend.sharedMaterial;
                    mRend.sharedMaterial.SetTexture("_MainTex", sRend.sprite.texture);
                }
                frag.SetActive(true);
                if (Labirint.GetCurrentRoom()) frag.transform.parent = Labirint.GetCurrentRoom().transform;
            }
        }
        //if fragments exist destroy the original
        if (fragments.Count > 0)
        {
            GetComponent<UnityEngine.Collider2D>().isTrigger = true;
            GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }

    /// <summary>
    /// Creates fragments and then disables them
    /// </summary>
    public void FragmentInEditor()
    {
        if (fragments.Count > 0)
        {
            DeleteFragments();
        }
        GenerateFragments();
        SetPolygonsForDrawing();
        foreach (GameObject frag in fragments)
        {
            frag.transform.parent = transform;
            frag.SetActive(false);
        }
    }

    public void FragmentInEditor_meshSave()
    {
        if (fragments.Count > 0)
        {
            DeleteFragments();
        }
        GenerateFragments(true);
        SetPolygonsForDrawing();

        foreach (GameObject frag in fragments)
        {
            frag.transform.parent = transform;
            frag.SetActive(false);
        }
    }

    public void DeleteFragments()
    {
        foreach (GameObject frag in fragments)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(frag);
            }
            else
            {
                Destroy(frag);
            }
        }
        fragments.Clear();
        polygons.Clear();
    }
    /// <summary>
    /// Turns Gameobject into multiple fragments
    /// </summary>
    private void GenerateFragments(bool meshSaved = false)
    {
        fragments = new List<GameObject>();

        switch (shatterType)
        {
            case ShatterType.Triangle:
                fragments = SpriteExploder.GenerateTriangularPieces(gameObject, extraPoints, subshatterSteps, null);
                break;
            case ShatterType.Voronoi:
                fragments = SpriteExploder.GenerateVoronoiPieces(gameObject, extraPoints, subshatterSteps, null);
                break;
            default:
                Debug.Log("invalid choice");
                break;
        }

#if UNITY_EDITOR
        for (int i = 0; i < fragments.Count; i++)
        {
            if (fragments[i] != null)
            {
                fragments[i].layer = LayerMask.NameToLayer(fragmentLayer);
                fragments[i].GetComponent<Renderer>().sortingLayerName = sortingLayerName;
                fragments[i].GetComponent<Renderer>().sortingOrder = orderInLayer;

                /// prefab mesh save        
                if (meshSaved)
                {
                    if (!string.IsNullOrEmpty("Assets/External/2D_Destruction/SavedMeshes"))
                    {
                        Directory.CreateDirectory("Assets/External/2D_Destruction/SavedMeshes");
                    }

                    var mesh = fragments[i].GetComponent<MeshFilter>().sharedMesh;
                    AssetDatabase.CreateAsset(mesh, "Assets/External/2D_Destruction/SavedMeshes/" + transform.parent.name + "_" + i + ".asset");
                }
            }
        }

        AssetDatabase.SaveAssets();
#endif
        foreach (ExplodableAddon addon in GetComponents<ExplodableAddon>())
        {
            if (addon.enabled)
            {
                addon.OnFragmentsGenerated(fragments);
            }
        }
    }

    private void SetPolygonsForDrawing()
    {
        polygons.Clear();
        List<Vector2> polygon;

        foreach (GameObject frag in fragments)
        {
            polygon = new List<Vector2>();
            foreach (Vector2 point in frag.GetComponent<PolygonCollider2D>().points)
            {
                Vector2 offset = RotateAroundPivot((Vector2)frag.transform.position, (Vector2)transform.position, Quaternion.Inverse(transform.rotation)) - (Vector2)transform.position;
                offset.x /= transform.localScale.x;
                offset.y /= transform.localScale.y;
                polygon.Add(point + offset);
            }
            polygons.Add(polygon);
        }
    }

    private Vector2 RotateAroundPivot(Vector2 point, Vector2 pivot, Quaternion angle)
    {
        Vector2 dir = point - pivot;
        dir = angle * dir;
        point = dir + pivot;
        return point;
    }

    void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            if (polygons.Count == 0 && fragments.Count != 0)
            {
                SetPolygonsForDrawing();
            }

            Gizmos.color = Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector2 offset = (Vector2)transform.position * 0;
            foreach (List<Vector2> polygon in polygons)
            {
                for (int i = 0; i < polygon.Count; i++)
                {
                    if (i + 1 == polygon.Count)
                    {
                        Gizmos.DrawLine(polygon[i] + offset, polygon[0] + offset);
                    }
                    else
                    {
                        Gizmos.DrawLine(polygon[i] + offset, polygon[i + 1] + offset);
                    }
                }
            }
        }
    }
}
