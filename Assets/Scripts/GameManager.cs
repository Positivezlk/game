using System.Collections;
using System.Linq;
using UnityEngine;

namespace MutantEvolutionIdle
{
    /// <summary>
    /// Bootstrapper for all game systems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Managers")]
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private MutationManager mutationManager;
        [SerializeField] private EvolutionManager evolutionManager;
        [SerializeField] private SaveSystem saveSystem;
        [SerializeField] private UIManager uiManager;

        [Header("Loop")]
        [SerializeField] private float autoSaveIntervalSeconds = 10f;

        private float productionTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            LoadGame();
            uiManager.Initialize(resourceManager, mutationManager, evolutionManager);
            StartCoroutine(AutoSaveLoop());
        }

        private void Update()
        {
            productionTimer += Time.deltaTime;
            if (productionTimer >= 1f)
            {
                float ticks = Mathf.Floor(productionTimer);
                productionTimer -= ticks;

                float productionMultiplier = evolutionManager.GetPermanentProductionMultiplier();
                float idleSpeedMultiplier = evolutionManager.GetPermanentIdleSpeedMultiplier();
                resourceManager.TickProduction(ticks, productionMultiplier, idleSpeedMultiplier);
            }
        }

        public void TryEvolution()
        {
            if (!evolutionManager.TryEvolve(resourceManager.CurrentBiomass))
            {
                return;
            }

            resourceManager.ResetRun();
            mutationManager.ResetRun();
            uiManager.ResetOrganismVisual();
            uiManager.RefreshAll();
            SaveGame();
        }

        public void SaveGame()
        {
            var data = new SaveSystem.SaveData
            {
                biomass = resourceManager.CurrentBiomass,
                dnaPoints = evolutionManager.DnaPoints,
                purchasedMutationIds = mutationManager.PurchasedMutationIds.ToList()
            };

            saveSystem.Save(data);
        }

        public void LoadGame()
        {
            SaveSystem.SaveData data = saveSystem.Load();
            evolutionManager.Initialize(data.dnaPoints);
            resourceManager.Initialize(data.biomass);
            mutationManager.Initialize(resourceManager, evolutionManager, data.purchasedMutationIds);
        }

        private IEnumerator AutoSaveLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoSaveIntervalSeconds);
                SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}
