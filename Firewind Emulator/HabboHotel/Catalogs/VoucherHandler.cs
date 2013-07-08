using System;
using System.Data;
using Firewind.HabboHotel.GameClients;
using Firewind.Messages;
using Database_Manager.Database.Session_Details.Interfaces;
using Firewind.Messages.Headers;

namespace Firewind.HabboHotel.Catalogs
{
    class VoucherHandler
    {
        private static Boolean IsValidCode(string Code)
        {
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT null FROM credit_vouchers WHERE code = @code");
                dbClient.addParameter("code", Code);

                if (dbClient.getRow() != null)
                {
                    return true;
                }
            }

            return false;
        }

        internal static int GetVoucherValue(string Code)
        {
            DataRow Data;

            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT value FROM credit_vouchers WHERE code = @code");
                dbClient.addParameter("code", Code);

                Data = dbClient.getRow();
            }

            if (Data != null)
            {
                return (int)Data[0];
            }

            return 0;
        }

        private static void TryDeleteVoucher(string Code)
        {
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("DELETE FROM credit_vouchers WHERE code = @code");
                dbClient.addParameter("code", Code);
                dbClient.runQuery();
            }
        }

        internal static void TryRedeemVoucher(GameClient Session, string Code)
        {
            if (!IsValidCode(Code))
            {
                ServerMessage Error = new ServerMessage(Outgoing.VoucherRedeemError);
                Error.AppendRawInt32(0); // 0=invalid code,1=technical issue,3=redeem at webpage
                Session.SendMessage(Error);
                return;
            }

            int Value = GetVoucherValue(Code);

            TryDeleteVoucher(Code);

            Session.GetHabbo().Credits += Value;
            Session.GetHabbo().UpdateCreditsBalance();

            ServerMessage message = new ServerMessage(Outgoing.VoucherRedeemOk);
            message.AppendString("Credits"); // productName
            message.AppendString("Awesome"); // productDescription
            Session.SendMessage(message);
        }
    }
}
