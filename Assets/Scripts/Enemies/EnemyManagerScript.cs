using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagerScript : MonoBehaviour
{
    [Header("Assigned Elements")]
    [SerializeField] public GameData gameManager;
    [SerializeField] public GameObject meatForkChefPrefab;
    [SerializeField] private List<EnemySpawner> enemySpawner = new List<EnemySpawner>();

    public enum SelectedEnemy
    {
        MeatForkChef,
        MalletChef,
        HeadChef
    }

    [System.Serializable]
    public class EnemySpawner
    {
        public int xCoordinate;
        public int yCoordinate;
        public SelectedEnemy selectedEnemy;
        public bool enemyInstantiated;
        public GameObject enemyInstance;
    }

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameState == GameData.GameState.CharacterPlacement)
        {
            foreach (EnemySpawner spawner in enemySpawner)
            {
                string nodeName = "Node_" + spawner.xCoordinate.ToString() + "_" + spawner.yCoordinate.ToString();

                GameObject node = GameObject.Find(nodeName);

                if (node != null && !spawner.enemyInstantiated)
                {
                    node.tag = "EnemyPlacementNode";
                    switch (spawner.selectedEnemy)
                    {
                        case SelectedEnemy.MeatForkChef:
                            spawner.enemyInstance = Instantiate(meatForkChefPrefab, new Vector3(node.transform.position.x, 1.5f, node.transform.position.z), Quaternion.identity);
                            SharedCharacterAttributesScript attributesScript = spawner.enemyInstance.GetComponent<SharedCharacterAttributesScript>();
                            attributesScript.SetGridPosition(spawner.xCoordinate, spawner.yCoordinate);
                            break;
                        default:
                            break;
                    }
                    spawner.enemyInstantiated = true;
                }
                else if (node == null)
                {
                    Debug.LogWarning("Node not found: " + nodeName);
                }
            }
        }
        else
        {
            foreach (EnemySpawner spawner in enemySpawner)
            {
                //Destroy(spawner.enemyPlacementInstance);
            }
        }
    }
}
