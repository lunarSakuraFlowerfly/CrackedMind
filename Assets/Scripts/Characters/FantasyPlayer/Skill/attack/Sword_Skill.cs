using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : BaseSkill
{
    public SwordType swordType = SwordType.Regular;

    
    [Header("Bounce info")]
    [SerializeField] private int amountOfBounce;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Spin info")]
    [SerializeField]private float hitCooldown = .35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;

    [Header("Pierce info")]
    [SerializeField] private int amountOfPierce;
    [SerializeField] private float pierceGravity;

    [Header("skill info")]
    [SerializeField] private GameObject skillPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed = 12;

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;


    private List<GameObject> aimDots = new List<GameObject>();
    [Header("Skill Tree")]
    [SerializeField] private UI_SkillTreeSlot unlockSkillButton;
    [SerializeField] private bool canUseSkill;

    protected override void Start()
    {
        base.Start();
        skillType = SkillType.Talent;
        GenerateDots();
        unlockSkillButton.GetComponent<Button>().onClick.AddListener(UnlockSkill);
        unlockSkillButton.associatedSkill = this;
    }
    private void UnlockSkill()
    {
        if(unlockSkillButton.unlocked)
        {
            canUseSkill = true;
        }
    }
    public override bool CanUseSkill()
    {
        if(!canUseSkill) return false;
        if(player.Sword != null) return false;
        return base.CanUseSkill();
    }
    protected override void Update()
    {
        base.Update();
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            finalDir = new Vector2(AimDirection().x * launchForce.x, AimDirection().y * launchForce.y);
            DotsActive(false);
        }
        if(Input.GetKey(KeyCode.Mouse1))
        {
            for(int i = 0; i < numberOfDots; i++)
            {
                dots[i].transform.position = DotsPosition(spaceBetweenDots*i);
            }
        }
        
    }

    private void SetupGravity()
    {
        switch(swordType)
        {
            case SwordType.Bounce:
                swordGravity = bounceGravity;
                break;
            case SwordType.Pierce:
                swordGravity = pierceGravity;
                break;
            case SwordType.Spin:
                swordGravity = spinGravity;
                break;
            default:
                break;
        }
            
    }


    public void CreateSword()
    {
        GameObject newSword = Instantiate(skillPrefab,player.transform.position,transform.rotation);
        Sword_Skill_Controller sword = newSword.GetComponent<Sword_Skill_Controller>();
        SetupGravity();
        if(swordType == SwordType.Bounce)
        {
            sword.SetupBounce(true,amountOfBounce,bounceSpeed);
        }
        else if(swordType == SwordType.Pierce)
        {
            sword.SetupPierce(true,amountOfPierce);
        }
        else if(swordType == SwordType.Spin)
        {
            sword.SetupSpin(true,maxTravelDistance,spinDuration,hitCooldown);
        }
        sword.SetupSword(finalDir,swordGravity,player,freezeTimeDuration,returnSpeed);
        player.AssignNewSword(newSword);
        DotsActive(false);
    }

    #region Aim
    public Vector2 AimDirection()
    {
        Vector2 playerPos = player.transform.position;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - playerPos;
        return dir.normalized;
    }

    public void DotsActive(bool _isActive)
    {
        for(int i = 0; i < numberOfDots; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for(int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab,player.transform.position,Quaternion.identity,dotsParent);
            dots[i].SetActive(false);
        }
    }
    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().x*launchForce.x*t,
            AimDirection().y*launchForce.y*t) + 0.5f*(Physics2D.gravity*swordGravity)*(t*t);
        return position;
    }
    #endregion
    protected override void CheckUnlock()
    {
        UnlockSkill();
    }
}