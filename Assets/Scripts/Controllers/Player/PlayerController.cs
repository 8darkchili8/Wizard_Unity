using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private SpellManager spellManager;                               // Spell manager
    private SpellUIController spellUIController;                     // Spell UI controller
    private UIController uiController;                               // UI controller 
    
    // Mob selection params
    public bool isSelecting;
    public BoxCollider2D selectionBoxCollider;
    public List<GameObject> selectedMobs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spellManager = GameObject.FindWithTag("SpellManager").GetComponent<SpellManager>();
        spellUIController = spellManager.transform.Find("P_UI_Spell").GetComponent<SpellUIController>();
        uiController = GameObject.FindWithTag("UI").GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Player controls
        
        #region Mob selection controls
        if(!spellManager.isPreviewingSpell)
        {
            if((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) && isSelecting)
            {
                // Cancel selection
                selectedMobs =  new List<GameObject>();
                LeaveMobSelection();
            }
            if(Input.GetMouseButtonDown(0))
            {
                // Start mob selection
                EnterMobSelection();
            }
            if(Input.GetMouseButton(0) && isSelecting)
            {
                // Update mob selection
                MobSelection();
            }
            if(Input.GetMouseButtonUp(0) && isSelecting)
            {
                ExectuteMobSelection();
            }
            if(Input.GetMouseButtonDown(1) && !isSelecting && selectedMobs.Count != 0)
            {
                // Oder select mobs to move to point
                OrderMobNewTarget();
            }
        
        }

        #endregion

        #region Spell casting controls
        if(spellManager.isPreviewingSpell)
        {
            if(Input.GetMouseButtonDown(0))
            {
                spellManager.CastCurrentSpell();
            }
            // Leave spell preview
            if((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
            {
                spellManager.isPreviewingSpell = false;
                spellUIController.UI_PreviewSpellActivate(spellManager.isPreviewingSpell);
                spellManager.currentSpell = null;
            }
        }
        #endregion

        #region Shortcuts controls

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            spellManager.UpdatePlayerCurrentSpellByUI(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            spellManager.UpdatePlayerCurrentSpellByUI(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            spellManager.UpdatePlayerCurrentSpellByUI(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            spellManager.UpdatePlayerCurrentSpellByUI(3);
        }
        #endregion
    }

    #region Mob selection
    private void EnterMobSelection()
    {
        isSelecting = true;
        uiController.UI_StartPreviewMobSelection();
    }

    private void MobSelection()
    {
        uiController.UI_UpdatePreviewMobSelection();
    }

    private void ExectuteMobSelection()
    {
        selectedMobs = uiController.UI_ReturnMobSelection();
        foreach(GameObject selectedMob in selectedMobs)
        {
            selectedMob.GetComponent<MobController>().ActivateSelectedByPlayer(true);
        }
        isSelecting = false;
        LeaveMobSelection();
        // [TO DO] if result given, else, leave selection

    }

    private void LeaveMobSelection()
    {
        isSelecting = false;
        foreach(GameObject selectedMob in selectedMobs)
        {
            selectedMob.GetComponent<MobController>().ActivateSelectedByPlayer(false);
        }
        uiController.UI_LeaveMobSelection();
    }

    private void OrderMobNewTarget()
    {   
        foreach(GameObject selectedMob in selectedMobs)
        {
            selectedMob.GetComponent<MobController>().SetNewCurrentTargetTransform(uiController.GetMousePositionGameObject());
        }
    }
    #endregion
}
