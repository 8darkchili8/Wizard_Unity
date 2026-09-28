using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITextDamageController : MonoBehaviour
{
    [Header("Params")]
	[Space]
    public int textInitialVelocityY;
    public Rigidbody2D textRigidBody;
    public float textLifetime = 1.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textRigidBody.linearVelocity = new Vector2(Random.Range(-2,2), textInitialVelocityY);
        Destroy(gameObject, textLifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTextDamage(int damage)
    {
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "-" + damage.ToString();
    }
}
