using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_PopUpTextFx : MonoBehaviour
{
    private TextMeshPro textMesh;
    [SerializeField] private float speed;
    [SerializeField] private float desapearSpeed;
    [SerializeField] private float colorDesapearSpeed;
    [SerializeField] private float lifeTime;

    private float textTimer;

    private void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        textTimer = lifeTime;
    }

    void Update()
    {
        textTimer -= Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position,new Vector2(transform.position.x,transform.position.y+1),speed*Time.deltaTime);
        if(textTimer<0)
        {
            float alpha = textMesh.color.a-colorDesapearSpeed*Time.deltaTime;
            textMesh.alpha = alpha;
            if(textMesh.color.a<50)
                speed = desapearSpeed;
            if(textMesh.color.a<=0)
                Destroy(gameObject);
        }
    }
}
