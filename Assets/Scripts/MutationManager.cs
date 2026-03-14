using System;
using System.Collections.Generic;
using UnityEngine;

namespace MutantEvolutionIdle
{
    /// <summary>
    /// Handles mutation purchase state and applies mutation bonuses.
    /// </summary>
    public class MutationManager : MonoBehaviour
    {
        [SerializeField] private List<MutationData> availableMutations = new();

        private readonly HashSet<string> purchasedMutationIds = new();
        private float totalCostReduction;

        private ResourceManager resourceManager;
        private EvolutionManager evolutionManager;

        public IReadOnlyList<MutationData> AvailableMutations => availableMutations;
        public IReadOnlyCollection<string> PurchasedMutationIds => purchasedMutationIds;
        public float TotalCostReduction => totalCostReduction;

        public event Action<MutationData> OnMutationPurchased;
        public event Action OnMutationStateChanged;

        public void Initialize(ResourceManager resource, EvolutionManager evolution, IEnumerable<string> loadedPurchases)
        {
            resourceManager = resource;
            evolutionManager = evolution;
            purchasedMutationIds.Clear();
            totalCostReduction = 0f;

            if (loadedPurchases != null)
            {
                foreach (var id in loadedPurchases)
                {
                    var mutation = FindMutationById(id);
                    if (mutation == null)
                    {
                        continue;
                    }

                    purchasedMutationIds.Add(id);
                    ApplyMutationBonuses(mutation);
                }
            }

            OnMutationStateChanged?.Invoke();
        }

        public MutationData FindMutationById(string id)
        {
            return availableMutations.Find(m => m != null && m.id == id);
        }

        public bool IsPurchased(MutationData mutation)
        {
            return mutation != null && purchasedMutationIds.Contains(mutation.id);
        }

        public float GetMutationCost(MutationData mutation)
        {
            if (mutation == null)
            {
                return float.MaxValue;
            }

            float dnaReduction = evolutionManager != null ? evolutionManager.GetPermanentCostReduction() : 0f;
            float reduction = Mathf.Clamp01(totalCostReduction + dnaReduction);
            return Mathf.Max(1f, mutation.baseCost * (1f - reduction));
        }

        public bool PurchaseMutation(MutationData mutation)
        {
            if (mutation == null || IsPurchased(mutation))
            {
                return false;
            }

            float cost = GetMutationCost(mutation);
            if (!resourceManager.TrySpendBiomass(cost))
            {
                return false;
            }

            purchasedMutationIds.Add(mutation.id);
            ApplyMutationBonuses(mutation);

            OnMutationPurchased?.Invoke(mutation);
            OnMutationStateChanged?.Invoke();
            return true;
        }

        public void ResetRun()
        {
            purchasedMutationIds.Clear();
            totalCostReduction = 0f;
            OnMutationStateChanged?.Invoke();
        }

        private void ApplyMutationBonuses(MutationData mutation)
        {
            resourceManager.AddFlatProductionBonus(mutation.additiveBiomassPerSecond);
            resourceManager.MultiplyProductionBonus(mutation.multiplicativeProduction);
            totalCostReduction = Mathf.Clamp(totalCostReduction + mutation.costReduction, 0f, 0.95f);
        }
    }
}
