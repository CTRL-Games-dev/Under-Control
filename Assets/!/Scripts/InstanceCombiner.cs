using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InstanceCombiner : MonoBehaviour
{
    // Source Meshes you want to combine
    [SerializeField] private GameObject sourceGameObject;
    [SerializeField] private List<MeshFilter> listMeshFilter;
    [SerializeField] private List<MeshRenderer> listMeshRenderer;

    // Make a new mesh to be the target of the combine operation
    [SerializeField] private MeshFilter TargetMesh;
    [SerializeField] private Material _outlineGlow;

    [ContextMenu("Combine Meshes")]
    private void CombineMesh()
    {
        
        listMeshFilter = new(sourceGameObject.GetComponentsInChildren<MeshFilter>());
        TargetMesh = sourceGameObject.AddComponent<MeshFilter>();
        listMeshRenderer = new(sourceGameObject.GetComponentsInChildren<MeshRenderer>());
        //Make an array of CombineInstance.
        CombineInstance[] combine = new CombineInstance[listMeshFilter.Count];
        
        //Set Mesh And their Transform to the CombineInstance
        for (int i = 0; i < listMeshFilter.Count; i++)
        {
            combine[i].mesh = listMeshFilter[i].sharedMesh;
            combine[i].transform = listMeshFilter[i].transform.localToWorldMatrix;
        }

        // Create a Empty Mesh
        var mesh = new Mesh();

        //Call targetMesh.CombineMeshes and pass in the array of CombineInstances.
        mesh.CombineMeshes(combine,false);

        //Assign the target mesh to the mesh filter of the combination game object.
        TargetMesh.mesh = mesh;

        
        sourceGameObject.AddComponent<MeshRenderer>().SetMaterials(listMeshRenderer.Select(x => x.material).ToList());
        //sourceGameObject.AddComponent<MeshRenderer>().materials = listMeshRenderer.Select(x => x.material).ToArray();

        // Print Results
        print($"<color=#20E7B0>Combine Meshes was Successful!</color>");
    }


    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }
}
