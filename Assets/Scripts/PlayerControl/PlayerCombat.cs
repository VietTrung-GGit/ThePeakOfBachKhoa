using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Sword Settings")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private float swordDuration = 0.2f;
    [Header("Side Attack")]
    [SerializeField] private float sideDistance = 1.25f;
    [SerializeField] private float sideHeight = -0.14f;
    [Header("Down Attack")]
    [SerializeField] private float downDistance = 0.9f;
    private Player player;
    private GameObject activeSword;
    private void Awake() {
        player = GetComponent<Player>();
    }
    private void LateUpdate() {
        if (!player.AttackRequested || swordPrefab == null) return;
        if (activeSword != null){
            activeSword.SetActive(false);
            Destroy(activeSword);
        }
        bool attackDown = player.slideInputHold && !player.isGrounded;
        int facingDirection = player.facingDirection;
        Vector3 swordPosition;
        float swordRotation;
        if (attackDown) {
            swordPosition = new Vector3(0f,-downDistance,0f);
            swordRotation = -45f;
        } else {
            swordPosition = new Vector3(sideDistance * facingDirection,sideHeight,0f);
            swordRotation = facingDirection < 0 ? 180f : 0f;
        }
        activeSword = Instantiate(swordPrefab,transform,false);
        activeSword.transform.localPosition = swordPosition;
        activeSword.transform.localRotation = Quaternion.Euler(0f, 0f, swordRotation);
        Destroy(activeSword, swordDuration);
    }
}