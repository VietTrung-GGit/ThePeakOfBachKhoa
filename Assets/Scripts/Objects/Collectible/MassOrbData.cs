using UnityEngine;

[CreateAssetMenu(fileName = "MassOrbData", menuName = "Scriptable Objects/MassOrbData")]
public class MassOrbData : ScriptableObject
{
    [SerializeField] private float mMulti = 0.5f;

    public float massMulti()
    {
        return mMulti;
    }
}
