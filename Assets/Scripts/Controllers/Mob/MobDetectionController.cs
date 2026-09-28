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

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("---Mob detected : " + collider.gameObject.name);
        mobController.SetNewCurrentTarget(collider.gameObject);
    }
}
