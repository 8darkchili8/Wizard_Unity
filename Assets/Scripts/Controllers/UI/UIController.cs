using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{

    [Header("UI Components")]
	[Space]
	[SerializeField] private TextMeshProUGUI ui_playerLife;                     // Player Life
	[SerializeField] private GameObject ui_gameOver;                        // Game Over

    private GameObject playerGameObject;                                        //Player GameObject
    private MobController playerController;                                  //Player Controller

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerGameObject = GameObject.FindWithTag("Player");
        playerController = playerGameObject.GetComponentInChildren<MobController>();
    }

    // Update is called once per frame
    void Update()
    {
        UI_UpdateLife();
    }

    public void UI_UpdateLife()
    {
        ui_playerLife.text = playerController.hitPoints.ToString();
    }

    public void UI_GameOver()
    {
        ui_gameOver.SetActive(true);
    }
}
