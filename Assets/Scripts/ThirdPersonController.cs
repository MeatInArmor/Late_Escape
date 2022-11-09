using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Примечание: анимация вызывается через контроллер как для персонажа, так и для капсулы с использованием нулевых проверок аниматора.

!!! - это то, что нам скорей всего не понадобится
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Скорость передвижения персонажа в м/с")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Скорость бега персонажа в м/с (заменим на скорость передвижения в бою)")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Как быстро персонаж поворачивается лицом в направлении движения")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Ускорение и замедление")]
        public float SpeedChangeRate = 10.0f;

        

        // [Space(10)]
        // [Tooltip("!!! Высота, на которую может прыгнуть игрок !!!")]
        // public float JumpHeight = 1.2f;

        [Tooltip("Персонаж использует собственное значение гравитации (Значение по умолчанию -9.81f)")]
        public float Gravity = -15.0f;

        // [Space(10)]
        // [Tooltip("Время, необходимое для того, чтобы снова прыгнуть. Установите 0f, чтобы снова мгновенно прыгнуть (используем, но не для прыжка)")]
        // public float JumpTimeout = 0.50f;

        [Tooltip("Время, необходимое для перехода в состояние падения. Полезно для спуска по лестнице")]
        public float FallTimeout = 0.15f;


        [Header("Grounding")]
        [Tooltip("Заземлен персонаж или нет. Не является частью встроенной проверки CharacterController")]
        public bool Grounded = true;

        [Tooltip("Полезно для неровной почвы !!")]
        public float GroundedOffset = -0.14f;

        [Tooltip("Радиус заземленной чеки. Должен соответствовать радиусу CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("Какие слои персонаж использует в качестве земли")]
        public LayerMask GroundLayers;


        [Header("Cinemachine")]
        [Tooltip("Цель слежения, установленная в виртуальной камере Cinemachine, за которой будет следовать камера.")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("На сколько градусов можно поднять камеру")]
        public float TopClamp = 70.0f;

        [Tooltip("На сколько градусов можно опустить камеру")]
        public float BottomClamp = -30.0f;

        [Tooltip("Дополнительные градусы для переопределения камеры. Полезно для точной настройки положения камеры в заблокированном состоянии.")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("!!! Для фиксации положения камеры по всем осям")]
        public bool LockCameraPosition = false;


        [Header("Сharacteristics")]
        [Tooltip("Максимальное здоровье")]
        public int HealthMax = 100;

        [Tooltip("Текущее здоровье")]
        public int HealthCurrent = 100;

        [Tooltip("Сила обычной атаки")]
        public int NormalAttackStrength = 10;

        [Tooltip("Магическая сила")]
        public int MagicStrength = 10;


        [Header("Sounds")]
        [Tooltip("Звуки ходьбы")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;


        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player movement
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        //private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _stopMoveingTimeoutDelta;

        private float _normalAttackTimeoutDelta;
        private float _magicCastTimeoutDelta;

        private float _dodgeTimeTimeoutDelta;
        private float _dodgeReloadingTimeoutDelta;
        private float _dodgeModeReloadingTimeoutDelta;



        // animation IDs
        private int _animIDAttack;
        private int _animIDSpeed;
        private int _animIDGrounded;
       // private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            //получить референс (ссылку) на нашу основную камеру
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "В пакете Starter Assets отсутствуют зависимости. Пожалуйста, используйте Tools/Starter Assets/Reinstall Dependencies, чтобы исправить это.");
#endif

            AssignAnimationIDs();

            // сбросить наши тайм-ауты при запуске
           // _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            Attack();
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void Attack()
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                _animator.SetTrigger("Attack");
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDAttack = Animator.StringToHash("Attack");
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
           // _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // установить положение сферы со смещением
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // обновить аниматор, если используется персонаж
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // если есть вход и положение камеры не фиксировано
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Не умножайте ввод с помощью мыши на Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            //зафиксируйте наши вращения, чтобы наши значения были ограничены на 360 градусов
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine будет следовать этой цели
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // установить целевую скорость на основе скорости движения, скорости спринта и нажатия кнопки спринта
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // упрощенное ускорение и замедление, разработанное таким образом, чтобы его можно было легко удалить, заменить или повторить

             // примечание: оператор Vector2 == использует аппроксимацию, поэтому не подвержен ошибкам с плавающей запятой и дешевле, чем величина
             // если нет ввода, устанавливаем целевую скорость на 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // ссылка на текущую горизонтальную скорость игроков
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // ускоряться или замедляться до заданной скорости
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // создает изогнутый результат, а не линейный, что дает более органичное изменение скорости
                 // запись T в Lerp фиксируется, поэтому нам не нужно ограничивать скорость
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // скорость округления до 3 знаков после запятой
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // нормализовать направление ввода
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // примечание: оператор Vector2 != использует аппроксимацию, поэтому не подвержен ошибкам с плавающей запятой и дешевле, чем величина
             // если есть ход, поверните игрока, когда игрок движется
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // повернуть лицом к направлению ввода относительно положения камеры
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // переместить игрока
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            //обновить аниматор, если используется персонаж
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // сбросить таймер тайм-аута падения (опять же, у нас будет не прыжки)
                _fallTimeoutDelta = FallTimeout;

                // обновить аниматор, если используется персонаж
                if (_hasAnimator)
                {
                    //_animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // остановить бесконечное падение нашей скорости при заземлении 
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                // if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                // {
                //     // квадратный корень из H * -2 * G = скорость, необходимая для достижения желаемой высоты
                //     _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                //     // обновить аниматор, если используется персонаж
                //     if (_hasAnimator)
                //     {
                //         _animator.SetBool(_animIDJump, true);
                //     }
                // }

                // тайм-аут прыжка
                // if (_jumpTimeoutDelta >= 0.0f)
                // {
                //     _jumpTimeoutDelta -= Time.deltaTime;
                // }
            }
            else
            {
                // сбросить таймер тайм-аута перехода
                //_jumpTimeoutDelta = JumpTimeout;

                // падение тайм-аут
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // обновить аниматор, если используется персонаж
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // !!! если мы не заземлены, не прыгайте !!!
                //_input.jump = false;
            }

            // применять гравитацию с течением времени, если вы находитесь под терминалом (умножьте на дельта-время дважды, чтобы линейно ускорить с течением времени)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // когда выбрано, нарисуйте гизмо (штуковину?) в положении и соответствующем радиусе заземленного коллайдера
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
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

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}