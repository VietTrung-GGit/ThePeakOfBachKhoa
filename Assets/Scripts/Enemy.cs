using UnityEngine;
public abstract class Enemy : MonoBehaviour
{
    [Range(0.0f, 5.0f)]
    [Tooltip("The time the character must wait to escape the threatened state.")]
    public float threatTime = 3.0f;
    [Range(0.0f, 5.0f)]
    [Tooltip("The time the character must wait to escape the paralyzed state.")]
    public float paralysisTime = 3.0f;
    private float currentThreatTime;
    private float currentParalysisTime;

    // Check if the player is threatening this character.
    private bool isThreatened = false;
    // Check if the player poses a threat to this character.
    private bool isPlayerAThreat = false;
    private bool isParalyzed = false;
    public bool IsThreatened => isThreatened;
    public bool IsParalyzed => isParalyzed;

    private void Awake()
    {
        currentParalysisTime = paralysisTime;
        currentThreatTime = threatTime;
    }

    private void Update()
    {
        UpdateLogic();
        UpdateTimer();
    }
    protected abstract void UpdateLogic();
    private void UpdateTimer()
    {
        if (isThreatened && !isPlayerAThreat)
        {
            currentThreatTime -= Time.deltaTime;
            if (currentThreatTime <= 0.0f)
            {
                isThreatened = false;
            }
        }

        if (isParalyzed)
        {
            currentParalysisTime -= Time.deltaTime;
            if (currentParalysisTime <= 0.0f)
            {
                isParalyzed = false;
            }
        }
    }

    public void OnPlayerTriggerEnter2D(OnTriggerDelegation delegation)
    {
        //Must check if Player is in Super Sprint
        if (delegation.Other.CompareTag("Player"))
        {
            currentThreatTime = threatTime;
            isThreatened = true;
            isPlayerAThreat = true;
        }
    }

    public void OnPlayerTriggerExit2D(OnTriggerDelegation delegation)
    {
        if (delegation.Other.CompareTag("Player"))
        {
            isPlayerAThreat = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Knock back and defeat based on collision with player and whether the player is in Super Sprint
        if (collision.collider.CompareTag("Player"))
        {
            currentParalysisTime = paralysisTime;
            isParalyzed = true;
        }
    }
}