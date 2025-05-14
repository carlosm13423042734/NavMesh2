using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Jugador : MonoBehaviour
{
    public Vector3 destino;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NavMeshAgent>().SetDestination(destino);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 1000)) { 
                GetComponent<NavMeshAgent>().SetDestination(hit.point);
            }
       }
    }
}
