using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add to a parent object that contains all the meshes you want to combine.
/// </summary>
public class CombineMeshes : MonoBehaviour
{ 
    [ContextMenu("Combine mesh")]
    ///Use to combine meshes usin THE SAME material together.
    public void Combine()
    {
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
        var newObj = new GameObject($"Combined {name}");
       
        newObj.AddComponent<MeshFilter>().mesh = combinedMesh;
        newObj.AddComponent<MeshRenderer>().material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
        newObj.transform.position += new Vector3(combinedMesh.bounds.size.x, 0, combinedMesh.bounds.size.z);
    }
}
