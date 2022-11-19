using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class CharacterValues : MonoBehaviour
{
	public AnimatorController[] _characterController; 
    public GameObject[] _characterModel;

 // Структура персоанжей с характеристиками
    public static Character[] character = new Character[4]; // 0 - фермер; 1 - дочь; 2 - урядник; 3 - воин
    public struct Character
    {
        //public int currentLevel; //уровень персонажа
        //public int levelPoints;  //опыт персонажа для увеличения уровня
        public int maxHP; // максимальное здоровье
        public int currentHP; // текущее здоровье
        //public int maxStamina; // максимальная выносливость
        //public int currentStamina; // текущая выносливость
        //public int maxMana; // максимальная мана
        //public int currentMana; //текущая мана
        public int damage; // урон (средний)
        public float magicDammage; // множитель урона магии (пока что от обычного урона)
        //public float chargeAttackDammageMultiplier; // множитель урона сильной атаки
        public int deffence; // блокируемый урон (в единицах)
        public bool isDefeated; // персонаж побеждён
        public float moveSpeedMultiplier; //множитель скорости передвижения
    }  

    // игровые аспекты
    public static bool canMove; // становится false во время атаки, сильной атаки, магии, переключения персонажа, поражения 
    public static bool isCameraRotationBlocked; // остановка поворота камеры при движении мышью при true

    // общие сведения команды
    public static int teamMembers; // всего членов команды в текущий момент
    public static int currentTeamMember; // номер текущего члена отряда
    public static float teamMemberChangeCD; // время перезарядки смены персонажа
    public static float teamMemberChangeCDDeltaTime; // время до следующей смены персонажа
    public static bool inFightMode; // находится ли персонаж в состоянии боя
    
    // Взаимодействия со врагами
    public static List<Enemy> enemyInFight = new List<Enemy>(); // список врагов, с которыми сражаемся в данный момент 
    //public static List<GameObject> enemyInFight = new List<GameObject>(); 
    public static Enemy enemyCurrentTarget; // текущая цель атаки
    //public static GameObject enemyCurrentTarget;
    public static float distanceToEnemyCurrentTarget; //дистанция до цели атаки

private void Awake()
{
    canMove = true;
    isCameraRotationBlocked = false;

    teamMembers = 1;
    currentTeamMember = 1;
    teamMemberChangeCD = 5;
    teamMemberChangeCDDeltaTime = 0;
    inFightMode = false;

    CharacterValues.character[0].maxHP = 40;
    CharacterValues.character[0].currentHP = 40;
    CharacterValues.character[0].damage = 8;
    CharacterValues.character[0].magicDammage = 12;    
    CharacterValues.character[0].deffence = 2;
    CharacterValues.character[0].isDefeated = false;
    CharacterValues.character[0].moveSpeedMultiplier = 1;
}
}
