using UnityEngine;
using UnityEngine.UI;

public class CanWalkAttackIndicatorScript : MonoBehaviour
{
    [Header("Assigned Elements")]
    public SharedCharacterAttributesScript sharedAttributesScript;
    public Image canWalk;
    public Image canAttack;
    public Image canUseAbility;

    [Header("Verifications")]
    public bool found;

    // Start is called before the first frame update
    void Start()
    {
        if (!found)
        {
            sharedAttributesScript = FindSharedAttributesScript(transform.parent);
            found = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!sharedAttributesScript.walked)
        {
            canWalk.color = Color.green;
        }
        else
        {
            canWalk.color = new Color(0.0f, 0.5f, 0.0f);
        }

        if (!sharedAttributesScript.attacked)
        {
            canAttack.color = Color.red;
        } 
        else
        {
            canAttack.color = new Color(0.5f, 0.0f , 0.0f);
        }

        if (sharedAttributesScript.currentAbility != SharedCharacterAttributesScript.Ability.None)
        {
            if (!sharedAttributesScript.usedAbility)
            {
                canUseAbility.color = new Color(1f, 0.0f, 1f); // Purple
            }
            else
            {
                canUseAbility.color = new Color(0.25f, 0.0f, 0.25f); // Dark purple
            }
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
