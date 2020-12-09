using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Hero : MonoBehaviour
{
    [SerializeField] RunTimeData data;
    [SerializeField] int damage;
    Vector3 shootingAt;
    Animator heroAnimator;
    NavMeshAgent agent;
    Vector3 goal;
    CapsuleCollider collider;
    [SerializeField] int health;
    AudioSource pistolShotSound;
    bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.HeroShot += OnHeroShot;
        GameEvents.GameOver += OnGameOver;

        heroAnimator = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        goal = this.transform.GetChild(9).transform.position;
        collider = this.GetComponent<CapsuleCollider>();
        agent.SetDestination(goal);
        pistolShotSound = transform.GetComponent<AudioSource>();
        
        gameOver = false;
        data.heroDamage = damage;
    }

    // Update is called once per frame
    void Update()
    {
        data.heroPos = this.transform.position;

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
        bool shotAnything = Physics.Linecast(transform.position, Camera.main.transform.position, out shot, ~(1 << 9));

        // Check if what you shot is the player (player is represented by layer 8.)
        if (shotAnything && shot.collider.gameObject.layer == 8 && !gameOver)
        {
            //You found the player! Stop and shoot at them.

            // This bit of code is from helpful user Revolver2k on the unity forums.
            // Wasn't my question but it's really useful, it takes lookAt and only
            // uses rotates on the y axis. 
            // https://answers.unity.com/questions/36255/lookat-to-only-rotate-on-y-axis-how.html

            ShootAt(Camera.main.transform.position);

        }
        //If the player isn't in sight...
        else {
            //Try shooting at the nearest bad guy
            bool shotBadGuy = Physics.Linecast(transform.position, data.nearestBadGuyToHero, ~((1 << 9) | (1 << 10)));

            //Ignore the one that means no bad guy is left
            if (Vector3.Equals(data.nearestBadGuyToHero, new Vector3(0f, 0f, 0f))) 
            {
                shotBadGuy = true;
            }

            if (!shotBadGuy)
            {
                ShootAt(data.nearestBadGuyToHero);
            }
            else
            {

                // Check if we're still stopped, and if we are
                // let's get moving to the core.
                if (agent.isStopped == true)
                    agent.isStopped = false;

                // Stop shooting doofus they're gone!
                heroAnimator.SetBool("Shooting", false);
            }
        }


    }

    public void HurtWhatYoureShooting() 
    {
        RaycastHit shot;
        bool shotAnything = Physics.Linecast(transform.position, shootingAt, out shot, ~(1 << 9));

        pistolShotSound.Play();

        if(shotAnything && shot.transform.gameObject.layer == 8)
        {
            GameEvents.InvokePlayerShot();
        }

        bool shotBadGuy = Physics.Linecast(transform.position, shootingAt, ~((1 << 9) | (1 << 10)));
        if (!shotBadGuy) 
        {
            GameEvents.InvokeNearestBadGuyShot();
        }
    }

    public void ShootAt(Vector3 position) 
    {

        Vector3 targetPosition = new Vector3(position.x,
                                   this.transform.position.y,
                                   position.z);

        this.transform.LookAt(targetPosition);

        shootingAt = position;
        heroAnimator.SetBool("Shooting", true);
        agent.isStopped = true;
    }

    void OnHeroShot(object sender, IntEventArgs args) 
    {
        heroAnimator.SetBool("Shot", true);
        this.health -= args.intPayload;

    }

    void OnGameOver(object sender, EventArgs args)
    {
        gameOver = true;


        GameEvents.HeroShot -= OnHeroShot;
        GameEvents.GameOver -= OnGameOver;
    }
}
