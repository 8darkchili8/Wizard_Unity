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
        if(spellManager.isPreviewingSpell)
        {
            UI_PreviewSpellTrack();
        }

        // Update cooldown visual
        var spellCountBase = 0;
        for(int spellCount = spellCountBase; spellCount < spellManager.spellArray.Length; spellCount++)
        {
            UI_UpdateSpellCooldownSlider(spellCount);
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

    public void UI_PreviewSpellTrack()
    {
        var screenPos = Input.mousePosition;
        var worldPos = spellManager.mainCamera.GetComponent<Camera>().ScreenToWorldPoint(screenPos);
        ui_spellPreview.transform.position = new Vector3(worldPos.x, worldPos.y, 0);
    }

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

        string textTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        if(time > 0)
        {
            ui_spellSliderArray[spellSlotNumber].GetComponentInChildren<TextMeshProUGUI>().text = textTime;
            ui_spellSliderArray[spellSlotNumber].GetComponent<Slider>().value = time;
        }
    }
}
