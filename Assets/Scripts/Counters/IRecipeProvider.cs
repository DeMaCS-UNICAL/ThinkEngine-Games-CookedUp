using System;

public interface IRecipeProvider {
    BaseRecipeSO CurrentRecipe { get; }
    event EventHandler<ValueChangedEvent<BaseRecipeSO>> OnRecipeChanged;
}
