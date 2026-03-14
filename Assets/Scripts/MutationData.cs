using UnityEngine;

namespace MutantEvolutionIdle
{
    /// <summary>
    /// Defines a mutation upgrade the player can buy.
    /// Stored as a ScriptableObject for easy balancing and content expansion.
    /// </summary>
    [CreateAssetMenu(fileName = "MutationData", menuName = "Mutant Evolution Idle/Mutation Data")]
    public class MutationData : ScriptableObject
    {
        [Header("Identity")]
        public string id = "mutation_id";
        public string mutationName = "New Mutation";
        [TextArea] public string description;

        [Header("Cost")]
        [Min(1f)] public float baseCost = 10f;

        [Header("Production Effects")]
        [Tooltip("Adds flat biomass per second.")]
        public float additiveBiomassPerSecond = 0f;

        [Tooltip("Multiplies production. 2 = x2 production.")]
        public float multiplicativeProduction = 1f;

        [Tooltip("Reduces future mutation costs. 0.1 = 10% cheaper.")]
        [Range(0f, 0.95f)]
        public float costReduction = 0f;

        [Header("Visual")]
        public Sprite organismSpriteOverride;
    }
}
