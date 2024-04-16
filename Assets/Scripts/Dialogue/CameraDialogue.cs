using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using TMPro;
using UnityEngine;

public class CameraDialogue : MonoBehaviour
{
    [Header("Camera Settings")]

    [SerializeField] private float followSpeed = 5f;

    [Header("Dialogue Settings")]

    public TextMeshProUGUI dialogueText;

    public TextMeshProUGUI speakerNameText;

    [SerializeField] private float textSpeed = 0.05f;

    [Header("Verifications")]

    [SerializeField] private bool cameraFlag;

    [SerializeField] public bool stop;

    [SerializeField] public bool enqueued;

    [SerializeField] private bool textEnded;

    [SerializeField] private bool startingText;
    
    [SerializeField] public bool reachedInitialPos;

    [SerializeField] private bool isPositionCorrect;

    [SerializeField] private bool isRotationCorrect;

    [SerializeField] public bool canStart;


    private Vector3 initialCameraPosition;
    private Vector3 initialCameraRotation;

    [Header("Assigned Elements")]

    public GameObject currentFocus;

    public GameObject currentCharacter;

    public GameObject dialogueUI;

    public GameObject dialogueEndIndicator;

    public DialogueInitScript dialogueInit;

    public GameData gameManager;

    public TopDownCameraController topDownCameraController;

    [System.Serializable]
    public class DialogueEntry
    {
        public GameObject character;

        public GameObject focus;
        public string dialogueText;
        public Emotion emotion;

        public enum Emotion { Neutral, Happy, Sad, Angry }

        public DialogueEntry(GameObject character, string dialogueText, Emotion emotion)
        {
            this.character = character;
            this.focus = character;
            this.dialogueText = dialogueText;
            this.emotion = emotion;
        }

        public DialogueEntry(GameObject character, GameObject focus, string dialogueText, Emotion emotion)
        {
            this.character = character;
            this.focus = focus;
            this.dialogueText = dialogueText;
            this.emotion = emotion;
        }
    }

    private Queue<DialogueEntry> dialogueEntries = new Queue<DialogueEntry>();

    public DialogueEntry currentEntry;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
    }

    void Start()
    {
        dialogueUI = GameObject.FindWithTag("Dialogue Manager");
        dialogueEndIndicator = GameObject.FindWithTag("Dialogue End Indicator");
        //CycleThroughDialogue();
        cameraFlag = false;

        initialCameraPosition = transform.position;
        initialCameraRotation = transform.rotation.eulerAngles;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.gameState == GameData.GameState.Cutscene)
        {
            stop = true;
            dialogueUI.SetActive(false);
            dialogueEndIndicator.SetActive(false);
        }

        if (dialogueInit.asigned && !enqueued)
        {
            foreach (DialogueEntry element in dialogueInit.dialogueStart)
            {
                dialogueEntries.Enqueue(element);
            }
            enqueued = true;
        }

        if (gameManager.gameState == GameData.GameState.Cutscene && !canStart)
        {
            isPositionCorrect = Vector3.Distance(transform.position, initialCameraPosition) < 0.3f;

            if (!isPositionCorrect)
            {
                transform.position = Vector3.Lerp(transform.position, initialCameraPosition, Time.deltaTime * followSpeed);
            }

            Quaternion targetRotation = Quaternion.Euler(initialCameraRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.2f)
            {
                isRotationCorrect = true;
            }
        }

        if (isPositionCorrect && isRotationCorrect)
        {
            canStart = true;
        }

        if (gameManager.gameState == GameData.GameState.Cutscene && !startingText && dialogueInit.asigned && canStart)
        {
            CycleThroughDialogue();
            startingText = true;
        }

        if (gameManager.gameState == GameData.GameState.Cutscene && canStart)
        {
            if (Input.GetKeyDown(gameManager.GetValidKeyPress()) && textEnded && !stop && cameraFlag == true)
            {
                CycleThroughDialogue();
                cameraFlag = false;
            }
            else if (Input.GetKeyDown(gameManager.GetValidKeyPress()) && !textEnded)
            {
                textSpeed = 0.01f;
            }

            if (textEnded && cameraFlag == true && !stop)
            {
                dialogueEndIndicator.SetActive(true);
            }

            if (textEnded)
            {
                WiggleText(dialogueText);
            }
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.gameState == GameData.GameState.Cutscene && canStart) 
        {
            CameraMove();
        }

        if (stop && !reachedInitialPos)
        {
            transform.position = Vector3.Lerp(transform.position, initialCameraPosition, Time.deltaTime * followSpeed);
            if (Vector3.Distance(transform.position, initialCameraPosition) < 0.03f)
            {
                topDownCameraController.angleAboveTarget = 30;
                topDownCameraController.angleAroundTarget = 0;
                reachedInitialPos = true;
                gameManager.gameState = GameData.GameState.PlayerTurn;
            }
        }
    }


    private void CycleThroughDialogue()
    {
        string characterName;

        if (dialogueEntries.Count > 0)
        {

            stop = false;
            dialogueUI.SetActive(true);
            textSpeed = 0.05f;

            textEnded = false;
            dialogueEndIndicator.SetActive(false);
            currentEntry = dialogueEntries.Dequeue();

            if (currentEntry.character == null)
            {
                characterName = "Null Character";
                dialogueUI.SetActive(false);
                currentCharacter = null;
            }
            else
            {
                if (currentEntry.focus == null)
                {
                    currentEntry.focus = currentEntry.character;
                }
                characterName = currentEntry.character.name;
            }

            string dialogue = currentEntry.dialogueText;
            speakerNameText.text = characterName.Replace("(Clone)", "");


            StartCoroutine(ShowText(dialogueText, dialogue));
        }

        else if (dialogueEntries.Count == 0)
        {
            stop = true;
            textEnded = false;
            dialogueEndIndicator.SetActive(false);
            dialogueUI.SetActive(false);
        }
    }

    private void CameraMove()
    {
        if (currentEntry != null)
        {
            currentFocus = currentEntry.focus;
            currentCharacter = currentEntry.character;
        }

        if (cameraFlag == false && !stop)
        {
            if (currentEntry.character == null || currentFocus != currentCharacter)
            {
                Vector3 targetPosition = new Vector3(currentFocus.transform.position.x, transform.position.y, currentFocus.transform.position.z - 9);
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    dialogueEndIndicator.SetActive(true);
                    cameraFlag = true;
                    textEnded = true;
                }
            }

            else
            {
                Vector3 targetPosition = new Vector3(currentFocus.transform.position.x / 4, transform.position.y, currentFocus.transform.position.z - 9);
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    cameraFlag = true;
                }
            }
        }
    }

    public void WiggleText(TextMeshProUGUI textMesh)
    {
        textMesh.ForceMeshUpdate();
        var textInfo = textMesh.textInfo;
        bool withinParentheses = false;
        for (int j = 0; j < textInfo.characterCount; j++)
        {
            var charInfo = textInfo.characterInfo[j];
            if (!charInfo.isVisible)
            {
                continue;
            }

            if (charInfo.character == '(')
            {
                withinParentheses = true;
                continue;
            }
            else if (charInfo.character == ')')
            {
                withinParentheses = false;
                continue;
            }

            if (withinParentheses)
            {
                var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                for (int k = 0; k < 4; k++)
                {
                    var orig = verts[charInfo.vertexIndex + k];
                    verts[charInfo.vertexIndex + k] = orig + new Vector3(0, Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 3f, 0);
                }
            }
        }
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }


    private IEnumerator ShowText(TextMeshProUGUI textMesh, string fullText)
    {
        if (fullText.Length == 0)
        {
            dialogueEndIndicator.SetActive(false);
        }
        else
        {
            int i = 0;
            bool insideTag = false;
            while (i < fullText.Length)
            {
                if (fullText[i] == '<')
                {
                    insideTag = true;
                }

                if (fullText[i] == '>')
                {
                    insideTag = false;
                    i++;
                    continue;
                }

                if (!insideTag)
                {
                    textMesh.text = fullText.Substring(0, i + 1);
                    WiggleText(textMesh);
                    yield return new WaitForSeconds(textSpeed);
                }

                if (stop)
                {
                    yield break;
                }

                i++;
            }
            textEnded = true;
        }
    }
}