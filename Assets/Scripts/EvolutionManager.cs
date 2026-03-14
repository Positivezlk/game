using System;
using UnityEngine;

namespace MutantEvolutionIdle
{
    /// <summary>
    /// Handles prestige/evolution logic and permanent DNA bonuses.
    /// </summary>
    public class EvolutionManager : MonoBehaviour
    {
        [Header("Evolution")]
        [SerializeField] private float evolutionBiomassRequirement = 500f;
        [SerializeField] private int dnaPerEvolution = 1;

        [Header("Permanent Bonuses Per DNA")]
        [SerializeField, Range(0f, 1f)] private float productionBonusPerDna = 0.10f;
        [SerializeField, Range(0f, 2f)] private float idleSpeedBonusPerDna = 0.20f;
        [SerializeField, Range(0f, 0.5f)] private float costReductionPerDna = 0.02f;

        public int DnaPoints { get; private set; }

        public event Action<int> OnDnaChanged;

        public void Initialize(int loadedDnaPoints)
        {
            DnaPoints = Mathf.Max(0, loadedDnaPoints);
            OnDnaChanged?.Invoke(DnaPoints);
        }

        public bool CanEvolve(float currentBiomass)
        {
            return currentBiomass >= evolutionBiomassRequirement;
        }

        public bool TryEvolve(float currentBiomass)
        {
            if (!CanEvolve(currentBiomass))
            {
                return false;
            }

            DnaPoints += Mathf.Max(1, dnaPerEvolution);
            OnDnaChanged?.Invoke(DnaPoints);
            return true;
        }

        public float GetPermanentProductionMultiplier()
        {
            return 1f + (DnaPoints * productionBonusPerDna);
        }

        public float GetPermanentIdleSpeedMultiplier()
        {
            return 1f + (DnaPoints * idleSpeedBonusPerDna);
        }

        public float GetPermanentCostReduction()
        {
            return Mathf.Clamp(DnaPoints * costReductionPerDna, 0f, 0.9f);
        }

        public float GetEvolutionRequirement()
        {
            return evolutionBiomassRequirement;
        }
    }
}
