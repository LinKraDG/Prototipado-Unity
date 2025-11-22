using UnityEngine;
using UnityEngine.UI;

public class LifeCanvas : MonoBehaviour
{
    [SerializeField]
    Image lifeBar;
    [SerializeField]
    EntityLife entityLife;

    private void OnEnable()
    {
        entityLife.onLifeChange.AddListener(OnLifeChange);
    }

    private void OnDisable()
    {
        entityLife.onLifeChange.RemoveListener(OnLifeChange);
    }

    void OnLifeChange(float newLifeValue)
    {
        lifeBar.fillAmount = newLifeValue / entityLife.GetMaxLife();
    }
}
