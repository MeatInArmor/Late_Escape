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
    public static List<Character> character = new List<Character>();
    public struct Character
    {
        public int currentLevel;
        public int levelPoints;
        public int maxHP;
        public int currentHP;
        //public int maxStamina;
        //public int currentStamina;
        //public int maxMana;
        //public int currentMana;
        public float magicDammageMultiplier;
        public float chargeAttackDammageMultiplier;
        public int dammage;
        public int deffence;
        public bool isDefeated;
        public float moveSpeedMultiplier;

    }

    // игровые аспекты
    public static bool canMove; // становится false во время атаки, сильной атаки, магии, переключения персонажа, поражения 
    public static bool isCameraRotationBlocked; // остановка поворота камеры при движении мышью при true

    // общие сведения команды
    public static int teamMembers; // всего членов команды в текущий момент
    public static int currentTeamMember; // номер текущего члена отряда
    public static float teamMemberChangeCD; // время перезарядки смены персонажа
    public static float teamMemberChangeCDDeltaTime; // время до следующей смены персонажа
    public static bool fightMode; // находится ли персонаж в состоянии боя
    
    // Взаимодействия со врагами
    public static List<Enemy> enemyInFight = new List<Enemy>(); // список врагов, с которыми сражаемся в данный момент
    public static Enemy enemyCurrentTarget; // текущая цель атаки
    public static float distanceToEnemyCurrentTarget; //дистанция до цели атаки


private void Awake()
{
    teamMembers = 1;
    currentTeamMember = 1;
    //character[0].maxHP = 20;
    //Debug.Log(character[0].maxHP);

}
}
