using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpellScriptable", menuName = "Scriptable Objects/SpellScriptable")]
public class SpellScriptable : ScriptableObject
{
    [Header("Info")]
	[Space]
    public string spellName;

    [Header("Stats")]
	[Space]
    public int spellDamage;	                                // Spell damage
    public float spellCooldown = 3f;				    	// Spell cooldown
    public float castingSpeed;                              // Spell loading time
    public bool isTargetingOnlyInside;                      // Are tagets forgotten once they step out of the hex
    public LayerMask targetable;							// View
    public List<string> attackableFactionsList;				// List of attackable factinos

    [Header("Status")]
	[Space]
    public List<string> statusList;                         // Status applied by the spell

    [Header("Summon")]
	[Space]
    public bool isSpellSummoning;                           // Is the spell summoning GO 
    public GameObject spellSummon;                          // GameObject summoned by the spell 
    public int spellSummonNumber;                           // Numlber of GO to summon 
    
    [Header("View Params")]
	[Space]
    public Sprite spellPreview;                             // Spell preview sprite
    public float rotationSpeed;							    // Speed rotation of the spell

    [Header("Params")]
	[Space]
    public GameObject spellPrefab;							// Spell GameObject
}
