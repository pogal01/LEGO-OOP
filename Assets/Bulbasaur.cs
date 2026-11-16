using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bulbasaur : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    public Transform Target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = Target.position;
    }
}
