using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotCharacterWidget : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] CanvasGroup _canvasGroup;

    [Header("Main Settings")]
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _emptySlot;
    [SerializeField] private GameObject _infoCharacterSlot;
    [SerializeField] private TMP_Text _nameLable;
    [SerializeField] private TMP_Text _levelLable;
    [SerializeField] private TMP_Text _goldLable;

    public Button SlotButton => _button;

    public void ShowInfoCracterSlot(string name, string level, string gold)
    {
        _nameLable.text = name;
        _levelLable.text = level;
        _goldLable.text = gold;

        _infoCharacterSlot.SetActive(true);
        _emptySlot.SetActive(false);
    }

    public void ShowEmptySlot()
    {
        _infoCharacterSlot.SetActive(false);
        _emptySlot.SetActive(true);

    }
}
