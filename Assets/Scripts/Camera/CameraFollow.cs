using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player_transform;
    [Header("Flip rotation stats")]
    [SerializeField] private float _flipYTime = 0.5f;
    [SerializeField] private float maxAhead = 3f;
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float _fallSpeedthres = -5f;
    [SerializeField] private float _fallMaxSpeed = -18f;
    [SerializeField] private float _fallLookahead = -7f;
    private float  cur_ahead;
    private float cur_fall;
    private float ahead_vel;
    private float _fall_vel;
    private Coroutine _turnCoroutine;    
    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = player_transform.gameObject.GetComponent<Player>();
    }
    // Update is called once per frame
    void Start()
    {
    }
    void LateUpdate()
    {
        float t = Mathf.Clamp01(Mathf.Abs(player.getXvelocity()/player.getmaxSpeed()));
        float smooth = t*t*(3f-2f*t);
        int moveDir = player.isFacingRight() ? 1 : -1;
        float targetAhead = smooth * maxAhead * moveDir;

        if (smooth == 0)
        {
            cur_ahead = Mathf.SmoothDamp(cur_ahead, targetAhead, ref ahead_vel, smoothTime*1.5f);
        }
        else
        {
            cur_ahead = Mathf.SmoothDamp(cur_ahead, targetAhead, ref ahead_vel, smoothTime);
        }
        
        float fallT = Mathf.InverseLerp(_fallSpeedthres, _fallMaxSpeed, player.getYVelocity());
        float targetFall = fallT * _fallLookahead;
        cur_fall = Mathf.SmoothDamp(cur_fall, targetFall, ref _fall_vel, smoothTime * 0.5f);

        transform.position = player.transform.position + new Vector3(cur_ahead, cur_fall ,0);
    }

    public void CallTurn()
    {
        _turnCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float start_rotation = transform.localEulerAngles.y;
        float end_rotation = player.isFacingRight() ? 0f : 180f;
        float yRot;

        float elapsedTime = 0f;
        while (elapsedTime < _flipYTime)
        {
            elapsedTime += Time.deltaTime;

            yRot = Mathf.Lerp(start_rotation, end_rotation,  elapsedTime /_flipYTime);
            transform.rotation = Quaternion.Euler(0f, yRot, 0f);

            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, end_rotation, 0f);
    }
}
