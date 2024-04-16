using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEndIndicatorScript : MonoBehaviour
{
    [Header("Dialogue End Indicator Settings")]
    [SerializeField] private float maxRightPosition;
    [SerializeField] private float maxLeftPosition;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private bool moveRight = true;

    [Header("Assigned Elements")]
    public GameData gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
    }

    void FixedUpdate()
    {
        if (gameManager.gameState == GameData.GameState.Cutscene)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 currentPosition = rectTransform.anchoredPosition;

            float targetX = moveRight ? maxRightPosition : maxLeftPosition;
            currentPosition.x = Mathf.Lerp(currentPosition.x, targetX, Time.fixedDeltaTime * followSpeed);

            rectTransform.anchoredPosition = currentPosition;

            if (Mathf.Abs(currentPosition.x - targetX) < 0.8f)
            {
                moveRight = !moveRight;
            }
        }
    }
}
