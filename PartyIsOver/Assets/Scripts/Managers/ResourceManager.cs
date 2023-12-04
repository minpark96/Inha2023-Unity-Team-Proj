using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if(typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }
        
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        if(original == null )
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if(original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        return Object.Instantiate(original, parent);
    }

    public GameObject PhotonNetworkInstantiate(string path, Transform parent = null, Vector3? pos = null, Quaternion? rot = null, byte group = 0, object[] data = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        //Debug.Log(path);
        pos = pos ?? Vector3.zero;
        rot = rot ?? Quaternion.identity;

        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (prefab.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(prefab, parent).gameObject;

        return PhotonNetwork.Instantiate($"Prefabs/{path}", (Vector3)pos, (Quaternion)rot, group, data);
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        PhotonNetwork.Destroy(go);
    }

}