using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WatStudios.DeepestDungeon.Core;
using WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities;
using WatStudios.DeepestDungeon.Messaging;

public class OpenTooltip : MonoBehaviour
{
    public GameObject _tooltip;


    [Header("DD Abilities")]
    public Ability[] _ddAbilities;
    [Header("Tank Abilities")]
    public Ability[] _tankAbilities;
    [Header("Healer Abilities")]
    public Ability[] _healerAbilities;
       
    public Ability ability1;
    public Image abilityImage1;
    public Text nameText1;
    public Text descriptionText1;
    public Text cooldownText1;

    public Ability ability2;
    public Image abilityImage2;
    public Text nameText2;
    public Text descriptionText2;
    public Text cooldownText2;

    public Ability ability3;
    public Image abilityImage3;
    public Text nameText3;
    public Text descriptionText3;
    public Text cooldownText3;

    private void Start()
    {
        ActivateToolTips();
    }

    private void ActivateToolTips()
    {
        switch (GameManager.ChosenPlayerClass)
        {
            case PlayerClass.Dmg:
                ability1 = _ddAbilities[0];
                ability2 = _ddAbilities[1];
                ability3 = _ddAbilities[2];
                Init();
                break;
            case PlayerClass.Tank:
                ability1 = _tankAbilities[0];
                ability2 = _tankAbilities[1];
                ability3 = _tankAbilities[2];
                Init();
                break;
            case PlayerClass.Healer:
                ability2 = _healerAbilities[1];
                ability1 = _healerAbilities[0];
                ability3 = _healerAbilities[2];
                Init();
                break;
            default:
                Debug.LogError("This isn't a viable player class!");
                break;
        }
    }

    void Init()
    {
        abilityImage1.sprite = ability1.AbilityImage;
        nameText1.text = ability1.Name;
        descriptionText1.text = ability1.Description;
        cooldownText1.text = " " + ability1.Cooldown.BaseValue.ToString() + " s";

        abilityImage2.sprite = ability2.AbilityImage;
        nameText2.text = ability2.Name;
        descriptionText2.text = ability2.Description;
        cooldownText2.text = " " + ability2.Cooldown.BaseValue.ToString() + " s";

        abilityImage3.sprite = ability3.AbilityImage;
        nameText3.text = ability3.Name;
        descriptionText3.text = ability3.Description;
        cooldownText3.text = " " + ability3.Cooldown.BaseValue.ToString() + " s";
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            _tooltip.SetActive(true);
        }
        else
        {
            _tooltip.SetActive(false);
        }
    }
}
