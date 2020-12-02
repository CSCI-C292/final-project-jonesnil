using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BadGuy : MonoBehaviour
{
    [SerializeField] RunTimeData data;
    Animator badGuyAnimator;
    NavMeshAgent agent;
    Vector3 firstStop;
    Vector3 secondStop;
    Vector3 thirdStop;
    Vector3 fourthStop;

    // Start is called before the first frame update
    void Start()
    {
        badGuyAnimator = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        firstStop = this.transform.GetChild(6).position;
        secondStop = this.transform.GetChild(7).position;
        thirdStop = this.transform.GetChild(8).position;
        fourthStop = this.transform.GetChild(9).position;
        agent.SetDestination(firstStop);

    }

    // Update is called once per frame
    void Update()
    {
        if (CanShootHero())
        {
            //You found the hero! Stop and shoot at them.
            // (reusing this from the hero class)

            // This bit of code is from helpful user Revolver2k on the unity forums.
            // Wasn't my question but it's really useful, it takes lookAt and only
            // uses rotates on the y axis. 
            // https://answers.unity.com/questions/36255/lookat-to-only-rotate-on-y-axis-how.html

            Vector3 targetPosition = new Vector3(data.heroPos.x,
                                       this.transform.position.y,
                                       data.heroPos.z);

            this.transform.LookAt(data.heroPos);

            badGuyAnimator.SetBool("Shooting", true);
            agent.isStopped = true;
        }
        else 
        {

            // Check if we're still stopped, and if we are
            // let's get moving to the core.
            if (agent.isStopped == true)
                agent.isStopped = false;

            // Stop shooting doofus they're gone!
            badGuyAnimator.SetBool("Shooting", false);


            Patrol();
        }

        badGuyAnimator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void GoToFourth()
    {
        agent.SetDestination(fourthStop);
    }

    void GoToThird()
    {
        agent.SetDestination(thirdStop);
    }

    void GoToSecond() 
    {
        agent.SetDestination(secondStop);
    }

    void GoToFirst() 
    {
        agent.SetDestination(firstStop);
    }

    bool CanShootHero() 
    {
        return Physics.Linecast(transform.position, data.heroPos, (1 << 9));
    }

    void Patrol() 
    {
        float distanceToFirst = (this.transform.position - firstStop).magnitude;
        if (distanceToFirst < .1f)
            Invoke("GoToSecond", .5f);

        float distanceToSecond = (this.transform.position - secondStop).magnitude;
        if (distanceToSecond < .1f)
            Invoke("GoToThird", .5f);

        float distanceToThird = (this.transform.position - thirdStop).magnitude;
        if (distanceToThird < .1f)
            Invoke("GoToFourth", .5f);

        float distanceToFourth = (this.transform.position - fourthStop).magnitude;
        if (distanceToFourth < .1f)
            Invoke("GoToFirst", .5f);
    }
}
