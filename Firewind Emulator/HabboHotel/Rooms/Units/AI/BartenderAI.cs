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
        // Pleaee note: the bot is currently configured for the following drinks: juice, milk, latter, water, regular coffee, decaff coffee and tea.
        //                                                                          2,5,??,7,8,8,1
        private static Dictionary<string, int> DrinkList = new Dictionary<string, int>()
        {
            {"tea", 1},
            {"juice", 2},
            {"milk", 5},
            {"latter", 5},
            {"water", 7},
            {"regular coffee", 8},
            {"decaff coffee", 8}
        };
        private List<int> _customers;
        private RoomUser _currentCustomer;

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

        internal override void OnUserChat(RoomUser user, string text, bool shout)
        {
            // Sure thing!
            // You got it!
            // For you, %name%, I'll do it.
            // So %name% wants a %item%...? I'll see what I can do!

            // You know, I used to be in barbershop quartet
            // There you go %name%
            // Here you are, %name%
            // Down the hatch, %name%
            // Don't drink it all in one go, savour the flavor!
            // Enjoy!
            if (_currentCustomer != null) // Already serving somebody
            {
                // send chat message?
                return;
            }
            int drinkID = -1;
            string drinkName = "nothing";
            foreach (var drink in DrinkList)
            {
                if (text.Contains(drink.Key)) // User wants this drink!
                {
                    drinkID = drink.Value;
                    drinkName = drink.Key;
                }
            }

            if (drinkID == -1) // Nothing found
                return;

            _unit.Chat("You got it!", true);
            _currentCustomer = user;
            _unit.MoveTo(user.SquareInFront);
        }

        int ad= 0;
        internal override void OnCycle()
        {
            ad++;
            if (ad == 10)
            {
                if (_currentCustomer == null)
                    _unit.MoveTo(_unit.GetRoom().GetGameMap().getRandomWalkableSquare());
                ad = 0;
            }
            if (_currentCustomer != null && _unit.Coordinate == _currentCustomer.SquareInFront)
            {
                _unit.Chat("Don't drink it all in one go, savour the flavor!", true);
                _currentCustomer = null;
            }

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
                    _unit.Chat(string.Format("Hello, {0}", customer.Name), true);
                }
            }

            var test = new HashSet<int>(_customers);
            test.SymmetricExceptWith(currentCustomers);
            List<int> toRemove = new List<int>();
            foreach (int customerID in test)
            {
                if (!currentCustomers.Contains(customerID)) // A customer left
                {
                    _customers.Remove(customerID);
                    //toRemove.Add(customerID);
                    _unit.Chat(string.Format("Bye, {0}", unitManager.GetRoomUnitByVirtualId(customerID).Name), true);
                }
            }
            
            // Remove old customers
            //foreach (int customerID in toRemove)
            //    _customers.Remove(customerID);
        }
    }
}
