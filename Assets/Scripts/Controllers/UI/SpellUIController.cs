using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SpellUIController : MonoBehaviour
{

    [Header("UI Components")]
	[Space]
    [SerializeField] private GameObject[] ui_spellButtonArray;
	[SerializeField] private GameObject[] ui_spellSliderArray;
	[SerializeField] private GameObject ui_spellTooltip;
	[SerializeField] private GameObject ui_spellPreview;

    [Header("Params")]
	[Space]
    [SerializeField] private SpellManager spellManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Update cooldown visual
        var spellCountBase = 0;
        for(int spellCount = spellCountBase; spellCount < spellManager.spellArray.Length; spellCount++)
        {
            UI_UpdateSpellCooldownSlider(spellCount);
        }

        // Update spell preview position
        if(spellManager.isPreviewingSpell)
        {
            ui_spellPreview.transform.position = UI_GetMousePosition();
        }
    }

    // Add a spell in the spell UI
    public void UI_UpdatePlayerSpell(int spellSlotNumber, SpellScriptable spell)
    {
        ui_spellButtonArray[spellSlotNumber].GetComponentInChildren<TextMeshProUGUI>().text = spell.spellName;
        ui_spellSliderArray[spellSlotNumber].GetComponent<Slider>().maxValue = spell.spellCooldown;
        ui_spellSliderArray[spellSlotNumber].GetComponent<Slider>().value = spell.spellCooldown;
    }

    public void UI_PreviewSpellActivate(bool activating)
    {
        if(activating)
        {
            ui_spellPreview.GetComponent<SpriteRenderer>().sprite = spellManager.currentSpell.spellPreview;
        }
        ui_spellPreview.SetActive(activating);
    }

    public Vector3 UI_GetMousePosition()
    {
        var screenPos = Input.mousePosition;
        var worldPos = spellManager.mainCamera.GetComponent<Camera>().ScreenToWorldPoint(screenPos);
        return new Vector3(worldPos.x, worldPos.y, 0);
    }

    #region Manage spell cooldown
    public void UI_UpdateSpellCooldownButton(int spellSlotNumber, bool activating)
    {
        if(activating)
        {
            ui_spellButtonArray[spellSlotNumber].GetComponent<Image>().color = Color.red;
        }
        else
        {
            ui_spellButtonArray[spellSlotNumber].GetComponent<Image>().color = Color.white;
        }
    }

    private void UI_UpdateSpellCooldownSlider(int spellSlotNumber)
    {
        float time = spellManager.spellArray[spellSlotNumber].spellCooldown - (Time.time - spellManager.spellCooldownArray[spellSlotNumber]);

        int minutes = Mathf.FloorToInt(time/60);
        int seconds = Mathf.FloorToInt(time - minutes * 60f);

        string textTime = string.Format("{0:0}:{1:00}", minutes, seconds+1);
        if(time > 0)
        {
            ui_spellSliderArray[spellSlotNumber].GetComponentInChildren<TextMeshProUGUI>().text = textTime;
            ui_spellSliderArray[spellSlotNumber].GetComponent<Slider>().value = time;
            ui_spellSliderArray[spellSlotNumber].SetActive(true);
        }
        else
        {
            ui_spellSliderArray[spellSlotNumber].SetActive(false);
        }
    }
    #endregion

    #region Manage spell tooltip
    public void UI_ActivateSpellTooltip(int spellSlotNumber)
    {
        var tooltipText = "";
        ui_spellTooltip.SetActive(true);
        if(spellManager.spellArray[spellSlotNumber].spellDamage > 0)
        {
            tooltipText += "Damage(s) : " + spellManager.spellArray[spellSlotNumber].spellDamage + "\n";
        }
        if(spellManager.spellArray[spellSlotNumber].statusList.Count > 0)
        {
            tooltipText += "Effect -:";
            foreach(string status in spellManager.spellArray[spellSlotNumber].statusList)
            {
                tooltipText+= status + " ";
            }
            tooltipText += "\n";
        }
        if(spellManager.spellArray[spellSlotNumber].isSpellSummoning)
        {
            tooltipText += "Summoning : " + spellManager.spellArray[spellSlotNumber].spellSummonNumber + "\n";
        }
        if(spellManager.spellArray[spellSlotNumber].attackableFactionsList.Count > 0)
        {
            tooltipText += "Target(s) : ";
            foreach(string target in spellManager.spellArray[spellSlotNumber].attackableFactionsList)
            {
                tooltipText+= target + " ";
            }
        }
        tooltipText += "\n";
        ui_spellTooltip.SetActive(true);
        ui_spellTooltip.GetComponentInChildren<TextMeshProUGUI>().text = tooltipText;
    }

    public void UI_DeactivateSpellTooltip()
    {
        ui_spellTooltip.SetActive(false);
        ui_spellTooltip.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }
    #endregion
}
