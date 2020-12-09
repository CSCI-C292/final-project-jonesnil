using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class BadGuy : MonoBehaviour
{
    [SerializeField] RunTimeData data;
    [SerializeField] int health;
    [SerializeField] int damage;
    CapsuleCollider badGuyCollider;
    Animator badGuyAnimator;
    NavMeshAgent agent;
    Vector3 firstStop;
    Vector3 secondStop;
    Vector3 thirdStop;
    Vector3 fourthStop;
    public bool nearest;
    public bool dead;
    AudioSource pistolShotSound;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.NearestBadGuyShot += OnNearestBadGuyShot;
        GameEvents.GameOver += OnGameOver;

        badGuyAnimator = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        firstStop = this.transform.GetChild(6).position;
        secondStop = this.transform.GetChild(7).position;
        thirdStop = this.transform.GetChild(8).position;
        fourthStop = this.transform.GetChild(9).position;
        badGuyCollider = this.GetComponent<CapsuleCollider>();
        agent.SetDestination(firstStop);
        nearest = false;
        pistolShotSound = transform.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (this.health <= 0) 
        {
            this.dead = true;
            this.badGuyAnimator.SetBool("Dead", true);
            agent.isStopped = true;
        }

        if (!dead)
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
                // let's get back to patrolling.
                if (agent.isStopped == true)
                    agent.isStopped = false;

                // Stop shooting doofus they're gone!
                badGuyAnimator.SetBool("Shooting", false);

                if(this.agent.enabled)
                    Patrol();
            }

            badGuyAnimator.SetFloat("Speed", agent.velocity.magnitude);
        }
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
        return !Physics.Linecast(transform.position, data.heroPos, ~((1 << 9) |  (1 << 10)));
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

    void OnNearestBadGuyShot(object sender, EventArgs args) 
    {
        if (this.nearest) 
        {
            this.health -= data.heroDamage;
        }
    }

    public void HurtWhatYoureShooting() 
    {
        pistolShotSound.Play();
        GameEvents.InvokeHeroShot(damage);
    }

    public void DieInvisible() 
    {
        this.badGuyCollider.enabled = false;
        this.agent.enabled = false;
        this.transform.position = new Vector3(100f, -20f, 100f);
        this.dead = true;
        this.badGuyAnimator.SetBool("Dead", true);
    }

    void OnGameOver(object sender, EventArgs args)
    {
        GameEvents.NearestBadGuyShot -= OnNearestBadGuyShot;
        GameEvents.GameOver -= OnGameOver;
    }

}
