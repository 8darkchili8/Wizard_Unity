using UnityEngine;

public class MobDetectionController : MonoBehaviour
{
    public MobController mobController;
    public CircleCollider2D mobDetectionCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mobDetectionCollider.includeLayers = mobController.mobScriptable.attackableFactions;
        mobDetectionCollider.excludeLayers = ~mobController.mobScriptable.attackableFactions;
        mobDetectionCollider.radius = mobController.mobScriptable.detectionRange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Mob detection trigger
    void OnTriggerEnter2D(Collider2D collider)
    {
        foreach(string layerName in mobController.mobScriptable.attackableFactionsList)
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
                    (collider.gameObject.layer == LayerMask.NameToLayer(mobController.mobScriptable.attackablePriorityFaction)) &&
                    // Check if current target isn't already priority layer
                    (mobController.mobCurrentTarget.layer != LayerMask.NameToLayer(mobController.mobScriptable.attackablePriorityFaction))
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
