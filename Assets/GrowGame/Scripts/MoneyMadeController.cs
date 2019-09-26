using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace GrowGame
{
    public class MoneyMadeController: MonoBehaviour
    {
        [SerializeField] private GlobalGameState gameState;
        [SerializeField] private TMP_Text moneyText;
        [ShowNonSerializedField]
        private float moneyMade = float.NegativeInfinity;

        private void Update()
        {
            var moneyNew = gameState.Money;
            if (Mathf.Abs(moneyNew - moneyMade) < 0.01)
            {
                return;
            }

            moneyMade = moneyNew;
            moneyText.text = $"£ {moneyMade:0.00}";
        }
    }
}