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

    public float distanceOfView;                                        // дальность обзора (область нахождения персонажа)
    [Range(0, 360)] public float angleOfView;                           // угол обзора
    public float detectionDistance;                                     // дальность абсолютного обнаружения

    public Transform enemyEye;                                          // "глаз", из которого ведётся наблюдение
    public Transform target;                                            // цель, за которой следит

    private NavMeshAgent agent;                                         // передвижение + поиск пути
    private float rotationSpeed;                                        // скорость поворота
    private Transform agentTransform;                                   // для получения ссылки на объект


    //время выполнения способностей
    public float normalAttackTimeout;                                  // перезарядка обычной атаки
    public float magicCastTimeout;                                     // перезарядка магии
    public float deadTimeout;                                          // время до уничтожения после поражения
    ////

    public Enemy enemy;                                                 // Объект - враг
    private Marker marker;                                              // объект - выделение при наведении
    private Selection selection;                                        // объект - выделение цели атаки
    private Selection currentTarget;                                    // для текущей цели атаки (selection выбранный в данный момент)
    public Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        rotationSpeed = agent.angularSpeed;
        agentTransform = agent.transform;
        DrawViewState();
    }

    private void Update()                                               // нахождение и перемещение к цели
    {
        float distanceToPlayer = Vector3.Distance(target.transform.position, agent.transform.position);
        if (distanceToPlayer <= detectionDistance || IsInView())
        {
            RotateToTarget();
            MoveToTarget();
        }
    }

    private bool IsInView()                                             // true если цель видна
    {
        float realAngle = Vector3.Angle(enemyEye.forward, target.position - enemyEye.position);
        RaycastHit hit;
        if (Physics.Raycast(enemyEye.transform.position, target.position - enemyEye.position, out hit, distanceOfView))
        {
            if (realAngle < angleOfView / 2f && Vector3.Distance(enemyEye.position, target.position) <= distanceOfView /*&& hit.transform == target.transform*/)
            {
                return true;
            }
        }
        return false;
    }

    private void RotateToTarget()                                       //поворот в сторону цели
    {
        Vector3 lookVector = target.position - agentTransform.position;
        lookVector.y = 0;
        if (lookVector == Vector3.zero) return;
        agentTransform.rotation = Quaternion.RotateTowards
            (
                agentTransform.rotation,
                Quaternion.LookRotation(lookVector, Vector3.up),
                rotationSpeed * Time.deltaTime
            );
    }

    private void MoveToTarget()
    {
        agent.SetDestination(target.position);
    }

    private void DrawViewState()
    {
        Vector3 left = enemyEye.position + Quaternion.Euler(new Vector3(0, angleOfView / 2f, 0)) * (enemyEye.forward * distanceOfView);
        Vector3 right = enemyEye.position + Quaternion.Euler(-new Vector3(0, angleOfView / 2f, 0)) * (enemyEye.forward * distanceOfView);
        Debug.DrawLine(enemyEye.position, left, Color.black);
        Debug.DrawLine(enemyEye.position, right, Color.black);
    }

    private void Awake()
    {

        //Выполняется для каждого враго по отдельности
        enemy = this.GetComponent<Enemy>();                             // запихиваем нашего врага впеременную
        marker = enemy.GetComponentInChildren<Marker>();                // круг наводки мышью
        selection = enemy.GetComponentInChildren<Selection>();          // круг выбора цели

        animator = this.GetComponent<Animator>();

        marker.gameObject.SetActive(false);                             // 
        selection.gameObject.SetActive(false);                          // делаем эти две штуки неактивными
        //AssignAnimationIDs();
        SetCharacteristics();
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
            detectionDistance = 2f;

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
    // private void AssignAnimationIDs()
    //     {
    //         _animIDAttack = Animator.StringToHash("Attack");
    //         _animIDSpeed = Animator.StringToHash("Speed");
    //         _animIDCanMove = Animator.StringToHash("CanMove");
    //         _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    //     }
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