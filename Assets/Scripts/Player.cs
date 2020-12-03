using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    Animator _gunAnimator;
    [SerializeField] RunTimeData data;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _mouseSensitivity;
    [SerializeField] int _damage;
    [SerializeField] int _startHealth;
    int _health;
    float _currentTilt;
    Camera _cam;
    CapsuleCollider collider;
    CharacterController controller;
    Vector3 lastMousePos;
    bool _dead;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.PlayerRespawn += OnPlayerRespawn;
        GameEvents.PlayerShot += OnPlayerShot;

        _health = _startHealth;
        _gunAnimator = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        _cam = Camera.main;
        collider = transform.GetComponent<CapsuleCollider>();
        Vector3 lastMousePos = Input.mousePosition;
        controller = transform.GetComponent<CharacterController>();
        _dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Gravity();

        Aim();

        if (Input.GetMouseButtonDown(0) && !_gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fire")) 
        {
            _gunAnimator.SetTrigger("Fire");
            Shoot();
        }

        if (!_dead && this._health <= 0)
        {
            _dead = true;
            GameEvents.InvokePlayerDead();
        }
    }

    void Aim()
    {
        Vector3 mousePos = Input.mousePosition;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up, mouseX * _mouseSensitivity);

        _currentTilt -= mouseY * _mouseSensitivity;
        _currentTilt = Mathf.Clamp(_currentTilt, -90f, 90f);

        if(!(mousePos == lastMousePos))
            _cam.transform.localEulerAngles = new Vector3(_currentTilt, 0f, 0f);

        lastMousePos = mousePos;
    }

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

    void OnPlayerShot(object sender, EventArgs args) 
    {
        this._health -= data.heroDamage;
    }

    void OnPlayerRespawn(object sender, PositionEventArgs args) 
    {
        this._health = _startHealth;
        _dead = false;
        Vector3 newPos = args.positionPayload;

        this.transform.position = newPos;
    }
}
