using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MutantEvolutionIdle
{
    /// <summary>
    /// Coordinates all UI updates and wiring.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Top Bar")]
        [SerializeField] private TMP_Text biomassText;
        [SerializeField] private TMP_Text biomassPerSecondText;
        [SerializeField] private TMP_Text dnaText;

        [Header("Center")]
        [SerializeField] private Image organismImage;
        [SerializeField] private Sprite defaultOrganismSprite;

        [Header("Bottom")]
        [SerializeField] private Transform mutationButtonContainer;
        [SerializeField] private MutationButton mutationButtonPrefab;
        [SerializeField] private Button evolutionButton;
        [SerializeField] private TMP_Text evolutionButtonText;

        [Header("Floating Text")]
        [SerializeField] private RectTransform floatingTextParent;
        [SerializeField] private TMP_Text floatingTextPrefab;

        private readonly List<MutationButton> mutationButtons = new();

        private ResourceManager resourceManager;
        private MutationManager mutationManager;
        private EvolutionManager evolutionManager;

        public void Initialize(ResourceManager resource, MutationManager mutation, EvolutionManager evolution)
        {
            resourceManager = resource;
            mutationManager = mutation;
            evolutionManager = evolution;

            resourceManager.OnBiomassChanged += _ => RefreshResourceUI();
            resourceManager.OnBiomassGenerated += SpawnFloatingText;
            mutationManager.OnMutationPurchased += OnMutationPurchased;
            mutationManager.OnMutationStateChanged += RefreshMutationButtons;
            evolutionManager.OnDnaChanged += _ => RefreshEvolutionUI();

            BuildMutationButtons();

            evolutionButton.onClick.RemoveAllListeners();
            evolutionButton.onClick.AddListener(OnEvolutionPressed);

            RefreshAll();
        }

        public void RefreshAll()
        {
            RefreshResourceUI();
            RefreshMutationButtons();
            RefreshEvolutionUI();
        }

        private void BuildMutationButtons()
        {
            foreach (Transform child in mutationButtonContainer)
            {
                Destroy(child.gameObject);
            }

            mutationButtons.Clear();
            foreach (var mutation in mutationManager.AvailableMutations)
            {
                if (mutation == null)
                {
                    continue;
                }

                MutationButton button = Instantiate(mutationButtonPrefab, mutationButtonContainer);
                button.Setup(mutation, mutationManager);
                mutationButtons.Add(button);
            }
        }

        private void RefreshResourceUI()
        {
            float production = resourceManager.GetEffectiveBiomassPerSecond(evolutionManager.GetPermanentProductionMultiplier());
            biomassText.text = $"Biomass: {resourceManager.CurrentBiomass:0}";
            biomassPerSecondText.text = $"Production: {production:0.##}/s";

            foreach (var button in mutationButtons)
            {
                button.Refresh();
            }

            RefreshEvolutionUI();
        }

        private void RefreshMutationButtons()
        {
            foreach (var button in mutationButtons)
            {
                button.Refresh();
            }
        }

        private void RefreshEvolutionUI()
        {
            dnaText.text = $"DNA: {evolutionManager.DnaPoints}";

            float requirement = evolutionManager.GetEvolutionRequirement();
            bool canEvolve = evolutionManager.CanEvolve(resourceManager.CurrentBiomass);
            evolutionButton.interactable = canEvolve;
            evolutionButtonText.text = $"Evolve ({requirement:0} Biomass)";
        }

        private void OnEvolutionPressed()
        {
            GameManager.Instance.TryEvolution();
        }

        private void OnMutationPurchased(MutationData mutation)
        {
            if (mutation.organismSpriteOverride != null && organismImage != null)
            {
                organismImage.sprite = mutation.organismSpriteOverride;
            }

            RefreshAll();
        }

        private void SpawnFloatingText(float amount)
        {
            if (floatingTextPrefab == null || floatingTextParent == null)
            {
                return;
            }

            TMP_Text floating = Instantiate(floatingTextPrefab, floatingTextParent);
            floating.text = $"+{amount:0}";
            StartCoroutine(AnimateFloatingText(floating.rectTransform, floating));
        }

        private System.Collections.IEnumerator AnimateFloatingText(RectTransform rect, TMP_Text text)
        {
            float duration = 0.75f;
            float elapsed = 0f;
            Vector2 start = new(Random.Range(-60f, 60f), Random.Range(-20f, 20f));
            Vector2 end = start + Vector2.up * 90f;
            Color startColor = text.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                rect.anchoredPosition = Vector2.Lerp(start, end, t);
                text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
                yield return null;
            }

            Destroy(text.gameObject);
        }

        public void ResetOrganismVisual()
        {
            if (organismImage != null)
            {
                organismImage.sprite = defaultOrganismSprite;
            }
        }
    }
}
