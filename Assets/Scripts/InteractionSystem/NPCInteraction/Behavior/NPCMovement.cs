using UnityEngine;
using System.Collections;

namespace NPCSystem.Behavior
{
    /// <summary>
    /// NPC移动控制器，处理NPC的移动行为
    /// </summary>
    public class NPCMovement : MonoBehaviour
    {
        #region 移动设置
        [Header("移动设置")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float stoppingDistance = 0.1f;
        [SerializeField] private bool useSmoothMovement = true;
        [SerializeField] private float smoothTime = 0.1f;
        #endregion

        #region 动画设置
        [Header("动画设置")]
        [SerializeField] private Animator animator;
        [SerializeField] private float moveThreshold = 0.1f;
        #endregion

        #region 私有字段
        private Vector2 targetPosition;
        private bool isMoving = false;
        private Vector2 currentVelocity;
        private Rigidbody2D rb;
        private float lastValidDirection = 0f;

        
        // 状态常量
        private const int IDLE_STATE = 0;
        private const int WALK_STATE = 1;
        private const int SIT_STATE = 2;
        #endregion

        #region Unity生命周期
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }

            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if(animator != null)
            {
                animator.SetInteger("State", IDLE_STATE);
                animator.SetFloat("Direction", lastValidDirection);
            }
        }

        private void Update()
        {
            if (isMoving)
            {
                Vector2 currentPosition = transform.position;
                float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);

                if (distanceToTarget <= stoppingDistance)
                {
                    StopMoving();
                    return;
                }

                UpdateAnimationState(targetPosition - currentPosition);
            }
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                Vector2 currentPosition = transform.position;
                Vector2 direction = (targetPosition - currentPosition).normalized;
                Vector2 movement = direction * moveSpeed;

                if (useSmoothMovement)
                {
                    rb.velocity = Vector2.SmoothDamp(rb.velocity, movement, ref currentVelocity, smoothTime);
                }
                else
                {
                    rb.velocity = movement;
                }
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="position">目标位置</param>
        public void MoveTo(Vector2 position)
        {
            targetPosition = position;
            isMoving = true;
            
            // 设置为行走状态
            if (animator != null)
            {
                animator.SetInteger("State", WALK_STATE);
            }
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        public void StopMoving()
        {
            isMoving = false;
            rb.velocity = Vector2.zero;
            UpdateAnimationState(Vector2.zero);
            
            // 设置为待机状态
            if (animator != null)
            {
                animator.SetInteger("State", IDLE_STATE);
            }
        }
        
        /// <summary>
        /// 坐下
        /// </summary>
        public void Sit()
        {
            isMoving = false;
            rb.velocity = Vector2.zero;
            
            // 设置为坐下状态
            if (animator != null)
            {
                animator.SetInteger("State", SIT_STATE);
            }
        }

        /// <summary>
        /// 检查是否正在移动
        /// </summary>
        /// <returns>是否正在移动</returns>
        public bool IsMoving()
        {
            return isMoving;
        }

        /// <summary>
        /// 获取当前移动速度
        /// </summary>
        /// <returns>当前速度大小</returns>
        public float GetCurrentSpeed()
        {
            return rb.velocity.magnitude;
        }

        /// <summary>
        /// 朝向玩家
        /// </summary>
        public IEnumerator LookAt(Vector3 targetPosition)
        {
            if(animator!=null)
            {
                //计算朝向目标的方向    
                yield return new WaitUntil(()=>!isMoving);
                Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                //计算角度
                float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
                //将角度转换为方向
                lastValidDirection = MapAngleToDirection(angle);
                //更新动画方向
                animator.SetFloat("Direction", lastValidDirection);
                Debug.Log("LookAt: Direction = " + lastValidDirection);

            }
        }

        /// <summary>
        /// 设置坐下状态
        /// </summary>
        public void SetSitState(bool isSit)
        {
            isMoving = false;
            if(animator!=null)
            {
                animator.SetInteger("State", SIT_STATE);
            }
        }

        /// <summary>
        /// 四处察看
        /// </summary>
        public IEnumerator LookAround()
        {
            isMoving = false;
            //以当前的方向为节点，播放一个右方向的等待动画，然后回来后播放一个向左方向的等待动画，然后再回来
            //播放一个右方向的等待动画
            animator.SetInteger("State", IDLE_STATE);
            SetDirectionOffset(1);
            yield return new WaitForSeconds(1f);
            SetDirectionOffset(-1);
            yield return new WaitForSeconds(1f);
            SetDirectionOffset(-1);
            yield return new WaitForSeconds(1f);
            SetDirectionOffset(1);
            yield return new WaitForSeconds(1f);
        }

        public void SetDirectionOffset(float offset)
        {
            // 确保结果始终为正数
            lastValidDirection = (lastValidDirection + offset + 4) % 4;
            animator.SetFloat("Direction", lastValidDirection);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 更新动画状态
        /// </summary>
        /// <param name="moveDirection">移动方向</param>
        private void UpdateAnimationState(Vector2 moveDirection)
        {
            if (animator == null) return;

            if (moveDirection.magnitude > moveThreshold)
            {
                // 修正角度计算，保持与LookAt一致
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                float direction = MapAngleToDirection(angle);
                lastValidDirection = direction;

                animator.SetFloat("Direction", direction);
                animator.SetInteger("State", WALK_STATE);
            }
            else
            {
                animator.SetFloat("Direction", lastValidDirection);
                animator.SetInteger("State", IDLE_STATE);
            }
        }

        /// <summary>
        /// 将角度映射到四个方向
        /// 0=F(前), 1=L(左), 2=R(右), 3=B(后)
        /// </summary>
        /// <param name="angle">角度值</param>
        /// <returns>映射后的方向值</returns>
        private float MapAngleToDirection(float angle)
        {
            Debug.Log("MapAngleToDirection: angle = " + angle);
            angle = (angle + 360f) % 360f;

            // 四方向映射
            if (angle > 45f && angle <= 135f) return 3f;     
            if (angle > 135f && angle <= 225f) return 1f;    
            if (angle > 225f && angle <= 315f) return 2f;    
            return 0f;                                       
        }
        #endregion
    }
}