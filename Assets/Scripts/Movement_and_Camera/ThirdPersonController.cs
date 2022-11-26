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
        public float MoveSpeed = 2.0f;                              // потом заменим на переменную из CharacterValues

        [Tooltip("Скорость бега персонажа в м/с (заменим на скорость передвижения в бою)")]
        public float SprintSpeed = 5.335f;                          // потом заменим на переменную из CharacterValues

        [Tooltip("Как быстро персонаж поворачивается лицом в направлении движения")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Ускорение и замедление")]
        public float SpeedChangeRate = 10.0f;

        

        [Tooltip("Персонаж использует собственное значение гравитации (Значение по умолчанию -9.81f)")]
        public float Gravity = -15.0f;


        [Header("Timeouts")]
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
        

        [Header("Sounds")]
        [Tooltip("Звуки ходьбы")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        // cinemachine and camera
        private float _cinemachineTargetYaw;                    // отвечает за x координату поворота камеры 
        private float _cinemachineTargetPitch;                  // отвечает за x координату поворота камеры 

        // player movement
        private float _speed;                                   // скорость
        private float _animationBlend;                          // вроде скорость изменения скорости (великий и могучий Русский язык)
        private float _targetRotation = 0.0f;                   // хз что это
        private float _rotationVelocity;                        //
        private float _verticalVelocity;                        // скорости по вертикали, горизонтали и Z
        private float _terminalVelocity = 53.0f;                //

        // timeouts deltatime
        //private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;                          // время падения (пока не удаляем)

        private float _normalAttackTimeoutDelta;                  // перезарядка обычной атаки
        //private float _magicCastTimeoutDelta;                   // перезарядка магии

        //private float _dodgeDurationTimeoutDelta;               // оставшаяся длительность уклонения (неуязвимость)
        //private float _dodgeCastingTimeoutDelta;                // время до конца возможности уклонения (нажатие кнопок)
        //private float _dodgeReloadingTimeoutDelta;              // перезарядка уклонения
        //private float _dodgeModeReloadingTimeoutDelta;          // перезарядка переключения режима уклонения

        //timeouts 

        // animation IDs (установка переменных внутри аниматора)
        private int _animIDAttack;                          
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDCanMove;
        private int _animIDDefeated;

        //boolean
        private bool _canMove;                                  // если true, двигаться можно
        private Vector3 direction;
        //создать статический класс со статическими переменными

        private float distance;                                 // расстояние между игроком и целью

        #if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;                       // сама система движения
        #endif
        private Animator _animator;                             // аниматор
        private CharacterController _controller;                // контроллер в аниматоре
        private StarterAssetsInputs _input;                     // настройки нажатия кнопок в PlayerInput
        private GameObject _mainCamera;                         // камера
        private const float _threshold = 0.01f;                 // время до начала поворота камеры при движении мышью?
        private bool _hasAnimator;                              // есть аниматор или нет

        // проверка управляется ли мышью
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
            
            _hasAnimator = TryGetComponent(out _animator);              // если на объекте есть аниматор, то _hasAnimator = true, _animator = этот самый аниматор
            _controller = GetComponent<CharacterController>();          // на объекте ищется и берётся класс CharacterController
            _input = GetComponent<StarterAssetsInputs>();               // на объекте ищется и берётся класс StarterAssetsInputs
            #if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                _playerInput = GetComponent<PlayerInput>();             // на объекте ищется и берётся класс PlayerInput
            #else
			    Debug.LogError( "В пакете Starter Assets отсутствуют зависимости. Пожалуйста, используйте Tools/Starter Assets/Reinstall Dependencies, чтобы исправить это.");
            #endif

            AssignAnimationIDs();

            // сбросить наши тайм-ауты при запуске
            _fallTimeoutDelta = FallTimeout;                            // время падения
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);              // снова проверяем наличиа аниматора, мбо значения сбросились (наверное)
            CanMoveCheck();
            Move();
            Attack();
            JumpAndGravity();
            GroundedCheck();
            //Debug.Log(_animationBlend);
        }
        private void LateUpdate()
        {
            CameraRotation();
        }
        // изменяет возможность перезвигаться. Передвигаться нельзя во время атаки, магии, уклонения и прочего
        private void CanMoveCheck()
        {
            if(_normalAttackTimeoutDelta > 0)                      // проверка, что атака ещё не закончена
            {   if(CharacterValues.enemyCurrentTarget != null)
                {
                    direction = CharacterValues.enemyCurrentTarget.transform.position - transform.position;
                    Quaternion _rotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, Time.deltaTime * CharacterValues.character[CharacterValues.currentTeamMember].rotationSpeed);
                    //Debug.Log(CharacterValues.canMove + " 1");
                }
                _normalAttackTimeoutDelta -= Time.deltaTime;        // уменьшение оставшегося времени атаки (отнимается разница во времени с прошлой проверки)
                if(_normalAttackTimeoutDelta <= 0)                  // если атака завершилась
                {
                    //Debug.Log(CharacterValues.canMove + " 2");
                    CanMoveChanger();   
                }
            }
        }

        private void CanMoveChanger()
        {
            if(_normalAttackTimeoutDelta <= 0)                      // если конкретно атакака завершилась (потом добавится конкретно магия и прочее)
            { 
                CharacterValues.canMove = true;                     // разрешаем двигаться
                _animator.SetBool(_animIDCanMove, true);            // переключаем анимацию в аниматоре, изменяя значение преременной там
            }
        }

        private void Attack()
        {
            if(Input.GetKeyDown(KeyCode.Mouse0) && CharacterValues.canMove)     // если нажата ЛКМ и персонаж может двигаться
            {
                CharacterValues.canMove = false;                                // то он больше не может двигаться
                _animationBlend = 0;                                            // зануляем скорость изменение скорости движения 
                _animator.SetFloat(_animIDSpeed, 0);                            // зануляем саму скорость в аниматоре
                _animator.SetBool(_animIDCanMove, false);                       // запрещаем ходить в аниматоре
                _normalAttackTimeoutDelta = CharacterValues.character[CharacterValues.currentTeamMember].normalAttackTimeout;   // ставим длительность атаки равной КД атаки выбранного персонажа
                //Debug.Log(CharacterValues.character[CharacterValues.currentTeamMember].normalAttackTimeout);
                //Debug.Log(CharacterValues.currentTeamMember);
                _animator.SetTrigger("Attack");                                 // включаем анимацию атаки
                //Debug.Log(CharacterValues.enemyCurrentTarget);
                if(CharacterValues.enemyCurrentTarget != null)                  // если цель выбрана (ссылка не на null, а на Enemy)
                    if(DistanceChecker())                                       // проверка находится ли выбранный враг в радиусе атаки
                    {   

                        // надо добавить поворот в сторону врага
                        float time = CharacterValues.character[CharacterValues.currentTeamMember].normalAttackTimeout / 1.5f; // задержка перед непосредственно нанесением урона
                        //Debug.Log(time);
                        Invoke("Hitting", time);                                // отложенное исполнение функции Hitting
                    }
            }
        }
        // Нанесение урона
        public void Hitting()
        {   
            Enemy enemy = CharacterValues.enemyCurrentTarget;                    // берём выбранного врага
            // надо усложнить рандомизацией урона и защитой врага
            enemy.currentHP -= CharacterValues.character[CharacterValues.currentTeamMember].damage;  // отнимаем его хп в количестве урона атаки 
            if(enemy.currentHP <= 0)                                             // если хп закончилось
            {
                enemy.Dead();
                CharacterValues.enemyCurrentTarget = null;
            }
            
            //Destroy(enemy.gameObject);                                           // уничтожаем врага
            Debug.Log(enemy.currentHP);
            //Debug.Log(CharacterValues.enemyCurrentTarget);
        }
        public bool DistanceChecker()
        {   
            
            distance = Vector3.Distance(CharacterValues.enemyCurrentTarget.transform.position, transform.position); // расстояние от игрока до выбранного врага
            //Debug.Log(distance);
            if(distance <= CharacterValues.character[CharacterValues.currentTeamMember].attackDistance)             // если враг в радиусе атаки
                return true;
            else 
                return false;

        }
  
        // просто засовываем переменные из аниматора в переменные этого скрипта
        private void AssignAnimationIDs()
        {
            _animIDAttack = Animator.StringToHash("Attack");
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDCanMove = Animator.StringToHash("CanMove");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDDefeated = Animator.StringToHash("Defeated");
            
        }
////////////////////////////////////////////////////////////
// всё что ниже можно не читать, оно было встроенно в ассет


        private void GroundedCheck()            // зх, возможно корректное приземление на землю
        {
            // установить положение сферы со смещением
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // если двигаеися мышка и положение камеры не фиксировано
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Ввод с помощью мыши не умножается на Time.deltaTime; (вроде если этого не делать, то на мощных компах значение будет в разы больше)
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;           // изменение направление вида камеры по X и Y
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;         
            }

            //фиксация значений вращения камеры, чтобы наши они были ограничены 360 градусам
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine будет следовать этой цели (кинемашина это модификация обычной камеры)
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        { 
            if(CharacterValues.canMove)                                                 // если двигаться можно (это уже наша доработка)
            {
            // установка целевую скорость на основе скорости движения, скорости спринта и нажатия кнопки спринта
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // упрощенное ускорение и замедление, разработанное таким образом, чтобы его можно было легко удалить, заменить или повторить

             // примечание: оператор Vector2 == использует аппроксимацию, поэтому не подвержен ошибкам с плавающей запятой и дешевле, чем величина
             // если нет ввода, устанавливаем целевую скорость на 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // ссылка на текущую горизонтальную скорость игрока
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;                                                   // множитель изменения скорости?
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;  // из той же оперы

            // ускоряться или замедляться до заданной скорости
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // создает изогнутый результат, а не линейный, что дает более органичное изменение скорости
                 // запись T в Lerp фиксируется, поэтому нам не нужно ограничивать скорость
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // скорость округляется до 3 знаков после запятой
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);       // скорость изменения скорости
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // нормализация направления ввода
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
/////////////////// дальше лень переводить
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
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // остановить бесконечное падение нашей скорости при заземлении 
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
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