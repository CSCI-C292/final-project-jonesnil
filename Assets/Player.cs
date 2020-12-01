using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator _gunAnimator;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _mouseSensitivity;
    [SerializeField] int _damage;
    float _currentTilt;
    Camera _cam;
    CapsuleCollider collider;
    CharacterController controller;
    Vector3 lastMousePos;

    // Start is called before the first frame update
    void Start()
    {
        _gunAnimator = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        _cam = Camera.main;
        collider = transform.GetComponent<CapsuleCollider>();
        Vector3 lastMousePos = Input.mousePosition;
        controller = transform.GetComponent<CharacterController>();
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

        if (Physics.Raycast(shootRay, 100f, 1 << 9)) 
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

}
