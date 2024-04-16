using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class CharPlacementButtonScript : MonoBehaviour
{
    [Header("Assigned Elements")]
    [SerializeField] public GameObject OnionPlacementPrefab;
    [SerializeField] public GameObject LemonPlacementPrefab;
    [SerializeField] public GameObject BananaPlacementPrefab;
    [SerializeField] public GameObject PumpkinPlacementPrefab;
    [SerializeField] public GameData gameManager;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button button;
    [SerializeField] private int buttonNumber;

    [Header("Verifications")]
    [SerializeField] public ButtonChar buttonChar;
    [SerializeField] public bool placed;
    [SerializeField] public bool placing;

    private GameObject instantiatedPrefab;

    public enum ButtonChar
    {
        Onion,
        Banana,
        Pumpkin,
        Lemon
    }

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();

        buttonNumber = GetButtonNumberFromName(gameObject.name);
    }

    private void Start()
    {
        switch (buttonChar)
        {
            case ButtonChar.Onion:
                buttonText.text = " " + buttonNumber.ToString() + ") " + "Onion";
                break;
            case ButtonChar.Lemon:
                buttonText.text = " " + buttonNumber.ToString() + ") " + "Lemon";
                break;
            case ButtonChar.Banana:
                buttonText.text = " " + buttonNumber.ToString() + ") " + "Banana";
                break;
            case ButtonChar.Pumpkin:
                buttonText.text = " " + buttonNumber.ToString() + ") " + "Pumpkin";
                break;
            default:
                buttonText.text = "Unknown";
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(buttonNumber.ToString()) && !placing)
        {
            InstantiateCharacters();
        } else if (Input.GetKeyDown(buttonNumber.ToString()) && placing)
        {
            Destroy(instantiatedPrefab);
        }


        if (instantiatedPrefab == null && GameObject.FindWithTag(buttonChar.ToString()) == null)
        {
            button.image.color = Color.white;
        }
        else if (instantiatedPrefab != null || GameObject.FindWithTag(buttonChar.ToString()) != null)
        {
            button.image.color = Color.gray;
        }

        if (GameObject.FindWithTag(buttonChar.ToString()) != null)
        {
            placed = true;
            placing = false;
        }
        else
        {
            placed = false;
        }

        if (instantiatedPrefab == null && placed == false)
        {
            placing = false;
        }
    }

    public void OnButtonClick()
    {
        InstantiateCharacters();
    }

    private int GetButtonNumberFromName(string name)
    {
        Match match = Regex.Match(name, @"\d+$");
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        return -1;
    }

    private void InstantiateCharacters()
    {
        if (instantiatedPrefab == null && GameObject.FindWithTag(buttonChar.ToString()) == null && CanPlace())
        {
            switch (buttonChar)
            {
                case ButtonChar.Onion:
                    instantiatedPrefab = Instantiate(OnionPlacementPrefab);
                    break;
                case ButtonChar.Banana:
                    instantiatedPrefab = Instantiate(BananaPlacementPrefab);
                    break;
                case ButtonChar.Lemon:
                    instantiatedPrefab = Instantiate(LemonPlacementPrefab);
                    break;
                case ButtonChar.Pumpkin:
                    instantiatedPrefab = Instantiate(PumpkinPlacementPrefab);
                    break;
            }
            placing = true;
        }
        else if (placing)
        {
            Destroy(instantiatedPrefab);
        }
    }

    private bool CanPlace()
    {
        CharPlacementButtonScript[] allButtonScripts = FindObjectsOfType<CharPlacementButtonScript>();
        foreach (CharPlacementButtonScript buttonScript in allButtonScripts)
        {
            if (buttonScript != this && buttonScript.placing)
            {
                return false;
            }
        }
        return true;
    }
}
