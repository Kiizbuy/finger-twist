using GameFramework.Inventory.Items;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Inventory.BuyCell
{
    [DisallowMultipleComponent]
    public sealed class Vendor : MonoBehaviour
    {
        public Inventory Inventory;
        public Dictionary<string, CurrencyStash> Currencies = new Dictionary<string, CurrencyStash>();

        public static bool TryTransaction(Vendor buyer, Vendor seller, BaseItemData item, string currencyName, int amount)
        {
            if (!buyer.CanBuy(currencyName, item.Price, amount) || !seller.CanSell(item.Title, amount))
                return false;

            buyer.Buy(item, currencyName, amount);
            seller.Sell(item, currencyName, amount);

            return true;
        }

        public void AddCurrency(Currency currency)
        {
            if (!Currencies.ContainsKey(currency.Name))
                Currencies.Add(currency.Name, new CurrencyStash { Currency = currency, Amount = 0 });
        }

        public void AddCurrency(Currency currency, float amount)
        {
            if (!Currencies.ContainsKey(currency.Name))
                Currencies.Add(currency.Name, new CurrencyStash { Currency = currency, Amount = 0 });

            var stash = Currencies[currency.Name];

            stash.Amount += amount;
            Currencies[currency.Name] = stash;
        }

        public void AddCurrency(string name, float amount)
        {
            if (Currencies.ContainsKey(name))
            {
                var stash = Currencies[name];

                stash.Amount += amount;
                Currencies[name] = stash;
            }
        }

        public void RemoveCurrency(string name)
        {
            if (Currencies.ContainsKey(name))
                Currencies.Remove(name);
        }

        public void RemoveCurrency(string name, float amount)
        {
            if (Currencies.ContainsKey(name))
            {
                var stash = Currencies[name];

                stash.Amount = Mathf.Clamp(stash.Amount - amount, 0, stash.Amount + amount);
                Currencies[name] = stash;
            }
        }

        public bool CanBuy(string currencyName, int itemCost, int amount)
        {
            var cost = itemCost * amount;

            if (!Currencies.ContainsKey(currencyName))
                return false;
            if (Currencies[currencyName].Amount < cost && !Mathf.Approximately(Currencies[currencyName].Amount, cost))
                return false;

            return true;
        }

        public bool CanSell(string itemName, int amount)
        {
            var stock = Inventory.GetItemStateViaName(itemName);

            return stock.ItemsCount >= amount;
        }

        private bool Buy(BaseItemData item, string currencyName, int amount)
        {
            if (!CanBuy(currencyName, item.Price, amount))
                return false;

            var currency = Currencies[currencyName];

            currency.Amount -= item.Price * amount;
            Currencies[currencyName] = currency;
            Inventory.AddItem(item, amount);

            return true;
        }

        private bool Sell(BaseItemData item, string currencyName, int amount)
        {
            if (!CanSell(item.Title, amount))
                return false;

            if (Currencies.ContainsKey(currencyName))
            {
                var currency = Currencies[currencyName];

                currency.Amount += item.Price * amount;
                Currencies[currencyName] = currency;
            }

            Inventory.RemoveItem(item, amount);

            return true;
        }

        private float GetCurrency(string name)
        {
            if (Currencies.ContainsKey(name))
                return Currencies[name].Amount;
            return 0;
        }
    }
}

