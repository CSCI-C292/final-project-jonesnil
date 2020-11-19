using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BadGuy : MonoBehaviour
{
    Animator badGuyAnimator;
    NavMeshAgent agent;
    Vector3 firstStop;
    Vector3 secondStop;
    Vector3 firstStopYourHeight;
    Vector3 secondStopYourHeight;

    // Start is called before the first frame update
    void Start()
    {
        badGuyAnimator = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        firstStop = this.transform.GetChild(6).position;
        secondStop = this.transform.GetChild(7).position;
        agent.SetDestination(firstStop);

    }

    // Update is called once per frame
    void Update()
    {
        float distanceToFirst = (this.transform.position - firstStop).magnitude;
        Debug.Log(distanceToFirst);
        if (distanceToFirst < .1f)
            Invoke("GoToSecond", .5f);

        float distanceToSecond = (this.transform.position - secondStop).magnitude;
        if (distanceToSecond < .1f)
            Invoke("GoToFirst", .5f);

        badGuyAnimator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void GoToSecond() 
    {
        agent.SetDestination(secondStop);
    }

    void GoToFirst() 
    {
        agent.SetDestination(firstStop);
    }
}
