using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RunLogic : MonoBehaviour {
    public enum GameState
    {
        GameIdle,
        GameStartState,
        GameEndState
    };

    ConnectionScript conn;
    public GameState state = GameState.GameIdle;
    private GUIStyle style;
    private FusionRotator userFusionRotator;
    private FusionRotator userFusionRotator2;

    Text flashingText;
    string blinkingText = "";
    bool isBlinking = false;
    bool gameStarted = false;

    // Use this for initialization
    void Start () {

        flashingText = GameObject.Find("Canvas/Text").GetComponent<Text>();

        conn = GameObject.Find("sensorfusion").GetComponent<ConnectionScript>();
        userFusionRotator = GameObject.Find("kionix_iot").GetComponent<FusionRotator>();
        userFusionRotator2 = GameObject.Find("kionix_iot_2").GetComponent<FusionRotator>();
        //print("userCubeFusionRotator " + userFusionRotator);

        int w = Screen.width, h = Screen.height;
        style = new GUIStyle();
        
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;                
        style.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 1f);

        state = GameState.GameIdle;

        StartCoroutine(BlinkText());
    }

    public IEnumerator BlinkText()
    {
        while (isBlinking)
        {
            flashingText.text = "";
            yield return new WaitForSeconds(.5f);

            if (isBlinking == false)
                break;

            flashingText.text = blinkingText;
            yield return new WaitForSeconds(.5f);
        }
    }

    public IEnumerator StopBlink()
    {
        
        yield return new WaitForSeconds(.5f);
        isBlinking = false;
    }

    void OnGUI()
    {
        if (state == GameState.GameIdle)
        {
            if (conn.connected)
            {
                if (!gameStarted)
                {
                    blinkingText = "Press space to reset orientation";                    
                }
                else
                {
                    blinkingText = "";
                }
            }
            else
            {
                blinkingText = "Press connect button";
            }

            if (isBlinking == false)
            {   
                isBlinking = true;
                StartCoroutine(BlinkText());
            }
        }
        else if (state == GameState.GameStartState)
        {
            state = GameState.GameStartState;
            blinkingText = "";
        }
    }

    // Update is called once per frame
    void Update () {

        if (conn.connected && Input.GetKeyUp("space"))
        {
            if (state == GameState.GameIdle)
            {                
                state = GameState.GameStartState;
                gameStarted = true;

                userFusionRotator.lockPosition = true;
                userFusionRotator2.lockPosition = true;
            }
            else
            {
                state = GameState.GameIdle;
            }
        }
    }    
}