// Source - https://stackoverflow.com/a/69741489
// Posted by KamielDev, modified by community. See post 'Timeline' for change history
// Retrieved 2026-11-16, License - CC BY-SA 4.0

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Delegates the call to OnTrigger2D for this object to another object.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class OnTrigger2DDelegator : MonoBehaviour
{
    private Collider2D caller;

    private void Awake()
    {
        caller = GetComponent<Collider2D>();
    }

    [Tooltip("Which function should be called when trigger was entered.")]
    public UnityEvent<OnTriggerDelegation> Enter;
    [Tooltip("Which function should be called when trigger was stayed in.")]
    public UnityEvent<OnTriggerDelegation> Stay;

    [Tooltip("Which function should be called when trigger was exited.")]
    public UnityEvent<OnTriggerDelegation> Exit;

    private void OnTriggerEnter2D(Collider2D other) => Enter.Invoke(new OnTriggerDelegation(caller, other));
    private void OnTriggerStay2D(Collider2D other) => Stay.Invoke(new OnTriggerDelegation(caller, other));
    private void OnTriggerExit2D(Collider2D other) => Exit.Invoke(new OnTriggerDelegation(caller, other));
}

/// <summary>
/// Stores which collider triggered this call and which collider belongs to the other object.
/// </summary>
public struct OnTriggerDelegation {

    /// <summary>
    /// Creates an OnTriggerDelegation struct.
    /// Stores which collider triggered this call and which collider belongs to the other object.
    /// </summary>
    /// <param name="caller">The trigger collider which triggered the call.</param>
    /// <param name="other">The collider which belongs to the other object.</param>
    public OnTriggerDelegation(Collider2D caller, Collider2D other)
    {
        Caller = caller;
        Other = other;
    }

    /// <summary>
    /// The trigger collider which triggered the call.
    /// </summary>
    public Collider2D Caller { get; private set; }

    /// <summary>
    /// The other collider.
    /// </summary>
    public Collider2D Other { get; private set; }
}
