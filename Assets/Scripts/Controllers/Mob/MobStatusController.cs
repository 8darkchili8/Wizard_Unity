using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MobStatusController : MonoBehaviour
{
    [Header("Params")]
	[Space]
    [SerializeField] private MobController mobController;

    [Header("Status")]
	[Space]
    public bool isChilled;                                                  // Is Mob chilled
    public bool isIced;                                                     // Is Mob iced
    public bool isCharmed;    

    [Header("Status params")]
	[Space]
    public LayerMask charmedAttackableFactions;                             // Attackable factions when a mob is charmed
    public List<string> charmedAttackableFactionsList = new List<string>(); // Attackable factions list when a mob is charmed


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mobController = gameObject.GetComponent<MobController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Mob status management
    public bool GetMobStatusImmunity(string status)
    {
        var isImmune = false;
        foreach(string statusImmunity in mobController.mobScriptable.statusImmunityList)
        {
            if(statusImmunity == status)
            {
                isImmune =  true;
            } 
        }
        return isImmune;
    }

    public void ApplyStatusList(List<string> statusList, bool isApplied = true)
    {
        foreach(string status in statusList)
        {
            ApplyStatus(status, isApplied);
        }
    }

    public void ApplyStatus(string status, bool isApplied)
    {
        if(!GetMobStatusImmunity("Chill"))
        {
            switch(status)
            {
                case "Chill":
                    StartCoroutine(ApplyStatusChill(isApplied));
                    break;
                case "Ice":
                    StartCoroutine(ApplyStatusIce(isApplied));
                    break;
                case "Charm":
                    StartCoroutine(ApplyStatusCharm(isApplied));
                    break;
                default:
                    break;
            }   
        }
    }

    public IEnumerator ApplyStatusChill(bool isApplied)
    {
        // Check if Mob is immune
        if(!GetMobStatusImmunity("Chill")) 
        {
            if(isApplied)
            {
                if(!isChilled && !isIced)
                {
                    mobController.mobCurrentSpeed -= mobController.mobScriptable.chillSpeedMalus; 
                    isChilled = isApplied;
                    yield return new WaitForSeconds(mobController.mobScriptable.chillMalusTime);
                    StartCoroutine(ApplyStatusChill(false));
                }
                else if(!isIced)
                {
                    StartCoroutine(ApplyStatusIce(true));
                    StartCoroutine(ApplyStatusChill(false));
                }
            }
            else if(!isApplied && isChilled)
            {
                mobController.mobCurrentSpeed += mobController.mobScriptable.chillSpeedMalus;
                isChilled = isApplied;
            }
        }
    }

    public IEnumerator ApplyStatusIce(bool isApplied)
    {
        // Check if Mob is immune
        if(!GetMobStatusImmunity("Ice"))
        {
            if(isApplied)
            {
                if(!isIced)
                {
                    mobController.mobCurrentSpeed -= 999; 
                    isIced = isApplied;
                    yield return new WaitForSeconds(mobController.mobScriptable.iceMalusTime);
                    StartCoroutine(ApplyStatusIce(false));
                }
            }
            else if(!isApplied && isIced)
            {
                mobController.mobCurrentSpeed += 999;
                isIced = isApplied;
            }
        }
    }

    public IEnumerator ApplyStatusCharm(bool isApplied)
    {
        // Check if Mob is immune
        if(!GetMobStatusImmunity("Charm"))
        {
            var children = mobController.self.GetComponentsInChildren<Transform>(includeInactive: true);
            if(isApplied)
            {  
                if(!isCharmed)
                {
                    foreach (Transform child in children)
                    {
                        if(child.gameObject.name != "DetectionCollider")
                        {
                            child.gameObject.layer = LayerMask.NameToLayer("PlayerMob");
                        }
                    }
                    isCharmed = isApplied;
                    mobController.mobCurrentTarget = null;
                    mobController.isSelectableByPlayer = true;
                    LayerMask layerMask = LayerMask.NameToLayer("Enemy");
                    mobController.detectionCollider.GetComponent<MobDetectionController>().SetMobPriorityAttackFaction("Enemy");
                    mobController.detectionCollider.GetComponent<MobDetectionController>().SetMobDetectionColliderLayer(charmedAttackableFactions);
                    mobController.detectionCollider.GetComponent<MobDetectionController>().SetMobAttackableFactionList(charmedAttackableFactionsList);
                    yield return new WaitForSeconds(mobController.mobScriptable.charmMalusTime);
                    StartCoroutine(ApplyStatusCharm(false));
                }
            }
            else if(!isApplied && isCharmed)
            {
                foreach (Transform child in children)
                {
                    if(child.gameObject.name != "DetectionCollider")
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("Enemy");
                    }
                    isCharmed = isApplied;
                    mobController.isFollowingPlayerGoTo = false;
                    mobController.isSelectableByPlayer = false;
                    mobController.detectionCollider.GetComponent<MobDetectionController>().SetMobPriorityAttackFaction(mobController.mobScriptable.attackablePriorityFaction);
                    mobController.detectionCollider.GetComponent<MobDetectionController>().SetMobDetectionColliderLayer(mobController.mobScriptable.attackableFactionsLayer);
                    mobController.detectionCollider.GetComponent<MobDetectionController>().SetMobAttackableFactionList(mobController.mobScriptable.attackableFactionsList);
                }
            }
        }
    }
    #endregion
}
