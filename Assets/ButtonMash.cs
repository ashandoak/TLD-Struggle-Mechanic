using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonMash : MonoBehaviour {

    private bool struggleWon = false;
    private bool unlockKickPush = false;
    private bool menuComplete = false;
    private bool buttonPressed = false;

    private string modeChosen;
    private int buttonPresses = 0;
    private int lastRunStop;
    private int currentRun;

    private float barIncrease = 0.035f;
    private float barDecrease = 0.003f;
    private float totalTime = 0;
    private float kickPushPostedTime;

    public float minFrequency;
    public float minTime;
    public float minConstFrequencyLength;
    public int minRhythmCount;
    public float randMinPresses;
    public float randRollValue;
    public float kickPushInterval;

    private float barDisplay = 0;
    private float lastButtonPressedTime = 0f;
    private float newButtonPressedTime = 0f;
    

    List<float> speedDeltas = new List<float>();

    Vector2 pos = new Vector2(20,20);
    Vector2 size = new Vector2(600,20);
    Vector2 buttonSize = new Vector2(100, 40);


    // Use this for initialization
    void Start () {

        
	}

    public void menuOption(string option)
    {
        Debug.Log(option);
        modeChosen = option;
        menuComplete = true;
        struggleWon = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape")) {
            Application.Quit();
        }
   
        if (menuComplete)
        {
            toggleMenu(false);

            if (Input.GetButtonDown("Fire1") && !buttonPressed)
            {
                buttonPressed = true;
                buttonPresses++;
                handlePress();       
            }                        

            if (barDisplay > 0)
            {
                barDisplay -= barDecrease;
            }

            if (size.x * barDisplay >= size.x || (unlockKickPush && Input.GetAxis("LeftTrigger") < 0))
            {
                struggleWon = true;
                menuComplete = false;
                toggleMenu(true);
                unlockKickPush = false;
            }

         

            if (Input.GetButtonUp("Fire1"))
            {
                buttonPressed = false;
            }


        }
        
    }

    private void handlePress()
    {
        lastButtonPressedTime = newButtonPressedTime;
        newButtonPressedTime = Time.time;
        float deltaTime = newButtonPressedTime - lastButtonPressedTime;

        totalTime += deltaTime;

        if (modeChosen == "minSpeed")
        {
            currentRun = buttonPresses - lastRunStop;

            if (totalTime < 1.0f)
            {
                if (currentRun >= minFrequency && !unlockKickPush)
                {
                    unlockKickPush = true;
                    kickPushPostedTime = Time.time;
                }
            }
            else
            {
                totalTime = 0;
                currentRun = 0;
                lastRunStop = buttonPresses;
            }

        }

        if (modeChosen == "constSpeed")
        {
            if (deltaTime < minTime)
            {
                speedDeltas.Add(deltaTime);
                if (speedDeltas.Count > minConstFrequencyLength && !unlockKickPush)
                {
                    speedDeltas.Clear();
                    unlockKickPush = true;
                    kickPushPostedTime = Time.time;
                }

            }
            else
            {
                speedDeltas.Clear();

            }
        }

        if (modeChosen == "rhythm")
        {
            if (speedDeltas.Count < 1)
            {
                if (deltaTime < 2.0f)
                {
                    speedDeltas.Add(deltaTime);
                }
            }
            else
            {
                if (deltaTime < speedDeltas[speedDeltas.Count - 1])
                {
                    speedDeltas.Add(deltaTime);
                }
                else
                {
                    speedDeltas.Clear();
                }
            }

            if (speedDeltas.Count >= minRhythmCount && !unlockKickPush)
            {
                speedDeltas.Clear();
                unlockKickPush = true;
                kickPushPostedTime = Time.time;
            }
        }


        if (modeChosen == "random")
        {
            speedDeltas.Add(deltaTime);
            if (speedDeltas.Count > randMinPresses)
            {
                float rv = Random.value;
                if (rv < randRollValue && !unlockKickPush)
                {
                    unlockKickPush = true;
                    kickPushPostedTime = Time.time;
                    speedDeltas.Clear();
                } else
                {
                    speedDeltas.Clear();
                }
            }
        }

        barDisplay += barIncrease;
        
    }

    private void toggleMenu(bool state)
    {
        CanvasGroup c = GameObject.Find("Canvas").GetComponent(typeof(CanvasGroup)) as CanvasGroup;
        if (state) {
            c.interactable = true;
            //c.alpha = 1;
        } else
        {
            c.interactable = false;
            //c.alpha = 0;
        }
    }
    void OnGUI()
    {

        if (menuComplete)
        {

            if (!struggleWon)
            {
                GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
                GUI.Box(new Rect(0, 0, size.x, size.y), "");

                // draw the filled-in part:
                GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay, size.y));
                GUI.Box(new Rect(0, 0, size.x, size.y), "");
                GUI.EndGroup();

                GUI.EndGroup();
            }

            if (unlockKickPush && !struggleWon)
            {
                if (Time.time - kickPushPostedTime < kickPushInterval)
                {
                    GUI.TextField(new Rect(pos.x * 10, pos.y * 3, buttonSize.x, buttonSize.y), "Push LT");
                } else
                {
                    unlockKickPush = false;
                }               
            }
        }

        if (struggleWon)
        {
            GUI.TextField(new Rect(pos.x, pos.y, size.x, size.y), "You have repelled the wolf");
            barDisplay = 0;
            lastButtonPressedTime = 0;
            newButtonPressedTime = 0;
            buttonPresses = 0;
            currentRun = 0;
        }
    }
}
