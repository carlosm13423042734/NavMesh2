using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class Boton : MonoBehaviour
{
    public GameObject puente;
    public NavMeshSurface navMesh;

    void Start()
    {
        if (puente != null)
            puente.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (puente != null)
                puente.SetActive(true);

            if (navMesh != null)
                navMesh.BuildNavMesh(); 
        }
    }
}
