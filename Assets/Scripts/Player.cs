using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// The aim and move code for this project is pretty much identical to the 3D tutorial from class.

public class Player : MonoBehaviour
{
    Animator _gunAnimator;
    [SerializeField] RunTimeData data;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _mouseSensitivity;
    [SerializeField] int _damage;
    [SerializeField] int _startHealth;
    [SerializeField] float _jumpHeight;
    float jumpGoal;
    bool jumping;
    int _health;
    float _currentTilt;
    Camera _cam;
    CapsuleCollider collider;
    CharacterController controller;
    Vector3 lastMousePos;
    bool _dead;
    AudioSource pistolShotSound;

    void Awake()
    {
        GameEvents.PlayerRespawn += OnPlayerRespawn;
        GameEvents.PlayerShot += OnPlayerShot;
        GameEvents.GameOver += OnGameOver;

        Cursor.lockState = CursorLockMode.Locked;
        _health = _startHealth;
        _gunAnimator = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        _cam = Camera.main;
        collider = transform.GetComponent<CapsuleCollider>();
        Vector3 lastMousePos = Input.mousePosition;
        controller = transform.GetComponent<CharacterController>();
        _dead = false;
        pistolShotSound = transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Gravity();

        // These two bits deal with jumping. I want the jump to seem smooth,
        // so I basically just give the player negative gravity when they hit spacebar
        // and then when they reach a certain height over where they started it turns off
        // and they fall again. Obviously they have to be on the ground to jump, though.

        if (Input.GetKeyDown("space") && controller.isGrounded) 
        {
            Debug.Log("Jump");
            jumping = true;
            jumpGoal = this.transform.position.y + _jumpHeight;
        }

        if (jumping) 
        {
            if (this.transform.position.y >= jumpGoal)
            {
                jumping = false;
            }
            else
                Jump();
        }


        // If you click the left mouse button, and your gun isn't already firing, and you're not dead,
        // do the shoot stuff.
        if (Input.GetMouseButtonDown(0) && !_gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fire") && !_dead) 
        {
            pistolShotSound.Play();
            _gunAnimator.SetTrigger("Fire");
            Shoot();
        }
    }

    private void LateUpdate()
    {

        Aim();

        // This is done once each time the player is killed. It points them to the hero
        // and invokes the dead event so statemanager can send it a bad guy location to spawn at.
        // The looking at the hero function doesn't work well at the moment though, but I didn't 
        // think it was 100% crucial to the project.

        if (!_dead && this._health <= 0)
        {

            Vector3 targetPosition = new Vector3(data.heroPos.x,
                                           this.transform.position.y,
                                           data.heroPos.z);
            transform.right = (targetPosition - this.transform.position).normalized * -1;

            controller.enabled = false;
            _dead = true;
            GameEvents.InvokePlayerDead();
        }
    }

    void Aim()
    {
        if (!_dead)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up, mouseX * _mouseSensitivity);

            _currentTilt -= mouseY * _mouseSensitivity;
            _currentTilt = Mathf.Clamp(_currentTilt, -90f, 90f);
            _cam.transform.localEulerAngles = new Vector3(_currentTilt, 0f, 0f);
        }
    }

    // Does a raycast to check if you shot the hero, invokes an event if you did.
    void Shoot() 
    {
        Vector3 centerScreen = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        Ray shootRay = new Ray(centerScreen, cameraForward);
        RaycastHit shot;
        bool shotAnything = Physics.Raycast(shootRay, out shot);

        if (shotAnything && shot.transform.gameObject.layer == 9) 
        {
            GameEvents.InvokeHeroShot(_damage);
        }
    }

    void Movement()
    {
        Vector3 sidewaysMovement = Input.GetAxis("Horizontal") * transform.right;
        Vector3 forwardsMovement = Input.GetAxis("Vertical") * transform.forward;
        Vector3 movementVector = sidewaysMovement + forwardsMovement;
        movementVector.Normalize();


        controller.Move(movementVector * _moveSpeed * Time.deltaTime);
    }

    void Gravity()
    {
        controller.Move(new Vector3(0, -5f * Time.deltaTime, 0));
    }

    void Jump()
    {
        controller.Move(new Vector3(0, 10f * Time.deltaTime, 0));
    }


    void OnPlayerShot(object sender, EventArgs args) 
    {
        this._health -= data.heroDamage;

    }

    // Moves player to new position and resets health.
    void OnPlayerRespawn(object sender, PositionEventArgs args) 
    {
        this._health = _startHealth;
        _dead = false;
        Vector3 newPos = args.positionPayload;
        this.transform.position = newPos;

        controller.enabled = true;
    }

    // Disconnect events so game can reload properly. Also unlocks cursor
    // so you can click buttons.
    void OnGameOver(object sender, BoolEventArgs args)
    {
        Cursor.lockState = CursorLockMode.None;

        GameEvents.PlayerRespawn -= OnPlayerRespawn;
        GameEvents.PlayerShot -= OnPlayerShot;
        GameEvents.GameOver -= OnGameOver;
    }
}
