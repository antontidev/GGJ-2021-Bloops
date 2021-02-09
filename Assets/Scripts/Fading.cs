using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour
{
    private Material mat;

    void Start()
    {
        mat = gameObject.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        Color newColor = mat.color;
        newColor.a -= Time.deltaTime;
        mat.color = newColor;
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }
    void OnTriggerEnter(Collider other)
    {
        if (mat.color.a > 0.4f)
        {
            Color newColor = mat.color;
            newColor.a -= Time.deltaTime;
            mat.color = newColor;
            gameObject.GetComponent<MeshRenderer>().material = mat;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (mat.color.a < 1f)
        {
            Color newColor = mat.color;
            newColor.a += Time.deltaTime;
            mat.color = newColor;
            gameObject.GetComponent<MeshRenderer>().material = mat;
        }
    }
}
