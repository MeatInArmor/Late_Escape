using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class Enemy : MonoBehaviour
{   ////
    // характеристики врага
    public int maxHP;                                                   // максимальное здоровье
    public int currentHP;                                               // текущее здоровье       
    public int damage;                                                  // урон (средний)
    public float magicDammage;                                          // магический урон      
    public int deffence;                                                // блокируемый урон (в единицах)
    public float attackDistance;                                        // максимальная дальность атаки
    public bool isDefeated;                                             // персонаж побеждён
    public float moveSpeed;                                             // множитель скорости передвижения
    public float moveDistance;                                          // до каких пор нужно двигаться к персонажу (минимальная дальность атаки)

    public bool inFight;                                                // становится true, когда заметил игрока или получил урон
    public bool inAttackZone;                                           // true, когда игрок в пределах attackDistance


    public float distanceOfView;                                        // дальность обзора (область нахождения персонажа)
    [Range(0, 360)] public float angleOfView;                           // угол обзора
    public float detectionDistance;                                     // дальность абсолютного обнаружения        // можно эту же переменную сделать как stopDistance 

    // для отслеживания игрока
    public Transform enemyEye;                                          // "глаз", из которого ведётся наблюдение
    public GameObject target;                                           // цель, за которой следит
    public float distanceToPlayer;

    // для следования за врагом
    private NavMeshAgent agent;                                         // передвижение + поиск пути
    private float rotationSpeed;                                        // скорость поворота
    private Transform agentTransform;                                   // для получения ссылки на объект
    private Vector3 direction;
    private float stopDistance;                                         //для остановки после преследования


    //время выполнения способностей
    public float normalAttackTimeout;                                  // перезарядка обычной атаки
    public float magicCastTimeout;                                     // перезарядка магии
    public float deadTimeout;                                          // время до уничтожения после поражения
    ////

    // для определения самого врага
    public Enemy enemy;                                                 // Объект - враг
    private Marker marker;                                              // объект - выделение при наведении
    private Selection selection;                                        // объект - выделение цели атаки
    private Selection currentTarget;                                    // для текущей цели атаки (selection выбранный в данный момент)
    public Animator animator;

    // animation IDs (установка переменных внутри аниматора)
        private int _animIDAttack;                          
        private int _animIDSpeed;
        private int _animIDMotionSpeed;
        private int _animIDDefeated;
        private int _animIDCanMove;

 private void Awake()
    {
        //Выполняется для каждого враго по отдельности
        enemy = this.GetComponent<Enemy>();                             // запихиваем нашего врага впеременную
        marker = enemy.GetComponentInChildren<Marker>();                // круг наводки мышью
        selection = enemy.GetComponentInChildren<Selection>();          // круг выбора цели

        animator = this.GetComponent<Animator>();                       // запихиваем аниматор в переменную

        marker.gameObject.SetActive(false);                             // 
        selection.gameObject.SetActive(false);                          // делаем эти две штуки неактивными

        inFight = false;
        inAttackZone = false;

        //AssignAnimationIDs();
        SetCharacteristics();
        AssignAnimationIDs();
    }
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        rotationSpeed = agent.angularSpeed;
        agentTransform = agent.transform;
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()                                               // нахождение и перемещение к цели
    {
        distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
        if(!inFight)
        {
            if (distanceToPlayer <= detectionDistance || IsInView())
            {
                RotateToTarget();
                MoveToTarget();
                animator.SetFloat(_animIDSpeed, 5.335f);
                inFight = true;
            }
            else
                animator.SetFloat(_animIDSpeed, 0);
        }
        else
        {
            if (distanceToPlayer > attackDistance)
            {
                RotateToTarget();
                MoveToTarget();
            }
            else inAttackZone = true;
            if (inAttackZone) Attack();
        }
    }

    private void Attack()
    {
        //остановка
        //атака
        Dead();
    }

    private bool IsInView()                                             // true если цель видна
    {
        float realAngle = Vector3.Angle(enemyEye.forward, target.transform.position - enemyEye.position);
        RaycastHit hit;
        if (Physics.Raycast(enemyEye.transform.position, target.transform.position - enemyEye.position, out hit, distanceOfView))
        {
            if (realAngle < angleOfView / 2f && Vector3.Distance(enemyEye.position, target.transform.position) <= distanceOfView /*&& hit.transform == target.transform*/)
            {
                return true;
            }
        }
        return false;
    }

    private void RotateToTarget()                                       //поворот в сторону цели
    {
        direction = target.transform.position - transform.position;
        Quaternion _rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, Time.deltaTime * rotationSpeed);

        // Vector3 lookVector = target.transform.position - agentTransform.position;
        // lookVector.y = 0;
        // if (lookVector == Vector3.zero) return;
        // agentTransform.rotation = Quaternion.RotateTowards
        //     (
        //         agentTransform.rotation,
        //         Quaternion.LookRotation(lookVector, Vector3.up),
        //         rotationSpeed * Time.deltaTime
        //     );
    }

    private void MoveToTarget()
    {
        agent.SetDestination(target.transform.position);
    }

   

    private void SetCharacteristics()
    {
        ////////// установка характеристик при появлении
        // если враг - красный фермер он получает свои характеристики
        if (gameObject.GetComponent<DevilFarmer>())
        {
            maxHP = 10;
            currentHP = 10;
            deadTimeout = 5;

            distanceOfView = 15f;
            angleOfView = 90f;
            detectionDistance = 3f;
            attackDistance = 3f;

            // сюда добавлять остальные характеристики
        }
        if (gameObject.GetComponent<HellWolf>())
        {
            maxHP = 20;
            currentHP = 10;
            deadTimeout = 5;

            distanceOfView = 15f;
            angleOfView = 90f;
            detectionDistance = 3f;

            // сюда добавлять остальные характеристики
        }

    }
    private void OnMouseOver()                          // когда курсор вошёл в область колайдера (физической части) объекта 
    {
        if (Time.timeScale == 1f)
        {
            marker.gameObject.SetActive(true);              // сделать круг наводки активным
            if (Input.GetMouseButtonDown(1))                 // если при этом щёлкнуть ПКМ...
                RightClick();
        }
    }

    private void OnMouseExit()                          // когда курсор вышел из области колайдера (физической части) объекта 
    {
        if (Time.timeScale == 1f)
        {
            marker.gameObject.SetActive(false);           // сделать круг наводки неактивным
        }
    }

    private void RightClick()
    {
        if (Time.timeScale == 1f)
        {
            enemy = this.GetComponent<Enemy>();             // снова запихиваем этот объект в переменную ибо она сбросилась
            if (CharacterValues.enemyCurrentTarget == null)  // если игрок не выбрал цель
            {
                selection.gameObject.SetActive(true);       // сделать активным круг выбора врага
                CharacterValues.enemyCurrentTarget = enemy; // сделать этого врага целью игрока             
            }
            else                                            // если игрок уже выбирал цель и она жива (когда умирает сама сбрасывается)
            {
                currentTarget = CharacterValues.enemyCurrentTarget.GetComponentInChildren<Selection>(); // выбрать круг выбора цели игрока (великий и могучий Русский язык)
                currentTarget.gameObject.SetActive(false);                                              // сделать его неактивным
                CharacterValues.enemyCurrentTarget = enemy;                                             // сделать этого врага целью игрока  
                selection.gameObject.SetActive(true);                                                   // сделать активным круг выбора врага
            }
        }
    }
    private void AssignAnimationIDs()
        {
            _animIDAttack = Animator.StringToHash("Attack");
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDCanMove = Animator.StringToHash("CanMove");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }
    public void Dead()
    {
        animator.SetTrigger("Defeated");
        Invoke("Destroy", deadTimeout);
    }
    public void Destroy()
    {
        Destroy(enemy.gameObject);
    }
}