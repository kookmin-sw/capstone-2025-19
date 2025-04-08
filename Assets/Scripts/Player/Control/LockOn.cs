using System.Collections;
using System.Collections.Generic;
using PlayerControl;
using Unity.Mathematics;
using UnityEngine;

namespace PlayerControl
{
    public class LockOn : MonoBehaviour
    {
        [Header("Init")]
        public GameObject mainCamera;
        public LayerMask targetLayer;
        public Transform lockOnImage;

        [Header("Setting")]
        public float lockOnRadius = 10f;
        public float minViewAngle = -70;
        public float maxViewAngle = 70;
        public float lookAtSmoothing = 10f;

        [Header("Debug")]
        public bool isFindTarget = false;
        public LockOnTarget currentTarget;
        public List<LockOnTarget> targetEnemies = new List<LockOnTarget>();
        
        private InputHandler input;
        private bool isLockOn = false;
        private int currentIndex = 0;

        private Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        // Start is called before the first frame update
        void Start()
        {
            input = GetComponent<InputHandler>();
            lockOnImage.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (input.lockOn)
            {
                isLockOn = isLockOn ? false : true;
                if (isLockOn)
                {
                    FindLockOnTarget();
                }
                else
                {
                    ResetTarget();
                }
                input.lockOn = false;
            }

            if (isFindTarget)
            {
                if (currentTarget == null)
                {
                    ResetTarget();
                    return;
                }
                if(IsTargetRange())
                {
                    LookAtTarget();
                }
                else
                {
                    ResetTarget();
                }

                if(input.lockOnPrevious)
                {
                    ChangePreviousTarget();
                    input.lockOnPrevious = false;
                }
                if(input.lockOnNext)
                {
                    ChangeNextTarget();
                    input.lockOnNext = false;
                }
            }
            
        }

        // Sphere 반경 안에 있는 Enemy 컴포넌트를 가진 개체들을 TargetEnemies 안에 집어넣음
        private void FindLockOnTarget()
        {
            Collider[] findTargets = Physics.OverlapSphere(transform.position, lockOnRadius, targetLayer);

            if (findTargets.Length <= 0)
            {
                return;
            }

            foreach (Collider findTarget in findTargets)
            {
                LockOnTarget target = findTarget.GetComponent<LockOnTarget>();

                if(target != null)
                {
                    
                    Vector3 targetDirection = target.transform.position - transform.position;

                    float viewAngle = Vector3.Angle(targetDirection, mainCamera.transform.forward);

                    if (viewAngle > minViewAngle && viewAngle < maxViewAngle)
                    {
                        RaycastHit hit;

                        if(Physics.Linecast(transform.position, target.lockOnTarget.transform.position, out hit, targetLayer))
                        {
                            targetEnemies.Add(target);
                        }
                    }
                    else
                    {
                        ResetTarget();
                    }
                }            
            }

            LockOnTarget();
        }

        // targetEnemies를 순회하면서 가장 가까이 있는 애를 락온 대상으로 지정함
        private void LockOnTarget()
        {
            float shortDistance = Mathf.Infinity;

            for(int i = 0; i < targetEnemies.Count; i++)
            {
                if(targetEnemies[i] != null)
                {
                    float distanceFromTarget = Vector3.Distance(transform.position, targetEnemies[i].transform.position);

                    if (distanceFromTarget < shortDistance)
                    {
                        shortDistance = distanceFromTarget;
                        currentTarget = targetEnemies[i];
                        currentIndex = i;
                    }
                }
                else
                {
                    ResetTarget();
                }
            }
            if (currentTarget != null)
            {
                isFindTarget = true;
                lockOnImage.gameObject.SetActive(true);
            }
        }

        private void LookAtTarget()
        {
            if (currentTarget == null)
            {
                ResetTarget();
                return;
            }

            Vector3 currentTargetPosition = currentTarget.lockOnTarget.transform.position;
            lockOnImage.position = Camera.main.WorldToScreenPoint(currentTargetPosition);

            Vector3 dir = (currentTargetPosition - transform.position).normalized;
            //dir.y = transform.position.y;

            if (!animator.GetBool("Interacting"))
            {
                transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * lookAtSmoothing);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            }
        }

        private void ChangeNextTarget()
        {
            if (currentIndex < targetEnemies.Count - 1)
            {
                currentIndex++;
            }
            else
            {
                currentIndex = 0;
            }

            currentTarget = targetEnemies[currentIndex];
        }

        private void ChangePreviousTarget()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
            }
            else
            {
                currentIndex = targetEnemies.Count - 1;
            }

            currentTarget = targetEnemies[currentIndex];
        }

        private void ResetTarget()
        {
            isFindTarget = false;
            currentTarget = null;
            targetEnemies.Clear();

            lockOnImage.gameObject.SetActive(false);
        }

        private bool IsTargetRange()
        {
            if (!isLockOn)
            {
                ResetTarget();
                return false;
            }

            float distance = (transform.position - currentTarget.transform.position).magnitude;
            if (distance > lockOnRadius)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, lockOnRadius);
        }
    }
}

