using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{

    [Header("UI Components")]
	[Space]
	[SerializeField] private TextMeshProUGUI ui_playerLife;                     // Player Life
	[SerializeField] private GameObject ui_gameOver;                            // Game Over
    [SerializeField] private LineRenderer selectionLineRenderer;                // Line renderer for mob selection
    [SerializeField] private GameObject selectionColliderGameObject;            // Selection collider GameObject
    [SerializeField] private GameObject mousePositionGameObject;               // Line renderer for mob selection

    private GameObject playerGameObject;                                        //Player GameObject
    private PlayerController playerController;                                  //Player Controller
    private MobController playerMobController;                                  //Player Mob Controller

    // Mob selection params
    private Vector2 initialMousePosition;
    private Vector2 currentMousePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerGameObject = GameObject.FindWithTag("Player");
        playerController = playerGameObject.GetComponentInChildren<PlayerController>();
        playerMobController = playerGameObject.GetComponentInChildren<MobController>();

        // Mob selection setup
        selectionLineRenderer.positionCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        UI_UpdateLife();
    }

    public void UI_UpdateLife()
    {
        ui_playerLife.text = playerMobController.hitPoints.ToString();
    }

    public void UI_GameOver()
    {
        ui_gameOver.SetActive(true);
    }

    #region Mob selection display
    public void UI_StartPreviewMobSelection()
    {
        // Register starting point of selection
        selectionLineRenderer.positionCount = 4;
        initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectionLineRenderer.SetPosition(0, new Vector2(initialMousePosition.x, initialMousePosition.y));
        selectionLineRenderer.SetPosition(1, new Vector2(initialMousePosition.x, initialMousePosition.y));
        selectionLineRenderer.SetPosition(2, new Vector2(initialMousePosition.x, initialMousePosition.y));
        selectionLineRenderer.SetPosition(3, new Vector2(initialMousePosition.x, initialMousePosition.y));

        playerController.selectionBoxCollider = selectionColliderGameObject.AddComponent<BoxCollider2D>();
        playerController.selectionBoxCollider.isTrigger = true;
        playerController.selectionBoxCollider.offset = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    public void UI_UpdatePreviewMobSelection()
    {
        // Updating second point of selection
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectionLineRenderer.SetPosition(0, new Vector2(initialMousePosition.x, initialMousePosition.y));
        selectionLineRenderer.SetPosition(1, new Vector2(initialMousePosition.x, currentMousePosition.y));
        selectionLineRenderer.SetPosition(2, new Vector2(currentMousePosition.x, currentMousePosition.y));
        selectionLineRenderer.SetPosition(3, new Vector2(currentMousePosition.x, initialMousePosition.y));

        selectionColliderGameObject.transform.position = (currentMousePosition + initialMousePosition) / 2;

        playerController.selectionBoxCollider.size = new Vector2(
            Mathf.Abs(initialMousePosition.x - currentMousePosition.x),
            Mathf.Abs(initialMousePosition.y - currentMousePosition.y)
        );
    }

    public List<GameObject> UI_ReturnMobSelection()
    {
        var selectedMobs = selectionColliderGameObject.GetComponent<SelectionColliderController>().ReturnSelectedMobs();
        return selectedMobs;
    }

    public void UI_LeaveMobSelection()
    {
        selectionLineRenderer.positionCount = 0;
        Destroy(playerController.selectionBoxCollider);
        transform.position = Vector3.zero;
    }

    public Vector3 GetMousePositionGameObject()
    {
        mousePositionGameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    #endregion
}
