using KitchenObjects.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressBarUI : MonoBehaviour {

        [SerializeField] private ProgressTracker progressTracker;

        [SerializeField, Tooltip("The object that has a IRecipeProvider script attached to it.\nIf null, recipe will not be tracked.")]
        private GameObject recipeProviderGameObject;
        private IRecipeProvider recipeProvider;

        [Header("UI")]
        [SerializeField] private Image progressFillBar;
        [SerializeField] private Image outputPreviewImage;
        [SerializeField] private GameObject progressBarUIObject;
        [SerializeField] private Image warningImage;

        public BaseRecipeSO CurrentRecipe => currentRecipe;
        private BaseRecipeSO currentRecipe;

        [SerializeField] private bool hideIfEmpty = true;
        [SerializeField] private bool hideIfFull = true;


        private bool isBurningRecipe = false;

        [SerializeField] private Color burningRecipeProgressColor = Color.red;
        private Color normalRecipeProgressColor;

        private void Start() {
            normalRecipeProgressColor = progressFillBar.color;
            if (progressTracker != null) {
                progressTracker.OnProgressChanged += (sender, e) => SetProgress(e.NewValue);
                SetProgress(progressTracker.Progress);
            }
            else {
                Debug.LogWarning("ProgressTracker is null, progress will not be tracked.");
            }


            if (recipeProviderGameObject != null) {
                if (recipeProviderGameObject.TryGetComponent<IRecipeProvider>(out recipeProvider)) {
                    recipeProvider.OnRecipeChanged += (sender, e) => SetRecipe(e.NewValue);
                    SetRecipe(recipeProvider.CurrentRecipe);
                }
            }
        }


        public void SetProgress(double progress) {
            progressFillBar.fillAmount = (float)progress;

            if ((hideIfEmpty && progress <= 0d) || (hideIfFull && progress >= 1d))
                Hide();
            else
                Show();
        }

        public void SetRecipe(BaseRecipeSO recipe) {
            currentRecipe = recipe;
            if (recipe != null)
                outputPreviewImage.sprite = recipe.Output.Sprite;

            isBurningRecipe = false;
            if (recipe is CookingRecipeSO cookingRecipe) {
                isBurningRecipe = cookingRecipe.IsBurningRecipe;
            }

            if (isBurningRecipe)
                progressFillBar.color = burningRecipeProgressColor;
            else
                progressFillBar.color = normalRecipeProgressColor;


            outputPreviewImage.gameObject.SetActive(recipe != null);
            warningImage.gameObject.SetActive(isBurningRecipe);
        }

        private void Show() {
            progressBarUIObject.SetActive(true);
        }

        private void Hide() {
            progressBarUIObject.SetActive(false);
        }
    }
}
