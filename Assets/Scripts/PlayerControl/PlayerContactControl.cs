using UnityEngine;
[RequireComponent(typeof(Player))]
public class PlayerContactHandler : MonoBehaviour {
    private Player player;
    private void Awake() {
        player = GetComponent<Player>();
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Enemy")) {
            if (player.isSuperSprinting) {
                Debug.Log("Super Sprint: Enemy destroyed!");
                Destroy(other);
            } else {
                Debug.Log("Trừ score do chạm enemy");
            }
            return;
        }
    }
}