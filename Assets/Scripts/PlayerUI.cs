using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private RectTransform HealthBar;

    private void OnEnable()
    {
        GetComponent<PlayerStats>().health.OnValueChanged += HealthChanged;
    }

    private void OnDisable()
    {
        GetComponent<PlayerStats>().health.OnValueChanged -= HealthChanged;
    }

    private void HealthChanged(int previousValue, int newValue)
    {
        //scale the health bar
        HealthBar.transform.localScale = new Vector3(newValue / 100f, 1, 1);
    }
}
