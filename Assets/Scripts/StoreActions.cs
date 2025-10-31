using UnityEngine;

public class StoreActions : MonoBehaviour
{
    [SerializeField] private PlayerStats player;
    [SerializeField] private StoreType storeType;

    public void BuyFood()
    {
        // Determine food cost based on store type
        int foodCost = storeType == StoreType.HealthyFood ? 150 : 100;

        if (player != null && player.Money >= foodCost)
        {
            player.DeductMoney(foodCost);
            Debug.Log($"Purchased food for {foodCost} from {storeType} store.");
        }
        else
        {
            Debug.Log("Not enough money to buy food.");
        }
    }
}
