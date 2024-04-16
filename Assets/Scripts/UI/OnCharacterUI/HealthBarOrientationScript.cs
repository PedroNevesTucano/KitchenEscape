using UnityEngine;

public class HealthBarOrientationScript : MonoBehaviour
{
    [Header("Assigned Elements")]
    [SerializeField] public GameData gameManager;
    private Camera mainCamera;
    private GameObject[] childObjects;
    public GameObject playerAura;
    public GameObject indicatorsBackground;
    public GameObject canWalkIndicator;
    public GameObject canAttackIndicator;
    public GameObject canUseAbilityIndicator;
    public SharedCharacterAttributesScript sharedAttributesScript;

    [Header("Verifications")]
    public bool found;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
        childObjects = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childObjects[i] = transform.GetChild(i).gameObject;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;

        if (!found)
        {
            sharedAttributesScript = FindSharedAttributesScript(transform.parent);
            found = true;
        }
    }

    void Update()
    {
        if (gameManager.gameState == GameData.GameState.PlayerTurn || gameManager.gameState == GameData.GameState.EnemyTurn)
        {
            if (mainCamera != null)
            {
                SetChildObjectsActive(true);

                Vector3 directionToCamera = mainCamera.transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(directionToCamera);
            }
        }
        else
        {
            SetChildObjectsActive(false);
        }
    }

    private void SetChildObjectsActive(bool active)
    {
        foreach (var child in childObjects)
        {
            child.SetActive(active);
        }
        playerAura.SetActive(active);
        if (sharedAttributesScript.allyCharacter)
        {
            indicatorsBackground.SetActive(active && gameManager.gameState == GameData.GameState.PlayerTurn);
            canWalkIndicator.SetActive(active && gameManager.gameState == GameData.GameState.PlayerTurn);
            canAttackIndicator.SetActive(active && gameManager.gameState == GameData.GameState.PlayerTurn);
            canUseAbilityIndicator.SetActive(active && gameManager.gameState == GameData.GameState.PlayerTurn);
        }
        else
        {
            indicatorsBackground.SetActive(active && gameManager.gameState == GameData.GameState.EnemyTurn);
            canWalkIndicator.SetActive(active && gameManager.gameState == GameData.GameState.EnemyTurn);
            canAttackIndicator.SetActive(active && gameManager.gameState == GameData.GameState.EnemyTurn);
        }
    }

    private SharedCharacterAttributesScript FindSharedAttributesScript(Transform currentTransform)
    {
        if (currentTransform == null)
        {
            return null;
        }

        SharedCharacterAttributesScript sharedAttributesScript = currentTransform.GetComponent<SharedCharacterAttributesScript>();
        if (sharedAttributesScript != null)
        {
            return sharedAttributesScript;
        }
        else
        {
            return FindSharedAttributesScript(currentTransform.parent);
        }
    }
}

