using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");

        DontDestroyOnLoad(gameObject);

        if (GameObject.FindGameObjectWithTag("Enemy") != null)
        {

        }
    }
}
