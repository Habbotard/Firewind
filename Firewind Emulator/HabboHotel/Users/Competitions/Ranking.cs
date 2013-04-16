using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Database_Manager.Database.Session_Details.Interfaces;

namespace Firewind.HabboHotel.Users.Competitions
{
    public class Ranking
    {
        public uint UserId;
        public RankingType Type;
        public string Information;
        public int RoomId;
        public int Score;
        public static Dictionary<uint, Ranking> getRanking;
        public static void initRankings(IQueryAdapter dbClient)
        {
            dbClient.setQuery("SELECT * FROM user_rankings ORDER BY score ASC");
            DataTable table = dbClient.getTable();
            getRanking = new Dictionary<uint, Ranking>();
            foreach (DataRow row in table.Rows)
            {
                Ranking r = new Ranking();
                r.UserId = Convert.ToUInt32(row["userid"]);
                string type = (string)row["type"];
                if (type == "competitions")
                    r.Type = RankingType.COMPETITIONS;
                else if (type == "snowwar")
                    r.Type = RankingType.SNOWWAR;
                else if (type == "fastfood")
                    r.Type = RankingType.FASTFOOD;
                else if (type == "slotcar")
                    r.Type = RankingType.SLOTCAR;
                else
                    r.Type = RankingType.NONE;
                r.Information = (string)row["information"];
                r.RoomId = (int)row["roomid"];
                r.Score = (int)row["score"];
                getRanking.Add(r.UserId, r);
            }
        }

        public static List<Ranking> getRankingForType(RankingType t)
        {
            List<Ranking> result = new List<Ranking>();
            foreach (Ranking r in getRanking.Values)
            {
                if (r.Type == t)
                    result.Add(r);
            }
            return result;
        }

        public static List<Ranking> getCompetitionForInfo(String Information)
        {
            List<Ranking> result = new List<Ranking>();
            foreach (Ranking r in getRanking.Values)
            {
                if (r.Type == RankingType.COMPETITIONS && r.Information == Information)
                    result.Add(r);
            }
            return result;
        }

        public static void AddScoreToUserId(int tScore, uint UserId, RankingType t)
        {
            if (!getRanking.ContainsKey(UserId))
                AddUserToRanking(UserId, t);
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("UPDATE user_rankings SET score = " + (getRanking[UserId].Score + tScore) + " WHERE userid = " + UserId + " AND type = '" + parseEnum(t) + "';");
            }
        }

        public static void AddUserToRanking(uint UserId, RankingType t, string Information = "hlatCompetitions", int RoomId = 0)
        {
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("INSERT INTO user_rankings (type, information, userid, roomid, score) VALUES ('" + parseEnum(t) + "', '" + Information + "', " + UserId + ", " + RoomId + ", 0);");
            }
            Ranking r = new Ranking();
            r.UserId = UserId;
            r.Type = t;
            r.Information = Information;
            r.RoomId = RoomId;
            r.Score = 0;
            getRanking.Add(UserId, r);
        }

        private static string parseEnum(RankingType t)
        {
            if (t == RankingType.COMPETITIONS)
                return "competitions";
            else if (t == RankingType.SNOWWAR)
                return "snowwar";
            else if (t == RankingType.FASTFOOD)
                return "fastfood";
            else if (t == RankingType.SLOTCAR)
                return "slotcar";
            else if (t == RankingType.NONE)
                return "";
            return "";
        }
    }

    public enum RankingType
    {
        COMPETITIONS,
        SNOWWAR,
        FASTFOOD,
        SLOTCAR,
        NONE
    }
}
