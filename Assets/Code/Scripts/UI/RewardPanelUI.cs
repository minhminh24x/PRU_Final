using UnityEngine;
using System.Collections.Generic;

public class RewardPanelUI : MonoBehaviour
{
    public void ShowRewards(List<(ItemData item, int amount)> rewardPairs)
    {
        Debug.Log("[RewardPanelUI] ShowRewards called with " + rewardPairs.Count + " items.");
        gameObject.SetActive(true);
        // This is a minimal implementation to satisfy compilation.
        // In a real scenario, this would populate a UI grid with reward items.
    }
}
