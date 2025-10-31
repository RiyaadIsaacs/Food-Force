using UnityEngine;
using UnityEngine.UI;
public class BeggarActions : MonoBehaviour
{
    public bool starving = true;

    [Header("Hunger Settings")]
    private float maxHunger = 100f;
    [SerializeField] private float currentHunger;
    private float hungerDrainRate = 0.5f; // per second

    [Header("UI References")]
    [SerializeField] private Image hungerFill;
    [SerializeField] private Transform barPivot;
    [SerializeField] private Camera mainCamera;


    public float Hunger => currentHunger;

    public void Eat(GameObject food)
    {
        currentHunger += food.GetComponent<Food>().NutritionValue;
        starving = false;
        if (currentHunger < 0f)
        {
            currentHunger = 0f;
        }
        Debug.Log($"Beggar fed. Current hunger: {currentHunger}");
    }

    private void Start()
    {
        currentHunger = maxHunger;
    }

    private void Update()
    {
        // Drain hunger
        currentHunger -= hungerDrainRate * Time.deltaTime;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

        // Update UI
        hungerFill.fillAmount = currentHunger / maxHunger;

        
    }

}
