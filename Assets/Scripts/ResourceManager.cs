using System;
using UnityEngine;

namespace MutantEvolutionIdle
{
    /// <summary>
    /// Handles biomass storage and idle generation.
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        [Header("Starting Values")]
        [SerializeField] private float startingBiomass = 0f;
        [SerializeField] private float baseBiomassPerSecond = 1f;

        public float CurrentBiomass { get; private set; }
        public float BaseBiomassPerSecond => baseBiomassPerSecond;

        public float AdditiveProductionBonus { get; private set; }
        public float MultiplicativeProductionBonus { get; private set; } = 1f;

        public event Action<float> OnBiomassChanged;
        public event Action<float> OnBiomassGenerated;

        public void Initialize(float loadedBiomass)
        {
            CurrentBiomass = Mathf.Max(startingBiomass, loadedBiomass);
            NotifyBiomassChanged();
        }

        public void TickProduction(float deltaTime, float dnaProductionMultiplier, float dnaIdleSpeedMultiplier)
        {
            float effectiveBps = GetEffectiveBiomassPerSecond(dnaProductionMultiplier);
            float generated = effectiveBps * Mathf.Max(0f, deltaTime) * Mathf.Max(0.01f, dnaIdleSpeedMultiplier);

            if (generated <= 0f)
            {
                return;
            }

            AddBiomass(generated);
            OnBiomassGenerated?.Invoke(generated);
        }

        public float GetEffectiveBiomassPerSecond(float dnaProductionMultiplier)
        {
            float flat = baseBiomassPerSecond + AdditiveProductionBonus;
            float multiplied = flat * Mathf.Max(1f, MultiplicativeProductionBonus);
            return multiplied * Mathf.Max(1f, dnaProductionMultiplier);
        }

        public bool TrySpendBiomass(float amount)
        {
            if (amount <= 0f || CurrentBiomass < amount)
            {
                return false;
            }

            CurrentBiomass -= amount;
            NotifyBiomassChanged();
            return true;
        }

        public void AddBiomass(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            CurrentBiomass += amount;
            NotifyBiomassChanged();
        }

        public void ResetRun()
        {
            CurrentBiomass = 0f;
            AdditiveProductionBonus = 0f;
            MultiplicativeProductionBonus = 1f;
            NotifyBiomassChanged();
        }

        public void AddFlatProductionBonus(float amount)
        {
            AdditiveProductionBonus += amount;
        }

        public void MultiplyProductionBonus(float factor)
        {
            MultiplicativeProductionBonus *= Mathf.Max(1f, factor);
        }

        private void NotifyBiomassChanged()
        {
            OnBiomassChanged?.Invoke(CurrentBiomass);
        }
    }
}
