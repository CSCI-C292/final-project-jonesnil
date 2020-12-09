using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] float turnSpeed;
    [SerializeField] float moveSpeed;
    bool moveToHelp;
    bool moveToStart;

    Text titleText;
    Button startButton;
    Text startButtonText;
    Button helpButton;
    Text helpButtonText;
    Button backButton;
    Text backButtonText;
    Image helpTextBackgroundOne;
    Text helpTextOne;
    Image helpTextBackgroundTwo;
    Text helpTextTwo;


    // Start is called before the first frame update
    void Start()
    {
        moveToHelp = false;
        moveToStart = false;
        titleText = transform.GetChild(0).GetComponent<Text>();
        startButton = transform.GetChild(1).GetComponent<Button>();
        startButtonText = startButton.transform.GetChild(0).GetComponent<Text>();
        helpButton = transform.GetChild(2).GetComponent<Button>();
        helpButtonText = helpButton.transform.GetChild(0).GetComponent<Text>();
        backButton = transform.GetChild(3).GetComponent<Button>();
        backButtonText = backButton.transform.GetChild(0).GetComponent<Text>();
        helpTextBackgroundOne = transform.GetChild(4).GetComponent<Image>();
        helpTextOne = transform.GetChild(5).GetComponent<Text>();
        helpTextBackgroundTwo = transform.GetChild(6).GetComponent<Image>();
        helpTextTwo = transform.GetChild(7).GetComponent<Text>();
        SetHelpUI(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (moveToHelp) 
        {
            TurnLeft();
            MoveForward();
        }

        if (moveToStart)
        {
            TurnRight();
            MoveBackward();
        }
    }

    void TurnLeft() 
    {
        Vector3 dummyRot = Camera.main.transform.rotation.eulerAngles;
        float yRot = dummyRot.y;

        dummyRot.y = Mathf.Lerp(yRot, 90, turnSpeed);

        Camera.main.transform.rotation = Quaternion.Euler(dummyRot);
    }

    void MoveForward() 
    {
        Vector3 dummyPos = Camera.main.transform.position;
        float xPos = dummyPos.x;

        dummyPos.x = Mathf.Lerp(xPos, -11.5f, moveSpeed);

        Camera.main.transform.position = dummyPos;
    }

    void TurnRight()
    {
        Vector3 dummyRot = Camera.main.transform.rotation.eulerAngles;
        float yRot = dummyRot.y;

        dummyRot.y = Mathf.Lerp(yRot, 180, turnSpeed);

        Camera.main.transform.rotation = Quaternion.Euler(dummyRot);
    }

    void MoveBackward()
    {
        Vector3 dummyPos = Camera.main.transform.position;
        float xPos = dummyPos.x;

        dummyPos.x = Mathf.Lerp(xPos, -21.5f, moveSpeed);

        Camera.main.transform.position = dummyPos;
    }

    void SetStartUI(bool setting) 
    {
        titleText.enabled = setting;
        startButton.image.enabled = setting;
        startButton.enabled = setting;
        startButtonText.enabled = setting;
        helpButton.image.enabled = setting;
        helpButton.enabled = setting;
        helpButtonText.enabled = setting;
    }

    void SetHelpUI(bool setting) 
    {
        backButton.image.enabled = setting;
        backButton.enabled = setting;
        backButtonText.enabled = setting;
        helpTextBackgroundOne.enabled = setting;
        helpTextOne.enabled = setting;
        helpTextBackgroundTwo.enabled = setting;
        helpTextTwo.enabled = setting;
    }

    public void HelpButtonPressed() 
    {
        moveToStart = false;
        moveToHelp = true;

        SetStartUI(false);
        SetHelpUI(true);
    }

    public void BackButtonPressed() 
    {
        moveToStart = true;
        moveToHelp = false;

        SetStartUI(true);
        SetHelpUI(false);
    }

    public void StartButtonPressed() 
    {
        SceneManager.LoadScene("Level1");
    }
}
