using System.Collections.Generic;
using System;
using UnityEngine;

namespace Defense.Systems
{
	// HACK
	public enum CurrencyType
	{
		Money,
		Gold,
		Gem,
	}

	public class CurrencySystem
	{
		private Action<CurrencyType, int> onCurrencyChangedEvent = null;
		public Action<CurrencyType, int> OnCurrencyChangedEvent { get => onCurrencyChangedEvent; set => onCurrencyChangedEvent = value; }

		private Dictionary<CurrencyType, int> wallet = new();

		public CurrencySystem()
		{
			// TODO - Load Wallet
			foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
			{
				wallet[type] = 0;
			}

			onCurrencyChangedEvent += ((type, value) =>
			{
				Debug.Log($"CURRENCY : {type} : {value}");
			});
		}

		public bool IsAbleToUseCurrency(CurrencyType type, int value)
		{
			return wallet[type] >= value;
		}

		public bool TryUseCurrency(CurrencyType type, int value)
		{
			if (!IsAbleToUseCurrency(type, value)) return false;
			wallet[type] -= value;
			onCurrencyChangedEvent?.Invoke(type, wallet[type]);

			return true;
		}

		public void AddCurrency(CurrencyType type, int value)
		{
			wallet[type] += value;
			onCurrencyChangedEvent?.Invoke(type, wallet[type]);
		}
	}

}
