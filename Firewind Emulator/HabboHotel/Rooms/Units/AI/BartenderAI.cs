using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units.AI
{
    class BartenderAI : AIBase
    {
        private List<int> _customers;

        private List<Point> GetCustomerArea()
        {
            List<Point> area = new List<Point>();
            for (int x = _unit.X - 2; x <= _unit.X + 2; x++)
                for (int y = _unit.Y - 2; y <= _unit.Y + 2; y++)
                        area.Add(new Point(x, y));

            area.Add(new Point(_unit.X - 3, _unit.Y));
            area.Add(new Point(_unit.X + 3, _unit.Y));
            area.Add(new Point(_unit.X , _unit.Y - 3));
            area.Add(new Point(_unit.X, _unit.Y + 3));

            return area;
        }

        internal BartenderAI(RoomAI unit) : base(unit)
        {
            _customers = new List<int>();
        }

        internal override void OnSelfEnterRoom()
        {

        }

        internal override void OnSelfLeaveRoom(bool Kicked)
        {

        }

        internal override void OnUserEnterRoom(RoomUser User)
        {

        }

        internal override void OnUserLeaveRoom(GameClients.GameClient Client)
        {

        }

        internal override void OnUserSay(RoomUser User, string Message)
        {

        }

        internal override void OnUserShout(RoomUser User, string Message)
        {

        }

        internal override void OnCycle()
        {
            RoomUnitManager unitManager = _unit.GetRoom().GetRoomUserManager();
            List<Point> area = GetCustomerArea();
            List<int> currentCustomers = new List<int>();

            foreach (Point p in area)
            {
                RoomUser customer = unitManager.GetUnitForSquare(p.X, p.Y) as RoomUser;
                if (customer == null)
                    continue;
                currentCustomers.Add(customer.VirtualID);

                if (!_customers.Contains(customer.VirtualID)) // New customer
                {
                    _customers.Add(customer.VirtualID);
                    _unit.Chat(string.Format("Hello there, {0}", customer.Name), true);
                }
            }

            List<int> toRemove = new List<int>();
            foreach (int customerID in _customers)
            {
                if (!currentCustomers.Contains(customerID)) // A customer left
                {
                    toRemove.Add(customerID);
                    _unit.Chat(string.Format("Bye, {0}", unitManager.GetRoomUnitByVirtualId(customerID).Name), true);
                }
            }
            
            // Remove old customers
            foreach (int customerID in toRemove)
                _customers.Remove(customerID);
        }
    }
}
