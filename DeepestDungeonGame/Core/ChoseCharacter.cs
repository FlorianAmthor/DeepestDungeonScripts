using UnityEngine;
using WatStudios.DeepestDungeon.Core;

public class ChoseCharacter : MonoBehaviour
{
    [SerializeField] private GameObject _models;

    private void OnEnable()
    {
        _models.SetActive(true);
    }

    private void OnDisable()
    {
        _models.SetActive(false);
    }

    // Update is called once per frame

    public void SetDMG()
    {
        GameManager.ChosenPlayerClass = PlayerClass.Dmg;
    }

    public void SetTANK()
    {
        GameManager.ChosenPlayerClass = PlayerClass.Tank;
    }

    public void SetHEALER()
    {
        GameManager.ChosenPlayerClass = PlayerClass.Healer;
    }
}
