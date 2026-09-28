using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MobScriptable", menuName = "Scriptable Objects/MobScriptable")]
public class MobScriptable : ScriptableObject
{
    [Header("Stats")]
	[Space]
	public int baseHitPoints = 3;							// Base hit points
	public int damage = 5;							        // Damage delt by the enemy
	public float speed = 3f;								// Move speed
	public float detectionRange = 3f;						// Detection range

    [Header("Faction")]
    [Space]
    public string faction;                                  // Faction of the mob
    public LayerMask attackableFactions;                    // Faction the mob can target
    public List<string> attackableFactionsList;                    // Faction the mob can target
    public string attackablePriorityFaction;                // Faction the mob targets in priority

    [Header("Behaviors")]
    [Space]
    public bool isPlayerMainTarget;				        // Can it moving
    public bool canMoveTowardsTarget;				    // Can it moving
    public bool canMeleeAttack;     				    // Can attack at melee
    public bool canRangeAttack;     				    // Can attack at range
    public bool canSummon;                              // Can it summon

    [Header("Summons")]
	[Space]
    public GameObject summon;
    public float summoningRate;

    [Header("Range attack")]
	[Space]
    public GameObject projectile;
    public float rangeAttackRate;
}
