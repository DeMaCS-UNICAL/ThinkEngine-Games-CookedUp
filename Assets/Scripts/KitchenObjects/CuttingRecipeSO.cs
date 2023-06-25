using UnityEngine;

[CreateAssetMenu(fileName = "new CuttingRecipeSO", menuName = "CookedUp/CuttingRecipeSO", order = 1)]
public class CuttingRecipeSO : BaseRecipeSO {
    [SerializeField, Range(0.01f, 10f)] private float timeToCut = 1f;

    /// <summary>
    /// Time in seconds to cut the input into the output
    /// </summary>
    public float TimeToCut => timeToCut;

}