
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    void Awake()
    {
        instance = this;
    }

}
