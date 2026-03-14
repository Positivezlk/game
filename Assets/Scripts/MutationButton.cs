using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MutantEvolutionIdle
{
    /// <summary>
    /// UI component for a single mutation purchase button.
    /// </summary>
    public class MutationButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text detailsText;

        private MutationData mutation;
        private MutationManager mutationManager;

        public void Setup(MutationData data, MutationManager manager)
        {
            mutation = data;
            mutationManager = manager;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(BuyMutation);

            Refresh();
        }

        public void Refresh()
        {
            if (mutation == null || mutationManager == null)
            {
                return;
            }

            bool purchased = mutationManager.IsPurchased(mutation);
            float cost = mutationManager.GetMutationCost(mutation);

            titleText.text = mutation.mutationName;
            detailsText.text = purchased
                ? "Purchased"
                : $"Cost: {cost:0}\n+{mutation.additiveBiomassPerSecond:0.##}/s, x{mutation.multiplicativeProduction:0.##}";

            button.interactable = !purchased;
        }

        private void BuyMutation()
        {
            mutationManager.PurchaseMutation(mutation);
        }
    }
}
