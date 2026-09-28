using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpellManager : MonoBehaviour
{

    #region Variables
    [Header("Spells")]
	[Space]
	[SerializeField] public SpellScriptable[] spellArray;					    // Spell array for now
	[SerializeField] public int startingSpellIndex;					            // Spell for now
	[SerializeField] public float[] spellCooldownArray = new float[4];		    // Spell cooldowns array for now

    [Header("States")]
	[Space]
    public bool isStartingWithCurrentSpell = false;                            // Is player starting with default spell selected
    public bool isPreviewingSpell = false;                                      // Is player previewing the spell

    [Header("Spell params")]
	[Space]
    public SpellScriptable currentSpell;							            // Current spell scriptable
    public int currentSpellSlotNumber;							                // Current spell index in array
    public Vector3 spellCastPositon;							                // Current spell index in array

    [Header("Params")]
	[Space]
    public GameObject mainCamera;							                    // Main Camera
    private SpellUIController spellUIController;							    // Spell UI Controller
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        spellUIController = this.GetComponentInChildren<SpellUIController>();

        // Setting up Starting spells
        var spellCount = 0;
        foreach(SpellScriptable spell in spellArray)
        {
            spellUIController.UI_UpdatePlayerSpell(spellCount, spell);
            spellUIController.UI_UpdateSpellCooldownButton(spellCount, true);
            spellCount++;
        }

        // Set up starting selected spell
        if(isStartingWithCurrentSpell)
        {            
            currentSpellSlotNumber = startingSpellIndex;
            UpdatePlayerCurrentSpellByUI(startingSpellIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Cooldown Management
        for(int spellCount = 0; spellCount<spellArray.Length; spellCount++)
        {
            SpellCooldownAutoReset(spellCount);
        }
    }

    public void CastCurrentSpell()
    {
        spellCastPositon = Input.mousePosition;
        // Prevents spell casting at bottom screen (for UI)
        if (IsSpellCastable(spellCastPositon))
        {
            var worldPos = mainCamera.GetComponent<Camera>().ScreenToWorldPoint(spellCastPositon);
            Instantiate(currentSpell.spellPrefab,new Vector2(worldPos.x,worldPos.y),Quaternion.identity);

            // Activate cooldown
            spellCooldownArray[currentSpellSlotNumber] = Time.time;
            spellUIController.UI_UpdateSpellCooldownButton(currentSpellSlotNumber, true);
        }
    }

    #region Spell update
    // Update current spell selected by player
    public void UpdatePlayerCurrentSpell(SpellScriptable spell)
    {
        currentSpell = spell;
    }

    // Update current spell selected by player
    public void UpdatePlayerCurrentSpellByUI(int spellSlotNumber)
    {
        currentSpell = spellArray[spellSlotNumber];
        currentSpellSlotNumber = spellSlotNumber;

        //Call spell preview
        isPreviewingSpell = true;
        spellUIController.UI_PreviewSpellActivate(isPreviewingSpell);
    }
    #endregion

    #region Is spell castable
    public bool IsSpellCastable(Vector3 screenPos)
    {
        var isSpellCastable = false;
        
        // Is not clicking on UI region [TO UPDATE]
        if(isSpellInScreenLimits() && !isSpellInCooldown(currentSpellSlotNumber) && currentSpell != null) 
        {
            isSpellCastable = true;
        }
        return isSpellCastable;
    }

    // Is not clicking on UI region [TO UPDATE]
    public bool isSpellInScreenLimits()
    {
        var isSpellInScreenLimits = (spellCastPositon.y > 130);
        return isSpellInScreenLimits;
    }

    // Is not in cooldown
    public bool isSpellInCooldown(int spellSlotNumber)
    {
        var isSpellInCooldown = (Time.time < spellCooldownArray[spellSlotNumber] + spellArray[spellSlotNumber].spellCooldown);
        return isSpellInCooldown;
    }
    #endregion

    #region Cooldown management
    public float SpellCooldownAutoReset(int spellSlotNumber)
    {
        if(((Time.time - spellCooldownArray[spellSlotNumber]) >= spellArray[spellSlotNumber].spellCooldown))
        {
            spellUIController.UI_UpdateSpellCooldownButton(spellSlotNumber, false);
        }
        return (Time.time - spellCooldownArray[spellSlotNumber]);
    }
    #endregion
}
