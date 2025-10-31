using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class BeggarActions : MonoBehaviour
{
    public bool starving = true;

    // beggar settlements
    private GameObject[] beggarsSettlementOne;
    private GameObject[] beggarsSettlementTwo;
    private GameObject[] beggarsSettlementThree;


    [Header("Hunger Settings")]
    private float maxHunger = 100f;
    [SerializeField] private float currentHunger;
    private float hungerDrainRate = 0.5f; // per second

    [Header("UI References")]
    [SerializeField] private Image hungerFill;
    [SerializeField] private Transform barPivot;
    [SerializeField] private Camera mainCamera;

    // Event fired when an entire settlement becomes fully fed.
    // Parameter: settlement index (1,2,3)
    public static event Action<int> OnSettlementFullyFed;

    public float Hunger => currentHunger;

    private void Awake()
    {
        // Populate each settlement array with all GameObjects tagged "beggar" that are on the named layer.
        beggarsSettlementOne = GetBeggarsOnLayer("beggarSettlementOne");
        beggarsSettlementTwo = GetBeggarsOnLayer("beggarSettlementTwo");
        beggarsSettlementThree = GetBeggarsOnLayer("beggarSettlementThree");

        Debug.Log($"SettlementOne: {beggarsSettlementOne.Length}, SettlementTwo: {beggarsSettlementTwo.Length}, SettlementThree: {beggarsSettlementThree.Length}");
    }

    // returns array of GameObjects tagged "beggar" on the layer
    private GameObject[] GetBeggarsOnLayer(string layerName)
    {
        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex == -1)
        {
            Debug.LogWarning($"Layer '{layerName}' not found. Returning empty array.");
            return new GameObject[0];
        }

        return GameObject.FindGameObjectsWithTag("Beggar")
                         .Where(g => g != null && g.layer == layerIndex)
                         .ToArray();
    }


    public void Eat(GameObject food)
    {
        currentHunger += food.GetComponent<Food>().NutritionValue;
        starving = false;
        if (currentHunger < 0f)
        {
            currentHunger = 0f;
        }

        Debug.Log($"Beggar fed. Current hunger: {currentHunger}");

        // After this beggar is fed, check whether its settlement is now fully fed.
        int settlementIndex = GetMySettlementIndex();
        if (settlementIndex != 0)
        {
            if (IsSettlementFullyFed(settlementIndex))
            {
                Debug.Log($"Settlement {settlementIndex} is now fully fed.");
                OnSettlementFullyFed?.Invoke(settlementIndex);

                // Reward the player by adding money via PlayerStats.AddMoney(600)
                var playerStats = FindAnyObjectByType<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.AddMoney(600);
                    Debug.Log($"Player awarded 600 for feeding settlement {settlementIndex}.");
                }
                else
                {
                    Debug.LogWarning("PlayerStats not found in scene. Cannot add settlement reward.");
                }
            }
        }
    }

    private void Start()
    {
        currentHunger = maxHunger;
    }

    private void Update()
    {
        // Drain hunger
        if (starving)
        {
            currentHunger -= hungerDrainRate * Time.deltaTime;
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        }

        // Update UI
        if (hungerFill != null)
            hungerFill.fillAmount = currentHunger / maxHunger;
    }


    // Feed everyone across all settlements (example action).
    public void FedEveryone()
    {
        FedSettlementArray(beggarsSettlementOne);
        FedSettlementArray(beggarsSettlementTwo);
        FedSettlementArray(beggarsSettlementThree);
    }

    // Feed a specific settlement by index: 1, 2 or 3. Returns false if invalid index.
    public bool FedSettlement(int settlementIndex)
    {
        switch (settlementIndex)
        {
            case 1:
                FedSettlementArray(beggarsSettlementOne);
                return true;
            case 2:
                FedSettlementArray(beggarsSettlementTwo);
                return true;
            case 3:
                FedSettlementArray(beggarsSettlementThree);
                return true;
            default:
                Debug.LogWarning("Invalid settlementIndex. Use 1, 2 or 3.");
                return false;
        }
    }

    // Helper to  mark not starving on each beggar in the array.
    private bool FedSettlementArray(GameObject[] settlement)
    {
        if (settlement == null) return false;

        foreach (var b in settlement)
        {
            if (b == null) continue;
            var ba = b.GetComponent<BeggarActions>();
            if (ba.starving)
            {
                ba.starving = false;
                return true;
            }
        }
        return false;
    }

    // Optional: refresh cached lists at runtime
    public void RefreshAllSettlements()
    {
        beggarsSettlementOne = GetBeggarsOnLayer("beggarSettlementOne");
        beggarsSettlementTwo = GetBeggarsOnLayer("beggarSettlementTwo");
        beggarsSettlementThree = GetBeggarsOnLayer("beggarSettlementThree");
    }

    // Determine which settlement this GameObject belongs to.
    // Returns 1..3 for settlements, or 0 if none.
    private int GetMySettlementIndex()
    {
        int myLayer = gameObject.layer;
        if (myLayer == LayerMask.NameToLayer("beggarSettlementOne")) return 1;
        if (myLayer == LayerMask.NameToLayer("beggarSettlementTwo")) return 2;
        if (myLayer == LayerMask.NameToLayer("beggarSettlementThree")) return 3;
        return 0;
    }

    // Check whether all beggars in the given settlement are not starving.
    private bool IsSettlementFullyFed(int settlementIndex)
    {
        GameObject[] settlement = settlementIndex switch
        {
            1 => beggarsSettlementOne,
            2 => beggarsSettlementTwo,
            3 => beggarsSettlementThree,
            _ => null
        };

        if (settlement == null || settlement.Length == 0) return false;

        foreach (var g in settlement)
        {
            if (g == null) return false;
            var ba = g.GetComponent<BeggarActions>();
            if (ba == null) return false;
            if (ba.starving) return false;
        }

        return true;
    }
}
