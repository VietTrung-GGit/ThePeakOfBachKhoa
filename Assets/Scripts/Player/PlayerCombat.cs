using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Player))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Properties")]
    [SerializeField] private GameObject attackWeapon;

    private Player player;
    private bool canHorizontalAttack = false;
    private bool canAttackDive = false;
    private Vector2 normalAttackWeaponPosition;

    private InputSystem_Actions inputActions;
    
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        player = GetComponent<Player>();
        normalAttackWeaponPosition = attackWeapon.transform.localPosition;
    }

    private void OnEnable() {
        inputActions.Player.Enable();
    }
    private void OnDisable() {
        inputActions.Player.Disable();
    }

    private void Start()
    {
        inputActions.Player.Attack.started += context => TriggerHorizontalAttack(context);
        inputActions.Player.AttackDive.started += context => TriggerAttackDive(context);
        inputActions.Player.Attack.canceled += context => CancelHorizontalAttack(context);
        inputActions.Player.Attack.canceled += context => CancelAttackDive(context);
    }

    private void Update()
    {
        if (canHorizontalAttack && (player.IsGrounded || !canAttackDive))
        {
            Vector2 attackWeaponPosition = attackWeapon.transform.localPosition;
            attackWeaponPosition.x = Mathf.Abs(attackWeaponPosition.x) * player.FacingDirection;
            attackWeapon.transform.localPosition = attackWeaponPosition;
            attackWeapon.SetActive(true);
        }
        else if (!player.IsGrounded && canAttackDive)
        {
            attackWeapon.transform.SetLocalPositionAndRotation(new Vector2(0.0f, - Mathf.Abs(normalAttackWeaponPosition.x)), Quaternion.Euler(0.0f, 0.0f, 90.0f));
            attackWeapon.SetActive(true);
            // Can add a dive impulse here
        }
        else
        {
            attackWeapon.SetActive(false);
            attackWeapon.transform.SetLocalPositionAndRotation(normalAttackWeaponPosition, Quaternion.identity);
        }
    }

    private void TriggerHorizontalAttack(InputAction.CallbackContext context)
    {
        Vector2 attackWeaponPosition = attackWeapon.transform.localPosition;
        attackWeaponPosition.x = Mathf.Abs(attackWeaponPosition.x) * player.FacingDirection;
        attackWeapon.transform.localPosition = attackWeaponPosition;
        attackWeapon.SetActive(true);
        canHorizontalAttack = true;
    }

    private void CancelHorizontalAttack(InputAction.CallbackContext context)
    {
        attackWeapon.SetActive(false);
        canHorizontalAttack = false;
    }

    private void TriggerAttackDive(InputAction.CallbackContext context)
    {
        Vector2 attackWeaponPosition = attackWeapon.transform.localPosition;
        attackWeaponPosition.x = Mathf.Abs(attackWeaponPosition.x) * player.FacingDirection;
        attackWeapon.transform.localPosition = attackWeaponPosition;
        attackWeapon.SetActive(true);
        canAttackDive = true;
    }

    private void CancelAttackDive(InputAction.CallbackContext context)
    {
        attackWeapon.SetActive(false);
        canAttackDive = false;
    }
}