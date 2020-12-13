using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

// The pathfinding on this class is pretty much braindead. I just set it to keep four points 
// I gave it in the editor and cycle them until something shows up it can shoot/ the player
// kills it to respawn.
public class BadGuy : MonoBehaviour
{
    [SerializeField] RunTimeData data;
    [SerializeField] int health;
    int damage;
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
        this.damage = 1;

    }

    // Update is called once per frame
    void Update()
    {
        if (this.health <= 0) 
        {
            this.nearest = false;
            this.dead = true;
            this.badGuyAnimator.SetBool("Dead", true);
            if (agent.enabled)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }
        }

        // This updates the RunTimeData object so the hero always knows where the closest
        // bad guy to him is.
        if (nearest) 
        {
            data.nearestBadGuyToHero = this.transform.position;
        }

        badGuyAnimator.SetFloat("Speed", agent.velocity.magnitude);

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

                Vector3 targetPosition = IgnoreY(data.heroPos);

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

        }
    }

    Vector3 IgnoreY(Vector3 target) 
    {
        return new Vector3(target.x, this.transform.position.y, target.z);
    }

    void GoToFourth()
    {
        if (this.agent.enabled)
            agent.SetDestination(fourthStop);
    }

    void GoToThird()
    {
        if (this.agent.enabled)
            agent.SetDestination(thirdStop);
    }

    void GoToSecond() 
    {
        if (this.agent.enabled)
            agent.SetDestination(secondStop);
    }

    void GoToFirst() 
    {
        if (this.agent.enabled)
            agent.SetDestination(firstStop);
    }

    bool CanShootHero() 
    {
        return !Physics.Linecast(transform.position, data.heroPos, ~((1 << 9) |  (1 << 10)));
    }

    // Checks how close the bad guy is to the patrol points and if it's on top of one it changes
    // the navmesh destination.
    void Patrol() 
    {
        float distanceToFirst = (this.transform.position - firstStop).magnitude;
        if (distanceToFirst < .1f)
            GoToSecond();

        float distanceToSecond = (this.transform.position - secondStop).magnitude;
        if (distanceToSecond < .1f)
            GoToThird();

        float distanceToThird = (this.transform.position - thirdStop).magnitude;
        if (distanceToThird < .1f)
            GoToFourth();

        float distanceToFourth = (this.transform.position - fourthStop).magnitude;
        if (distanceToFourth < .1f)
            GoToFirst();
    }

    // This does bad guy damage. Admittedly it's a little wasteful performance wise, because
    // it will be called on every bad guy and only do anything for the nearest one (that is
    // always the one the hero shoots).
    void OnNearestBadGuyShot(object sender, EventArgs args) 
    {
        if (this.nearest) 
        {
            this.health -= data.heroDamage;
        }
    }

    // This does less logic work than the hero class's version, because the bad guys only ever shoot
    // the hero. It's still called by the same frame of the shooting animation.
    public void HurtWhatYoureShooting() 
    {
        pistolShotSound.Play();
        GameEvents.InvokeHeroShot(damage);
    }

    // This is only called when the player died and wants to respawn as this bad guy.
    // It's called in StateManager and it just disables the bad guy's collider and navmesh agent
    // and puts the corpse where the player died.
    public void DieAtPosition(Vector3 position) 
    {
        this.agent.isStopped = true;
        this.nearest = false;
        this.dead = true;
        this.badGuyCollider.enabled = false;
        this.agent.enabled = false;
        this.health = 0;
        this.transform.position = IgnoreY(position);
        this.transform.rotation = Quaternion.Euler(IgnoreY(Camera.main.transform.rotation.eulerAngles));
    }

    // Disconnect events so game can reload properly
    void OnGameOver(object sender, BoolEventArgs args)
    {
        GameEvents.NearestBadGuyShot -= OnNearestBadGuyShot;
        GameEvents.GameOver -= OnGameOver;
    }

}
