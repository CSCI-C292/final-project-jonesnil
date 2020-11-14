using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    Animator heroAnimator;

    // Start is called before the first frame update
    void Start()
    {
        heroAnimator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        heroAnimator.SetFloat("Speed", 1f);

        heroAnimator.SetBool("Shooting", false);

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            heroAnimator.SetBool("Shooting", true);
        }

        
    }
}
