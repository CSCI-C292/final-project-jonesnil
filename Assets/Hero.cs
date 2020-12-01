using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hero : MonoBehaviour
{
    Animator heroAnimator;
    NavMeshAgent agent;
    Vector3 goal;
    [SerializeField] GameObject player;
    CapsuleCollider collider;
    [SerializeField] int health;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.HeroShot += OnHeroShot;

        heroAnimator = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        goal = this.transform.GetChild(9).transform.position;
        collider = this.GetComponent<CapsuleCollider>();
        agent.SetDestination(goal);
    }

    // Update is called once per frame
    void Update()
    {
        heroAnimator.SetFloat("Speed", agent.velocity.magnitude);
        if (this.health <= 0) 
        {
            agent.isStopped = true;
            heroAnimator.SetBool("Dead", true);
        }
        else
            TryToShootPlayer();
    }

    void TryToShootPlayer() 
    {
        // This bit is a mouthful, and even more frustrating to write than to read. 
        // It says, "cast a line from me to the player. Give me the first thing you
        // hit, but ignore layer 9 (meaning don't worry if you hit the hero's collider's.)"
        RaycastHit shot;
        Physics.Linecast(transform.position, player.transform.position, out shot, ~(1 << 9));

        // Check if what you shot is the player (player is represented by layer 8.)
        if (shot.collider.gameObject.layer == 8)
        {
            //You found the player! Stop and shoot at them.

            // This bit of code is from helpful user Revolver2k on the unity forums.
            // Wasn't my question but it's really useful, it takes lookAt and only
            // uses rotates on the y axis. 
            // https://answers.unity.com/questions/36255/lookat-to-only-rotate-on-y-axis-how.html

            Vector3 targetPosition = new Vector3(player.transform.position.x,
                                       this.transform.position.y,
                                       player.transform.position.z);

            this.transform.LookAt(targetPosition);

            heroAnimator.SetBool("Shooting", true);
            agent.isStopped = true;
        }
        //If the player isn't in sight...
        else {
            // Check if we're still stopped, and if we are
            // let's get moving to the core.
            if (agent.isStopped == true)
                agent.isStopped = false;

            // Stop shooting doofus they're gone!
            heroAnimator.SetBool("Shooting", false);

        }


    }

    void OnHeroShot(object sender, IntEventArgs args) 
    {
        this.health -= args.intPayload;

    }
}
