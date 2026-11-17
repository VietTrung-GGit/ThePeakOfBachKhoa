using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    private static CameraManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


}
