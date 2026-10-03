using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobDetectionController : MonoBehaviour
{
    public MobController mobController;
    public CircleCollider2D mobDetectionCollider;
    public string attackablePriorityFaction;
    public List<string> attackableFactionsList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetMobDetectionColliderLayer(mobController.mobScriptable.attackableFactionsLayer);
        mobDetectionCollider.radius = mobController.mobScriptable.detectionRange;
        attackablePriorityFaction = mobController.mobScriptable.attackablePriorityFaction;
        attackableFactionsList = mobController.mobScriptable.attackableFactionsList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMobDetectionColliderLayer(LayerMask layerMask)
    {
        mobDetectionCollider.includeLayers = layerMask;
        mobDetectionCollider.excludeLayers = ~layerMask;
    }

    public void SetMobPriorityAttackFaction(string faction)
    {
        attackablePriorityFaction = faction;
    }

    public void SetMobAttackableFactionList(List<string> factionList)
    {
        attackableFactionsList = factionList;
    }

    // Mob detection trigger
    void OnTriggerEnter2D(Collider2D collider)
    {
        foreach(string layerName in attackableFactionsList)
        {
            // If new target is attackable
            if(collider.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                if(mobController.mobCurrentTarget == null)
                {
                    mobController.SetNewCurrentTarget(collider.gameObject);
                }
                else if(
                    // Check new target is priority layer
                    (collider.gameObject.layer == LayerMask.NameToLayer(attackablePriorityFaction)) &&
                    // Check if current target isn't already priority layer
                    (mobController.mobCurrentTarget.layer != LayerMask.NameToLayer(attackablePriorityFaction))
                ){
                    mobController.SetNewCurrentTarget(collider.gameObject);
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if(mobController.isWaitingforTarget)
        {
            foreach(string layerName in mobController.mobScriptable.attackableFactionsList)
            {
                // If new target is attackable
                if(collider.gameObject.layer == LayerMask.NameToLayer(layerName))
                {
                    mobController.SetNewCurrentTarget(collider.gameObject);
                }
            }
        }
    }
}
