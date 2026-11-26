using UnityEngine;

[CreateAssetMenu(fileName = "BreakableData", menuName = "Scriptable Objects/BreakableData")]
public class BreakableData : ScriptableObject
{
    [SerializeField] private float maxHP;
    [SerializeField] private float hp;

}
