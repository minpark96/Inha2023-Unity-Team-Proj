using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EffectManager
{
    public GameObject Instantiate(ParticleSystem path, Vector3 pos, Quaternion rotation , Transform parent = null)
    {
        GameObject prefab = Managers.Resource.Load<GameObject>($"{path}");

        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return Object.Instantiate(prefab, pos, rotation);
    }


    //포톤에서 중간에 Instantiate를 할 수 있는지 확인 해봐야함
    public GameObject PhotonNetworkEffectInstantiate(string path, Transform parent = null, Vector3? pos = null, Quaternion? rot = null, byte group = 0, object[] data = null)
    {
        GameObject prefab = Managers.Resource.Load<GameObject>($"Effects/{path}");
        pos = pos ?? Vector3.zero;
        rot = rot ?? Quaternion.identity;

        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return PhotonNetwork.Instantiate($"Effects/{path}", (Vector3)pos, (Quaternion)rot, group, data);
    }

    public void Play(string path, Vector3 pos, Vector3 normal, Transform parent = null, Define.Effect effect = Define.Effect.PlayerEffect)
    {
        //기본적으로는 플레이어나 장나감총 이펙트를 많이 사용하므로 일단 만들어
        var targetPrefab = Managers.Resource.Load<ParticleSystem>(path);

        if (effect == Define.Effect.UIEffect)
        {
            targetPrefab = Managers.Resource.Load<ParticleSystem>(path);
        }

        var instantiatedObject = Instantiate(targetPrefab, pos, Quaternion.LookRotation(normal));

        ParticleSystem playEffect = instantiatedObject.GetComponent<ParticleSystem>();

        if (parent != null) playEffect.transform.SetParent(parent);

        playEffect.Play();
    }



}
