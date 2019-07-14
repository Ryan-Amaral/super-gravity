using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GravitySource : MonoBehaviour
{

    public float density = 1;

    // Start is called before the first frame update
    void Start()
    {
        //MeshCollider newCol = CopyComponent(gameObject.GetComponent<MeshCollider>(), gameObject);
        //newCol.sharedMesh.triangles = newCol.sharedMesh.triangles.Reverse().ToArray();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // http://answers.unity.com/answers/589400/view.html
    T CopyComponent<T>(T original, GameObject destination) where T : Component{
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields){
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
}
