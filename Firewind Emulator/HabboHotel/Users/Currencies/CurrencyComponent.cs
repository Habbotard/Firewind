using Database_Manager.Database.Session_Details.Interfaces;
using Firewind.HabboHotel.GameClients;
using Firewind.Messages;
using HabboEvents;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Users.Currencies
{
    class CurrencyComponent
    {
        private GameClient _parentClient;
        private Dictionary<byte, int> _currencies;

        internal CurrencyComponent(GameClient client)
        {
            this._parentClient = client;
        }

        internal void LoadCurrencies()
        {
            DataTable currencyResult;
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT type,amount FROM users_currencies WHERE users_id = @id LIMIT 7"); // only 7 different currencies ATM
                dbClient.addParameter("id", _parentClient.GetHabbo().Id);
                currencyResult = dbClient.getTable();
            }

            _currencies = new Dictionary<byte, int>();
            foreach (DataRow row in currencyResult.Rows)
            {
                _currencies.Add((byte)row[0], (int)row[1]);
            }
        }

        internal int GetAmountOfCurrency(byte currency)
        {
            if (!_currencies.ContainsKey(currency))
                return 0;
            return _currencies[currency];
        }

        internal void AddAmountOfCurrency(byte currency, int amount)
        {
            if (!_currencies.ContainsKey(currency))
                _currencies.Add(currency, 0);
            _currencies[currency] += amount;
        }

        internal void AddAmountOfCurrency(CurrencyType currency, int amount)
        {
            if (!_currencies.ContainsKey((byte)currency))
                _currencies.Add((byte)currency, 0);
            _currencies[(byte)currency] += amount;
        }

        internal void RemoveAmountOfCurrency(byte currency, int amount)
        {
            if (!_currencies.ContainsKey(currency))
                _currencies.Add(currency, 0);
            _currencies[currency] -= amount;
        }

        internal void RefreshActivityPointsBalance(byte currency)
        {
            RefreshActivityPointsBalance(new byte[] { currency });
        }
        internal void RefreshActivityPointsBalance(CurrencyType currency)
        {
            RefreshActivityPointsBalance(new byte[] { (byte)currency });
        }

        internal void RefreshActivityPointsBalance(byte[] updatedCurrencies)
        {
            if(_parentClient == null)
                return;

            ServerMessage message = new ServerMessage(Outgoing.ActivityPoints);

            message.AppendInt32(updatedCurrencies.Length); // count
            foreach (byte type in updatedCurrencies)
            {
                message.AppendInt32(type);
                message.AppendInt32(GetAmountOfCurrency(type));
            }
            _parentClient.SendMessage(message);

        }

        internal string GenerateCurrencyQuery()
        {
            StringBuilder query = new StringBuilder("REPLACE INTO users_currencies ");
            foreach (var entry in _currencies)
                query.Append(String.Format("VALUES({0},{1},{2}),", _parentClient.GetHabbo().Id, entry.Value, entry.Key));

            query.Remove(query.Length - 1, 1); // remove extra comma
            query.Append(';');
            return query.ToString();
        }

        internal void SendAllCurrencies()
        {
            ServerMessage message = new ServerMessage(Outgoing.ActivityPoints);

            message.AppendInt32(_currencies.Count);
            foreach (var entry in _currencies)
            {
                message.AppendInt32(entry.Key);
                message.AppendInt32(entry.Value);
            }
            _parentClient.SendMessage(message);
        }
    }
}
