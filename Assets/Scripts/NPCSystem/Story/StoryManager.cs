using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections;
using System;
using NPCSystem.Dialogue;
using Characters.Player;
using NPCSystem.Core;
using InteractionSystem.Item;
/// <summary>
/// 故事管理器
/// </summary>
namespace NPCSystem.Story
{
    public class StoryManager:MonoBehaviour
    {
        public static StoryManager Instance;

        //故事事件状态
        private Dictionary<StoryEventName,bool> storyEventStates;
        private void Awake()
        {
            Instance = this;
            InitializeAudioClipAndAudioSource();
            storyEventStates = new Dictionary<StoryEventName,bool>();
            foreach(StoryEventName eventName in Enum.GetValues(typeof(StoryEventName)))
            {
                storyEventStates[eventName] = false;
            }
        }
            // 添加Start方法来检查和初始化这些对象
    private void Start()
    {
        // 如果Inspector中未赋值，则尝试获取实例
        if(fadeManager == null) fadeManager = FadeManager.Instance;
        if(bed == null) bed = GameObject.Find("Bed");
        if(DoctorNPC == null) DoctorNPC = GameObject.Find("NPC_Doctor");
        
        // 检查并输出错误信息
        if(fadeManager == null) Debug.LogError("FadeManager未找到！");
        if(bed == null) Debug.LogError("Bed未找到！");
        if(DoctorNPC == null) Debug.LogError("DoctorNPC未找到！");

        // 尝试加载对话资源（如果在Inspector中未赋值）
        if (Chapter_0_event_3_Dialogue_1 == null)
            Chapter_0_event_3_Dialogue_1 = Resources.Load<DialogueData>("NPCs/Dialogue/chapter0/Chapter_0_event_3_dialogue_1");
        
        if (Chapter_0_event_3_Dialogue_2 == null)
            Chapter_0_event_3_Dialogue_2 = Resources.Load<DialogueData>("NPCs/Dialogue/chapter0/Chapter_0_event_3_dialogue_2");
        
        if (Chapter_0_event_3_Dialogue_3 == null)
            Chapter_0_event_3_Dialogue_3 = Resources.Load<DialogueData>("NPCs/Dialogue/chapter0/Chapter_0_event_3_dialogue_3");
        
        // 添加对话资源检查
        if (Chapter_0_event_3_Dialogue_1 == null)
            Debug.LogError("对话资源 Chapter_0_event_3_dialogue_1 加载失败！");
        
        if (Chapter_0_event_3_Dialogue_2 == null)
            Debug.LogError("对话资源 Chapter_0_event_3_dialogue_2 加载失败！");
        
        if (Chapter_0_event_3_Dialogue_3 == null)
            Debug.LogError("对话资源 Chapter_0_event_3_dialogue_3 加载失败！");
    }
        #region 序章中的变量
        //序章的第一个事件
        private Caputure0BedroomUnlockConditions caputure0BedroomUnlockConditions;
        
        [Header("序章中的变量")]
        [SerializeField] private GameObject BedroomDoor;
        [SerializeField] private GameObject Player;
        private AudioSource audioSource;
        [SerializeField] private AudioClip knockDoorSound;
        [SerializeField] private GameObject neighborNPC;
 
        [SerializeField] private DialogueData OnPlayerKnockDoorDialogue;



        #endregion

        #region 序章中的方法
        //检查是否有满足触发的可能
        private bool CheckBedroomUnlockConditions()
        {
            if(caputure0BedroomUnlockConditions.isBedTouched==true &&
             caputure0BedroomUnlockConditions.isCabinetTouched==true&&
             storyEventStates[StoryEventName.caputure0BedroomUnlockConditions]==false)
            {
                return true;
            }
            return false;
        }   

        //初始化audioclip和audiosource
        public void InitializeAudioClipAndAudioSource()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = knockDoorSound;
        }

        //每次设置变量的时候都会检查一次是否可以触发
        public void BedFirstTouchComplete()
        {
            caputure0BedroomUnlockConditions.isBedTouched = true;
            Debug.LogWarning("床第一次触摸完成");

            if(CheckBedroomUnlockConditions())
            {
                OnCaputure0BedroomUnlockConditionValid();
            }
        }
        public void CabinetFirstTouchComplete()
        {
            caputure0BedroomUnlockConditions.isCabinetTouched = true;
            Debug.LogWarning("柜子第一次触摸完成");
            if(CheckBedroomUnlockConditions())
            {
                OnCaputure0BedroomUnlockConditionValid();
            }
        }

        private void OnCaputure0BedroomUnlockConditionValid()
        {
            storyEventStates[StoryEventName.caputure0BedroomUnlockConditions] = true;
            StartCoroutine(DelayExecuteC0BV(3f));
        }

        private IEnumerator DelayExecuteC0BV(float delay)
        {
            yield return new WaitForSeconds(delay);

            //控制门打开
            BedroomDoor.SetActive(true);
            //敲门声音触发
            audioSource.PlayOneShot(knockDoorSound);
            yield return new WaitForSeconds(0.5f);
            //玩家独白
            DialogueManager.Instance.StartDialogue(OnPlayerKnockDoorDialogue,null);
            //将NPC放置到特定位置
            //neighborNPC.transform.position = LivingRoomDoor.position;
            //将NPC旋转到特定方向
            //neighborNPC.transform.rotation = Quaternion.Euler(0, 0, 0);

        }

        private IEnumerator DisablePlayerMovement(float duration)
        {
            Player.GetComponent<PlayerMovement>().enabled = false;
            yield return new WaitForSeconds(duration);
            Player.GetComponent<PlayerMovement>().enabled = true;
        }


        //序章的第二个事件

        [Header("序章的第二个事件")]
        [SerializeField] private Transform Capture0FirstMeetNeighborPosition;
        [SerializeField] private Vector3 biasPosition;
        private bool isPlayerMoveToRightPosition = false;
        [SerializeField] private DialogueData Capture0NeighborFirstEnterBedroomDialogue;
        [SerializeField] private Transform LivingRoomDoor;
    
        
        //玩家事件
        public void Capture0NeighborFirstEnterBedroom()
        {
            storyEventStates[StoryEventName.Capture0NeighborFirstEnterBedroom] = true;
            Player.GetComponent<PlayerMovement>().MoveTo(Capture0FirstMeetNeighborPosition.position-biasPosition,3f);
            Player.GetComponent<PlayerMovement>().OnPlayerMoveToRightPosition += OnPlayerMoveToRightPosition;
            

        }
        private void OnPlayerMoveToRightPosition()
        {
            //当玩家移动到特定位置后，npc开始出现然后移动
            neighborNPC.SetActive(true);
            neighborNPC.transform.position = LivingRoomDoor.position;
            neighborNPC.transform.rotation = LivingRoomDoor.rotation;
            //npc移动到特定位置
            neighborNPC.GetComponent<NPCEntity>().MoveTo(Capture0FirstMeetNeighborPosition.position+biasPosition);
            //移动到位置后朝向玩家
            neighborNPC.GetComponent<NPCEntity>().LookAt(Player.transform.position);
            //对话
            StartCoroutine(DelayDialogue(3f));
            Player.GetComponent<PlayerMovement>().OnPlayerMoveToRightPosition -= OnPlayerMoveToRightPosition;
            
        }
        //延迟对话
        private IEnumerator DelayDialogue(float delay)
        {
            yield return new WaitForSeconds(delay);
            DialogueManager.Instance.StartDialogue(Capture0NeighborFirstEnterBedroomDialogue,null);
            DialogueManager.Instance.OnDialogueEnd += OnCapture0NeighborFirstEnterBedroomDialogueEnd;
        }

        //当对话结束，调用序章的第三个事件
        private void OnCapture0NeighborFirstEnterBedroomDialogueEnd()
        {
            StartCoroutine(Capture0DoctorFirstEnterBedroom());
            DialogueManager.Instance.OnDialogueEnd -= OnCapture0NeighborFirstEnterBedroomDialogueEnd;
        }
        #endregion

        //[Header("序章的第三个事件")]
        
        //获取Asset/Resources/NPCs/Dialogue/Chapter_0_event_3_dialogue_1 资源
        private DialogueData Chapter_0_event_3_Dialogue_1;
        private DialogueData Chapter_0_event_3_Dialogue_2;
        private DialogueData Chapter_0_event_3_Dialogue_3;
        private FadeManager fadeManager = FadeManager.Instance;
        private GameObject bed = GameObject.Find("Bed");

        //从Hierarchy中获取DoctorNPC
        private GameObject DoctorNPC = GameObject.Find("NPC_Doctor");

        public void TestDoctorCome()
        {
            Debug.LogWarning("测试医生来");
            StartCoroutine(Capture0DoctorFirstEnterBedroom());
        }
        
        private IEnumerator Capture0DoctorFirstEnterBedroom()
        {
            storyEventStates[StoryEventName.Capture0DoctorFirstEnterBedroom] = true;
            //黑屏动画，同时主角禁用移动，可以直接disable移动脚本
            Player.GetComponent<PlayerMovement>().enabled = false;
            fadeManager.FadeIn();
            yield return new WaitForSeconds(2f);
            //黑屏后主角瞬移到卧室
            Player.transform.position = new Vector3(-5,-1.3f,0);
            fadeManager.FadeOut();
            yield return new WaitForSeconds(2f);
            Player.GetComponent<PlayerMovement>().enabled = true;

            //过2s后，出现敲门声，主角无法移动
            yield return new WaitForSeconds(2f);
            audioSource.PlayOneShot(knockDoorSound);
            yield return new WaitForSeconds(1.3f);

            // 添加对话资源检查
            if (Chapter_0_event_3_Dialogue_1 == null)
            {
                Debug.LogError("对话资源 Chapter_0_event_3_dialogue_1 为空，无法开始对话！");
                yield break;
            }
        
            //进行与邻居对话
            DialogueManager.Instance.StartDialogue(Chapter_0_event_3_Dialogue_1,null);


            DialogueManager.Instance.OnDialogueEnd += Chapter_0_when_door_opened;

        }

        private void Chapter_0_when_door_opened()
        {
            StartCoroutine(Cor_Chapter_0_when_door_opened());
            DialogueManager.Instance.OnDialogueEnd += On_Chapter_0_event_3_Dialogue_2_End;
            DialogueManager.Instance.OnDialogueEnd -= Chapter_0_when_door_opened;
        }

        private IEnumerator Cor_Chapter_0_when_door_opened()
        {
            //对话结束后邻居与医生进入房间，也就是移动移动到指定位置
            DoctorNPC.transform.position = new Vector3(2.07f,-3.78f,0);
            neighborNPC.transform.position = new Vector3(2.07f,-4f,0);
            neighborNPC.transform.rotation = Quaternion.Euler(0,0,0);
            DoctorNPC.transform.rotation = Quaternion.Euler(0,0,0);
            neighborNPC.GetComponent<NPCEntity>().LookAt(DoctorNPC.transform.position-new Vector3(1,0,0));
            DoctorNPC.GetComponent<NPCEntity>().LookAt(Player.transform.position-new Vector3(1,0,0));
            yield return new WaitForSeconds(0.5f);
            //医生移动到主角面前，然后朝向主角
            Vector3 neighborPosition_1 = new Vector3(-3.0f,-4f,0);
            Vector3 doctorPosition_1 = new Vector3(-4.0f,-3.78f,0);
            Vector3 doctorPosition_2 = new Vector3(-4.0f,-1.3f,0);
            DoctorNPC.GetComponent<NPCEntity>().MoveTo(doctorPosition_1);
            neighborNPC.GetComponent<NPCEntity>().MoveTo(neighborPosition_1);
            yield return DelayWhenPlayerMoveTo(neighborNPC,neighborPosition_1);
            neighborNPC.GetComponent<NPCEntity>().LookAt(Player.transform.position);
            yield return DelayWhenPlayerMoveTo(DoctorNPC,doctorPosition_1);   
            DoctorNPC.GetComponent<NPCEntity>().MoveTo(doctorPosition_2);
            yield return DelayWhenPlayerMoveTo(DoctorNPC,doctorPosition_2);
            DoctorNPC.GetComponent<NPCEntity>().LookAt(Player.transform.position);
            Player.GetComponent<PlayerMovement>().SetLookAt(neighborNPC.transform.position);

            yield return new WaitForSeconds(0.5f);
            DialogueManager.Instance.StartDialogue(Chapter_0_event_3_Dialogue_2,null);
        }



        //延迟到玩家移动到指定位置
        private IEnumerator DelayWhenPlayerMoveTo(GameObject _object,Vector3 _pos)
        {
            //如果插值小于0.1f，则认为到达指定位置
            while(Vector3.Distance(_object.transform.position,_pos)>0.1f)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
        }
        //添加对话结束的事件调用
        private void On_Chapter_0_event_3_Dialogue_2_End()
        {
            //如果对话结束，则调用Capture0DoctorFirstEnterBedroom
            StartCoroutine(Capture_0_event_3_PlayerSleep());
            DialogueManager.Instance.OnDialogueEnd -= On_Chapter_0_event_3_Dialogue_2_End;
        }
        private IEnumerator Capture_0_event_3_PlayerSleep()
        {
            yield return new WaitForSeconds(0.5f);
            //对话后医生做到凳子上，玩家躺倒床上
            //玩家移动到床的位置
            Vector3 playerPosition_1 = new Vector3(-7f,0.7f,0);
            Vector3 doctorPosition_1 = new Vector3(-7.003f,1.398f,0);
            Player.GetComponent<PlayerMovement>().MoveTo(playerPosition_1,3f);
            DoctorNPC.GetComponent<NPCEntity>().MoveTo(doctorPosition_1);
            yield return DelayWhenPlayerMoveTo(Player,playerPosition_1);
            yield return DelayWhenPlayerMoveTo(DoctorNPC,doctorPosition_1);
            Player.GetComponent<PlayerMovement>().SetLookAt(doctorPosition_1-new Vector3(1,0,0));
            //玩家躺在床上
            Player.GetComponent<PlayerMovement>().enabled = false;
            Player.GetComponent<PlayerMovement>().SetSleepState(true);
            //医生坐下
            yield return DelayWhenPlayerMoveTo(DoctorNPC,doctorPosition_1);
            DoctorNPC.GetComponent<NPCEntity>().SetSitState(true,new Vector3(-7.0f,1.403f,0));
            yield return new WaitForSeconds(1.2f);  
            //开启对话，对话结束后玩家进入梦境，可以直接调用bed的睡觉函数
            DialogueManager.Instance.StartDialogue(Chapter_0_event_3_Dialogue_3,null);
            DialogueManager.Instance.OnDialogueEnd += On_Chapter_0_event_3_Dialogue_3_End;
        }
        private void On_Chapter_0_event_3_Dialogue_3_End()
        {
            //如果对话结束，调用bed的睡觉函数
            bed.GetComponent<BedInteractable>().StartSleepSequence();
            DialogueManager.Instance.OnDialogueEnd -= On_Chapter_0_event_3_Dialogue_3_End;
        }
        
    }


    

    public struct Caputure0BedroomUnlockConditions
    {
        public bool isBedTouched;
        public bool isCabinetTouched;

    }

    public enum StoryEventName
    {
        caputure0BedroomUnlockConditions,
        Capture0NeighborFirstEnterBedroom,
        Capture0DoctorFirstEnterBedroom
    }
}

