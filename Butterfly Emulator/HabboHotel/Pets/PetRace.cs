using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Database_Manager.Database.Session_Details.Interfaces;

namespace Butterfly.HabboHotel.Pets
{
    public class PetRace
    {
        public int RaceId;
        public int Color1;
        public int Color2;
        public bool Has1Color;
        public bool Has2Color;

        public static List<PetRace> Races;

        public static void Init(IQueryAdapter dbClient)
        {
            dbClient.setQuery("SELECT * FROM pets_racesoncatalogue");
            DataTable Table = dbClient.getTable();

            Races = new List<PetRace>();
            foreach (DataRow Race in Table.Rows)
            {
                PetRace R = new PetRace();
                R.RaceId = (int)Race["raceid"];
                R.Color1 = (int)Race["color1"];
                R.Color2 = (int)Race["color2"];
                R.Has1Color = ((string)Race["has1color"] == "1");
                R.Has2Color = ((string)Race["has2color"] == "1");
                Races.Add(R);
            }
        }

        public static List<PetRace> GetRacesForRaceId(int sRaceId)
        {
            List<PetRace> sRaces = new List<PetRace>();
            foreach (PetRace R in Races)
            {
                if (R.RaceId == sRaceId)
                    sRaces.Add(R);
            }

            return sRaces;
        }

        public static bool RaceGotRaces(int sRaceId)
        {
            if (GetRacesForRaceId(sRaceId).Count > 0)
                return true;
            else
                return false;
        }
    }
}
