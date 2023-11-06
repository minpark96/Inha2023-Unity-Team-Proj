using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
        
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");

        if( prefab == null )
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return Object.Instantiate(prefab, parent);
    }

    public GameObject PhotonNetworkInstantiate(string path, Transform parent = null, Vector3? pos = null, Quaternion? rot = null, byte group = 0, object[] data = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        pos = pos ?? Vector3.zero;
        rot = rot ?? Quaternion.identity;

        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        
        return PhotonNetwork.Instantiate($"Prefabs/{path}", (Vector3)pos, (Quaternion)rot, group, data);
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }

}