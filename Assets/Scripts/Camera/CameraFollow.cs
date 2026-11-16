using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.AI;


public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player_transform;
    [Header("Flip rotation stats")]
    [SerializeField] private float _flipYTime = 0.5f;
    private Coroutine _turnCoroutine;    
    private Player player;
    private bool isFacingRight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = player_transform.gameObject.GetComponent<Player>();

        isFacingRight = player.isFacingRight();
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = player_transform.position;
    }

    public void CallTurn()
    {
        _turnCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float start_rotation = transform.localEulerAngles.y;
        float end_rotation = player.isFacingRight() ? 0f : 180f;
        float yRot = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < _flipYTime)
        {
            elapsedTime += Time.deltaTime;

            yRot = Mathf.Lerp(start_rotation, end_rotation, ( elapsedTime /_flipYTime));
            transform.rotation = Quaternion.Euler(0f, yRot, 0f);

            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, end_rotation, 0f);
    }
}
