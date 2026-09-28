using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private SpellManager spellManager;                     // SpellManager
    private SpellUIController spellUIController;                     // SpellManager

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spellManager = GameObject.FindWithTag("SpellManager").GetComponent<SpellManager>();

        var mainCamera = GameObject.FindWithTag("MainCamera");
        spellUIController = this.GetComponentInChildren<SpellUIController>();
    }

    // Update is called once per frame
    void Update()
    {
       // Player controls
        // Cast a spell
        if(Input.GetMouseButtonDown(0)){
            spellManager.CastCurrentSpell();
        }
        // Leave spell preview
        if((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) && spellManager.isPreviewingSpell){
            spellManager.isPreviewingSpell = false;
            spellUIController.UI_PreviewSpellActivate(spellManager.isPreviewingSpell);
            spellManager.currentSpell = null;
        }
        //Spell Shortcuts
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            spellManager.UpdatePlayerCurrentSpellByUI(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            spellManager.UpdatePlayerCurrentSpellByUI(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            spellManager.UpdatePlayerCurrentSpellByUI(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            spellManager.UpdatePlayerCurrentSpellByUI(3);
        }

    }
}
