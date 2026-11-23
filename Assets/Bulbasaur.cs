using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bulbasaur : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    public Transform Target;
    private Animator Anim;
    private bool IsMoving;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Anim = GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = Target.position;

        float currentMovementSpeed = agent.velocity.magnitude;
        IsMoving = currentMovementSpeed > 0.1f;

        Anim.SetBool("IsMoving", IsMoving);
    

    }

    private void LateUpdate()
    {
        if (!IsMoving)
        {
            gameObject.transform.GetChild(0).transform.eulerAngles = new Vector3(0, gameObject.transform.GetChild(0).transform.eulerAngles.y, 0);

        }
    }
}

