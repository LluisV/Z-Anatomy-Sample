using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TMPro;

public class ModelSetUp : MonoBehaviour
{
#if UNITY_EDITOR

    [MenuItem("Models/Delete global labels FIRST STEP (Select all)")]
    static void DeleteGlobalLabels()
    {
        DeleteGlobalLabels(Selection.gameObjects);
    }

    [MenuItem("Models/Process Model SECOND STEP (Select parent only)")]
    static void ProcessModel()
    {
        try
        {
            AddScriptAndMesh(Selection.activeGameObject.GetComponentsInChildren<Transform>(true).ToList());
            CreateLabels(Selection.activeGameObject.GetComponentsInChildren<Transform>(true).ToList());
            SetLayer(Selection.activeGameObject.GetComponentsInChildren<Transform>(true).ToList());
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    //This method deltes the lines of the global groups
    private static void DeleteGlobalLabels(GameObject[] selectedObjects)
    {
        foreach (var obj in selectedObjects)
        {
            if ((obj.name.Contains(".j") || obj.name.Contains(".i")) && obj.transform.parent.name.Contains(".g"))
            {
                DestroyImmediate(obj.gameObject);
            }
        }
    }

     private static void AddScriptAndMesh(List<Transform> gameObjects)
     {
         string actual = "";
         try
         {
             foreach (Transform child in gameObjects)
             {
                 actual = child.name;
                if(!child.name.Contains(".j") && !child.name.Contains(".i"))
                {
                    //child.gameObject.AddComponent<NameAndDescription>();
                    //child.gameObject.AddComponent<BodyPartVisibility>();
                }
                if (!child.name.Contains(".j") && !child.name.Contains(".i" )&& !child.name.Contains(".t") && !child.name.Contains(".s") && child.GetComponent<MeshRenderer>() != null)
                {
                     child.gameObject.AddComponent<MeshCollider>();
                }
             }
         }
         catch (System.Exception e)
         {
             Debug.Log("Error adding script and mesh to " + actual + ": " + e.Message);
         }
     }

    private static void SetLayer(List<Transform> gameObjects)
    {
        foreach (var obj in gameObjects)
        {
            obj.gameObject.layer = LayerMask.NameToLayer("Body");

        }
    }

    private static void CreateLabels(List<Transform> gameObjects)
    {
        foreach (Transform child in gameObjects)
        {
            if (child.name.Contains(".j") || child.name.Contains(".i"))
            {
                CreateLinePoints(child.gameObject);
                /*Line script = child.gameObject.AddComponent<Line>();
                script.lineMaterial = (Material)Resources.Load("LineMaterial", typeof(Material));
                script.transform.parent = script.transform.parent.parent;
                script.gameObject.SetActive(false);*/
            }
            else if (child.name.Contains(".t") || child.name.Contains(".s"))
            {
                /*Label script = child.gameObject.AddComponent<Label>();
                child.gameObject.AddComponent<TextMeshPro>();
                script.labelMaterial = (Material)Resources.Load("LabelMaterial", typeof(Material));*/
            }
        }
    }

     private static void CreateLinePoints(GameObject gameObject)
     {
         try
         {
            string name = gameObject.name;
            Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

            //This should never happen
            if (mesh.vertices.Length == 0)
            {
                Debug.Log("This line has no vertices.");
                return;
            }

            // Origin point
            Vector3 min = mesh.vertices[0];
            // Final point
            Vector3 max = mesh.vertices[7];
            // If for some reason the line does not have 8 vertices
            if(max == null)
                max = mesh.vertices[mesh.vertices.Length-1];

            // Set the origin point as child of the label
            GameObject minPoint = new GameObject();
            minPoint.name = "minPoint";
            minPoint.transform.parent = gameObject.transform;
            minPoint.transform.localPosition = min;

            // Set the final point as child of the label
            GameObject maxPoint = new GameObject();
            maxPoint.name = "maxPoint";
            maxPoint.transform.parent = gameObject.transform;
            maxPoint.transform.localPosition = max;

            // We don't need the mf and mr of the line, as it will be a LineRenderer
            DestroyImmediate(gameObject.GetComponent<MeshFilter>());
            DestroyImmediate(gameObject.GetComponent<MeshRenderer>());
         }
         catch (System.Exception e)
         {
             Debug.Log("Error creating line points of " + gameObject.name + ": " + e.Message);
         }
     }

#endif

}
