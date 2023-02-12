#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Add to a parent object that contains all the meshes you want to combine.
/// </summary>
public class CombineMeshes : MonoBehaviour
{ 
    public int count;// used for naming the combined objects
    [ContextMenu("Combine mesh")]
    ///Use to combine meshes usin THE SAME material together.
    public void Combine()
    {
        //make sure obj is at world center.
        transform.position = Vector3.zero; 
    
        List<CombineInstance> combinedMeshList = new List<CombineInstance>();
        //Get all meshfilters from this currentObject and its children. Include inactive.
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true); 
        for (int m = 0; m < meshFilters.Length; m++)
        {
            MeshFilter meshFilter = meshFilters[m];
            CombineInstance combine = new CombineInstance();

            //Assign mesh properties to the combined struct and then add to the combined list
            combine.mesh = meshFilter.sharedMesh;
            combine.transform = meshFilter.transform.localToWorldMatrix;
            combinedMeshList.Add(combine);
        }
        //Make a new mesh and add all the combined meshes to it
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combinedMeshList.ToArray());

        //Create new game object that will hold the new combined mesh.
        var newObj = new GameObject($"Combined {name.Replace("Generator","")} {count}");
        //Need to save mesh to assets.
        SaveMesh(combinedMesh, newObj.name, false, true);
        
        newObj.AddComponent<MeshFilter>().mesh = combinedMesh;
        newObj.AddComponent<MeshRenderer>().material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
        newObj.transform.position += new Vector3(combinedMesh.bounds.size.x, 0, combinedMesh.bounds.size.z);
        
        count++;
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
#endif
