using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    [Tooltip("The speed the projectile moves.")]
    public float moveSpeed = 3.0f;
    public GameObject player;
    //The direction that a turn angle is calculated from.
    private Vector3 baseDirection = Vector3.up;

    private void Update()
    {
        if (player == null)
        {
            return;
        }
        Vector3 direction = player.transform.position - transform.position;
        float eulerBaseTurnAngle = Mathf.Acos(Mathf.Clamp(Vector2.Dot(baseDirection, direction.normalized), -1.0f, 1.0f)) * Mathf.Rad2Deg;
        float directionClockwise = Vector3.Dot(Vector3.Cross(baseDirection, direction.normalized).normalized, Vector3.forward) >= 0.0f? 1.0f : -1.0f;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(new Vector3 (0.0f, 0.0f, eulerBaseTurnAngle * directionClockwise));
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0.0f, 0.0f, directionClockwise > 0.0f? eulerBaseTurnAngle : 360.0f - eulerBaseTurnAngle)), turnSpeed * Time.deltaTime);
    }
}