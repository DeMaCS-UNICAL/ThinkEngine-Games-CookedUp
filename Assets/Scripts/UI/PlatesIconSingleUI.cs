using KitchenObjects.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlatesIconSingleUI : MonoBehaviour {
    
        [SerializeField] private Image Icon;


        public void Init(KitchenObjectSO kitchenObjectSO) {
            gameObject.name = kitchenObjectSO.name + "Icon";
            Icon.sprite = kitchenObjectSO.Sprite;
        }

    
    }
}
