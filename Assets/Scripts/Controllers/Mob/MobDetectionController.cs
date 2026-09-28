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
        // [TO UPDATE] Set target management
        if((collider.gameObject.layer == LayerMask.NameToLayer(mobController.mobScriptable.attackablePriorityFaction)) || mobController.mobCurrentTarget == null){
            mobController.SetNewCurrentTarget(collider.gameObject);
        }
    }
}
