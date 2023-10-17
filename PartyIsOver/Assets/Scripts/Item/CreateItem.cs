using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateItem : MonoBehaviour
{
    GameObject _cube;
    void Start()
    {
        _cube = Managers.Resrouce.Instantiate("Cube");

        Destroy(_cube, 3.0f);
    }

}
