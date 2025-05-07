
using Photon.Pun;
using System.Collections;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Realtime;
using PlayerControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using RPGCharacterAnims.Lookups;

namespace PlayerControl
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(InputHandler))]
    [RequireComponent(typeof(AnimationHandler))]

    public class PlayerController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]   
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        public AudioClip SwordClip;
        public AudioClip GreatswordClip;
        public AudioClip CrossbowClip;

        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
        [Range(0, 1)] public float AttackAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Header("LockOn")]
        public float _cameraDirectionY = -0.3f;
        public float _cameraSmoothing = 50;
        public float _lockOnSpeed = 2.5f;

        [Header("Item")]
        public float _useItemSpeed = 1.5f;

        [Header("Stamina")]
        public float _sprintStaminaUsage = 10f;
        public float _dodgeStaminaUsage = 20f;
    
        [Header("Short Dash")]
        public float _dashDuration = 0.2f;
        public float _dashMaxAmount = 5f;
        private float _dashCurrentAmount = 0f;
        private Vector3 _dashDirection;
        private bool _IsDashing = false;

        [Header("Ghost Effect")]
        public float _ghostDuration = 0.15f;

        [Header("Die")]
        public float fadeTime = 3f;

        private bool _characterRollFreeze = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private bool test_useItem;

        private AnimationHandler animationHandler;
        private CharacterController _controller;
        private InputHandler _input;
        public GameObject mainCamera;
        private LockOn _lockOn;


        private PhotonView photonView;
        private bool photonIsMine = true;

        [SerializeField] PlayerGhostEffect playerGhostEffect;


        private const float _threshold = 0.01f;

        protected virtual void Awake()
        {
            // get a reference to our main camera
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            photonView = GetComponent<PhotonView>();
        }

        public void SetMainCamera(GameObject mainCamera)
        {
            this.mainCamera = mainCamera;
            Debug.Log($"main camera is {this.mainCamera}");
        }

        protected virtual void Start()
        {
            CreateAfterImages();
            Debug.Log("PlayerController is Start");
            if (SceneController.Instance.GetCurrentSceneName() == "MultiPlayTestScene")
            {
                if (!photonView.IsMine)
                {
                    photonIsMine = false;
                    GetComponent<InputHandler>().enabled = false;
                    GetComponentInChildren<PlayerTrigger>().enabled = false;
                    GetComponent<CharacterController>().enabled = false;
                    GetComponent<LockOn>().enabled = false;
                    GetComponent<PlayerInput>().enabled = false;
                    GetComponent<PlayerInput>().enabled = false;
                    this.enabled = false;
                }
            }

            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            animationHandler = GetComponent<AnimationHandler>();
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<InputHandler>();
            _lockOn = GetComponent<LockOn>();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            
            /*if(SceneController.Instance.GetCurrentSceneName() == "MultiPlayTestScene")
            {

                if (!photonView.IsMine) {
                    Destroy(GetComponentInChildren<PlayerTrigger>());
                    Destroy(GetComponent<CharacterController>());
                    Destroy(GetComponent<InputHandler>());
                    Destroy(GetComponent<LockOn>());
                    Destroy(GetComponent<PlayerInput>());
                    Destroy(GetComponent<PlayerAttacker>());
                    
                    
                    
                    Destroy(this); }
            }*/
        }

        public void SetCinemachineTarget(GameObject target)
        {
            CinemachineCameraTarget = target;
        }

        void CreateAfterImages()
        {
            playerGhostEffect = GetComponent<PlayerGhostEffect>();
            playerGhostEffect.Setup(transform.GetComponentInChildren<SkinnedMeshRenderer>(), 10, _ghostDuration);
        }

        protected virtual void Update()
        {
            /*if(SceneController.Instance.GetCurrentSceneName() == "MultiPlayTestScene")
            {
                if (photonView.IsMine)
                {
                    if (PlayerState.Instance.state == PlayerState.State.Die) return;
                    Move();
                    GroundedCheck();
                    UseItem();
                    PickUp();
                    JumpAndGravity();
                    Rolling();
                    Attack();
                }
            }
            else
            {
                
            }*/
            if (PlayerState.Instance.state == PlayerState.State.Die) return;
            Move();
            GroundedCheck();
            UseItem();
            PickUp();
            JumpAndGravity();
            Rolling();
            Attack();
            /*if (photonView.IsMine) 
            {
                //_hasAnimator = TryGetComponent(out _animator);

                JumpAndGravity();
                GroundedCheck();
                Move();
                Rolling();
            }*/





        }

        protected virtual void LateUpdate()
        {
            if(SceneController.Instance.GetCurrentSceneName() == "MultiPlayTestScene")
            {
                if (photonView.IsMine)
                {
                    CameraRotation();
                }
            }
            else
            {
                CameraRotation();
            }
            
        }

        protected void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            animationHandler.SetBool(AnimationHandler.AnimParam.Grounded, Grounded);
        }

        protected void CameraRotation()
        {
            /*Debug.Log($"test {_input.look.sqrMagnitude}");
            Debug.Log($"test {_threshold}");
            Debug.Log($"test {LockCameraPosition}");*/
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = 1.0f;
                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);


            if (_lockOn.isFindTarget && _lockOn.currentTarget != null && !animationHandler.GetBool(AnimationHandler.AnimParam.Interacting))
            {
                Vector3 direction = (_lockOn.currentTarget.lockOnTarget.transform.position - CinemachineCameraTarget.transform.position).normalized;
                direction.y = _cameraDirectionY;

                CinemachineCameraTarget.transform.forward = Vector3.Lerp(CinemachineCameraTarget.transform.forward, direction, Time.deltaTime * _cameraSmoothing);

                Vector3 camAngle = mainCamera.transform.eulerAngles;
                _cinemachineTargetYaw = camAngle.y;
                _cinemachineTargetPitch = camAngle.x;
            }
            else 
            {
                // Cinemachine will follow this target
                CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                    _cinemachineTargetYaw, 0.0f);
            }            
        }

        protected void Move()
        {   
            float Sprint = SprintSpeed;
            if (!Grounded) Sprint = MoveSpeed;
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed;
            if (_input.sprint && PlayerStatusController.Instance.curSp > 0)
            {
                targetSpeed = Sprint;
                PlayerStatusController.Instance.UseStamina(_sprintStaminaUsage * Time.deltaTime);
            }
            else
            {
                targetSpeed = MoveSpeed;
            }

            // 감속처리
            if (_lockOn.isFindTarget) targetSpeed = Mathf.Clamp(targetSpeed, 0, _lockOnSpeed);
            if (test_useItem) targetSpeed = Mathf.Clamp(targetSpeed, 0, _useItemSpeed);

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                if(this.mainCamera == null) { this.mainCamera = DungeonGenerator.Instance.mainCmera; }
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);
                // rotate to face input direction relative to camera position
                if (!_lockOn.isFindTarget && !animationHandler.GetBool(AnimationHandler.AnimParam.Interacting))
                {
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            if (_IsDashing)
            {
                _controller.Move(_dashDirection * (_dashCurrentAmount * Time.deltaTime) +
                            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                return;
            }

            if (animationHandler.GetBool(AnimationHandler.AnimParam.Interacting)) _speed = 0;
        
            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            animationHandler.SetFloat(AnimationHandler.AnimParam.Speed, _animationBlend);
            animationHandler.SetFloat(AnimationHandler.AnimParam.MotionSpeed, inputMagnitude);
        }

        protected void Rolling()
        {
            if (_input.rolling)
            {
                print("Rolling Detected");
                _input.rolling = false;
                if (PlayerStatusController.Instance.curSp <= 0) return;
                if (animationHandler.GetBool(AnimationHandler.AnimParam.Blocking) || !Grounded) return;
                transform.rotation = Quaternion.Euler(0.0f, _targetRotation, 0.0f);
                _dashDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                animationHandler.SetTrigger(AnimationHandler.AnimParam.Rolling);
                PlayerStatusController.Instance.UseStamina(_dodgeStaminaUsage);
                StartCoroutine(SmallDash());
                /*
                animationHandler.RootMotion(true);
                animationHandler.SetBool(AnimationHandler.AnimParam.Interacting, true);
                animationHandler.SetBool(AnimationHandler.AnimParam.Blocking, true);
                PlayerState.Instance.state = PlayerState.State.Invincible;*/
            }
        }

        protected void Attack()
        {
            if (_input.attack)
            {
                WeaponStats weapon = InventoryController.Instance.weaponPanel.GetWeapon();
                print(weapon);
                _input.attack = false;
                if (!Grounded) return;
                if (PlayerStatusController.Instance.curSp <= 0) return;
                if(weapon == null) return;
                if (!animationHandler.GetBool(AnimationHandler.AnimParam.Blocking)
                    || animationHandler.GetBool(AnimationHandler.AnimParam.CanDoCombo))
                {
                    if (!weapon.isRanged) animationHandler.SetTrigger(AnimationHandler.AnimParam.Attack);
                    else animationHandler.SetTrigger(AnimationHandler.AnimParam.RangedAttack);

                    animationHandler.RootMotion(true);
                    animationHandler.SetBool(AnimationHandler.AnimParam.Interacting, true);
                    animationHandler.SetBool(AnimationHandler.AnimParam.Blocking, true);
                    animationHandler.SetBool(AnimationHandler.AnimParam.Attacking, true);
                    PlayerStatusController.Instance.UseStamina(weapon.staminaUsage);
                }
            }
        }

        public void PlayAttackClip()
        {
            WeaponStats weapon = InventoryController.Instance.weaponPanel.GetWeapon();
            if (weapon == null) return;
            switch (weapon.weaponType)
            {
                case "OneHand":
                    AudioSource.PlayClipAtPoint(SwordClip, transform.TransformPoint(_controller.center), AttackAudioVolume);
                    break;

                case "TwoHand":
                    AudioSource.PlayClipAtPoint(GreatswordClip, transform.TransformPoint(_controller.center), AttackAudioVolume);
                    break;

                case "Crossbow":
                    AudioSource.PlayClipAtPoint(CrossbowClip, transform.TransformPoint(_controller.center), AttackAudioVolume);
                    break;
            }
        }

        protected void UseItem()
        {
            if (_input.useItem)
            {
                _input.useItem = false;
                // 대충 아이템 확인하는 조건문 들어가야함
                if (animationHandler.GetBool(AnimationHandler.AnimParam.Blocking) || !Grounded) return;
                animationHandler.SetTrigger(AnimationHandler.AnimParam.UseItem);
                animationHandler.SetBool(AnimationHandler.AnimParam.Blocking, true);
                test_useItem = true;
            }
        }

        protected void PickUp()
        {
            if (_input.pickup)
            {
                _input.pickup = false;
                if (animationHandler.GetBool(AnimationHandler.AnimParam.Blocking) || !Grounded) return;
                animationHandler.SetTrigger(AnimationHandler.AnimParam.PickUp);
                animationHandler.SetBool(AnimationHandler.AnimParam.Interacting, true);
                animationHandler.SetBool(AnimationHandler.AnimParam.Blocking, true);
                WeaponStats weapon;
                weapon = InventoryController.Instance.weaponPanel.GetWeapon();
            }
        }

        // state machine
        public void TestUseItemEnd()
        {
            test_useItem = false;
        }
        protected IEnumerator SmallDash()
        {
            float delta = 0;
            _IsDashing = true;

            playerGhostEffect.Create(true);
            animationHandler.SetBool(AnimationHandler.AnimParam.Interacting, true);
            animationHandler.SetBool(AnimationHandler.AnimParam.Blocking, true);
            PlayerState.Instance.state = PlayerState.State.Invincible;
            
            while (delta < _dashDuration)
            {
                _dashCurrentAmount = Mathf.Lerp(0, _dashMaxAmount, delta / _dashDuration);
                delta += Time.deltaTime;
                yield return null;
            }
            CancelDash();
        }

        protected void CancelDash()
        {
            playerGhostEffect.Create(false);
            _IsDashing = false;

            animationHandler.SetBool(AnimationHandler.AnimParam.Interacting, false);
            animationHandler.SetBool(AnimationHandler.AnimParam.Blocking, false);
            animationHandler.ResetTrigger(AnimationHandler.AnimParam.Rolling);
            PlayerState.Instance.state = PlayerState.State.Idle;
        }

        protected void JumpAndGravity()
        {
            if (animationHandler.GetBool(AnimationHandler.AnimParam.Blocking)) 
            {
                _input.jump = false;
                return;
            }
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                animationHandler.SetBool(AnimationHandler.AnimParam.Jump, false);
                animationHandler.SetBool(AnimationHandler.AnimParam.FreeFall, false);

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    animationHandler.SetBool(AnimationHandler.AnimParam.Jump, true);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    animationHandler.SetBool(AnimationHandler.AnimParam.FreeFall, true);
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        #region Dying
        public void DeathTrigger()
        {
            PlayerState.Instance.state = PlayerState.State.Die;
            StopAllCoroutines();
            StartCoroutine(DecayAndVanish(fadeTime));
        }
        protected IEnumerator DecayAndVanish(float fadeTime)
        {
            /*
            SkinnedMeshRenderer rend = GetComponentInChildren<SkinnedMeshRenderer>();
            if (rend == null) yield break;

            Material mat = rend.material;
            Color originalColor = mat.color;

            // URP setting
            mat.SetFloat("_Surface", 1); // 1 = Transparent
            mat.SetFloat("_Blend", 0); // Alpha blend
            mat.SetFloat("_ZWrite", 0);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;*/

            float t = 0f;
            while (t < fadeTime)
            {
                /*
                float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
                mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);*/
                t += Time.deltaTime;
                yield return null;
            }

            // Add Death Event Here
    
        }
        #endregion
        public void ForceJumpStop()
        {
            animationHandler.SetBool(AnimationHandler.AnimParam.Blocking, false);
            _input.jump = false;
            _jumpTimeoutDelta = 0;
            _fallTimeoutDelta = FallTimeout;
        }

        protected static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        protected void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        protected void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        protected void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}
