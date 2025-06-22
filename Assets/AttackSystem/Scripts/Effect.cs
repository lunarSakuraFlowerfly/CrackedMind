using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public float runTime=0.5f;

    private void Start()
    {
        transform.position = new Vector3(PlayerController.Instance.transform.position.x, PlayerController.Instance.transform.position.y +0.5f, PlayerController.Instance.transform.position.z);
        Destroy(gameObject, 0.467f);
    }
}
