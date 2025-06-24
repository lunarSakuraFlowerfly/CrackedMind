using System.Collections;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using TMPro;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;
    [Header("Pop up text FX")]
    [SerializeField] private GameObject popUpTextPrefab;
    [Header("Screen shake FX")]
    [SerializeField] private CinemachineImpulseSource screenShake;
    [SerializeField] private float shakeMultiplier;
    [SerializeField] private Vector3 shakePower;
    [Header("After Image FX")]
    [SerializeField] private float afterImageCooldown;
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;
    private float afterImageCooldownTimer;
    [SerializeField] private GameObject parent;


    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("Aliment color")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    [Header("Hit FX")]
    [SerializeField] private GameObject hitFX;
    [SerializeField] private GameObject criticalHitFX;
    [SerializeField] private Entity entity;

    [Space]
    [SerializeField] private ParticleSystem dustFX;

    private void Update()
    {
        afterImageCooldownTimer -=Time.deltaTime;
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalMat = sr.material;
    }

    public void CreatePopUpText(string _text)
    {
        float randomX = Random.Range(-0.3f,0.3f);
        float randomY = Random.Range(0.5f,0.8f);
        Vector3 positionOffset = new Vector3(randomX,randomY,0);
        GameObject newText = Instantiate(popUpTextPrefab,parent.transform.position+positionOffset,Quaternion.identity);
        newText.GetComponent<TextMeshPro>().text = _text;
    }

    public void ScreenShake()
    {
        screenShake.m_DefaultVelocity = new Vector3(shakePower.x*entity.facingDirection,shakePower.y)*shakeMultiplier;
        screenShake.GenerateImpulse();
    }
    public void ScreenShake(Vector2 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x*entity.facingDirection,_shakePower.y)*shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    public void CreateAfterImage()
    {
        if(afterImageCooldownTimer<0)
        {
            afterImageCooldownTimer = afterImageCooldown;
            GameObject newAfterImage = Instantiate(afterImagePrefab,transform.position,Quaternion.identity);
            if(entity.facingDirection==-1)
            {
                newAfterImage.transform.localScale = new Vector3(-1,1,1);
            }
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate,sr.sprite);
        }
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(.2f);
        sr.color = currentColor;
        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        if(sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;
    }

    #region 负面状态

    
    public void IgniteFxFor(float _seconds)
    {
        CancelColorChange();
        InvokeRepeating("IgniteColorFx",0,.3f);
        Invoke("CancelColorChange",_seconds);
    }
    private void IgniteColorFx()
    {
        if(sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    public void ChillFxFor(float _seconds)
    {
        CancelColorChange();
        InvokeRepeating("ChillColorFx",0,.3f);
        Invoke("CancelColorChange",_seconds);
    }



    private void ChillColorFx()
    {
        if(sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }

    public void ShockFxFor(float _seconds)
    {
        CancelColorChange();
        InvokeRepeating("ShockColorFx",0,.3f);
        Invoke("CancelColorChange",_seconds);
    }

    private void ShockColorFx()
    {
        if(sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    public void CreateHitFX(Transform _hitPosition,bool _critical)
    {
        float zRotation = Random.Range(-90,90);
        float xPosition = Random.Range(-0.5f,0.5f);
        float yPosition = Random.Range(-0.5f,0.5f);
        Vector3 hitFxRotation = new Vector3(0,0,zRotation);
        GameObject hitPrefab = hitFX;
        if(_critical)
        {
            hitPrefab = criticalHitFX;
            float yRotation = 0;
            zRotation =Random.Range(-45,45);
            if(entity.facingDirection==-1)
                yRotation = 180;
            hitFxRotation = new Vector3(0,yRotation,zRotation);

        }
        GameObject newHitFX = Instantiate(hitPrefab,_hitPosition.position+new Vector3(xPosition,yPosition,0),Quaternion.identity);
        newHitFX.transform.Rotate(hitFxRotation);
        Destroy(newHitFX,0.2f);
    }

    public void PlayDustFX()
    {
        if(dustFX!=null)
        {
            dustFX.Play();
        }
    }
    #endregion
}
