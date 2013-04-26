using Converter.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    class Database
    {
        public static void Cleanup(SqlDatabaseClient Client)
        {
            Client.ExecuteNonQuery("TRUNCATE TABLE items");
            Client.ExecuteNonQuery("TRUNCATE TABLE items_rooms");
            Client.ExecuteNonQuery("TRUNCATE TABLE items_users");
            Client.ExecuteNonQuery("TRUNCATE TABLE items_extradata");
        }

        /*public static void DoRoomItems(SqlDatabaseClient Phoenix, SqlDatabaseClient Butterfly)
        {
            DataTable items = Phoenix.ExecuteQueryTable("SELECT * FROM items WHERE room_id > 0 AND wall_pos NOT LIKE ':w=%'");

            foreach (DataRow item in items.Rows)
            {
                uint Id = (uint)item["id"];
                uint RoomId = (uint)item["room_id"];
                int CombinedXY = Combine((double)item["x"], (double)item["y"]);
                double Z = (double)item["z"];
                int Rotation = (int)item["rot"];
                int BaseItem = (int)item["base_item"];
                string ExtraData = (string)item["extra_data"];

                Butterfly.ExecuteNonQuery("REPLACE INTO items_rooms(item_id, room_id, x, y, n) VALUES ('" + Id + "', '" + RoomId + "', '" + CombinedXY + "', '" + Z + "', '" + Rotation + "')");
                Butterfly.ExecuteNonQuery("REPLACE INTO items(item_id, base_id) VALUES ('" + Id + "', '" + BaseItem + "')");

                if (ExtraData != "")
                {
                    string SafeExtraData = AddSlashes(ExtraData);
                    Butterfly.ExecuteNonQuery("REPLACE INTO items_extradata (item_id, data) VALUES ('" + Id + "', '" + SafeExtraData + "')");
                }
                items.Rows.Remove(item);
            }
        }*/

        public static void DoUserItems(SqlDatabaseClient Phoenix, SqlDatabaseClient Butterfly)
        {
            DataTable items = Phoenix.ExecuteQueryTable("SELECT * FROM items WHERE room_id = 0 AND wall_pos NOT LIKE ':w=%'");

            foreach (DataRow item in items.Rows)
            {
                uint Id = (uint)item["id"];
                int UserId = (int)item["user_id"];

                int BaseItem = (int)item["base_item"];
                string ExtraData = (string)item["extra_data"];

                Butterfly.ExecuteNonQuery("REPLACE INTO items_users(item_id, user_id) VALUES ('" + Id + "', '" + UserId + "')");
                Butterfly.ExecuteNonQuery("REPLACE INTO items(item_id, base_id) VALUES ('" + Id + "', '" + BaseItem + "')");

                if (ExtraData != "")
                {
                    string SafeExtraData = AddSlashes(ExtraData);
                    Butterfly.ExecuteNonQuery("REPLACE INTO items_extradata (item_id, data) VALUES ('" + Id + "', '" + SafeExtraData + "')");
                }
                items.Rows.Remove(item);
            }
        }

        public static void DoWallItems(SqlDatabaseClient Phoenix, SqlDatabaseClient Butterfly)
        {
            DataTable items = Phoenix.ExecuteQueryTable("SELECT * FROM items WHERE room_id > 0 AND wall_pos LIKE ':w=%'");

            foreach (DataRow item in items.Rows)
            {
                int Id = Convert.ToInt32(item["id"]);
                int RoomId = Convert.ToInt32(item["room_id"]);
                string WallPos = (string)item["wall_pos"];

                int Position;

                string start = WallPos.Replace(":w=", "");
                string start2 = start.Replace("l=", "");

                string[] furni1 = start.Split(' ');
                string[] furni2 = start2.Split(' ');

                string x = furni1[0];
                string x1 = x.Replace(",", ".");

                string y = furni2[1];
                string y1 = y.Replace(",", ".");

                if (furni2[2] == "l")
                    Position = 8;
                else
                    Position = 7;

                string data = x1;
                string[] furnix = data.Split('.');

                string data1 = y1;
                string[] furniy = data1.Split('.');

                double x0 = Convert.ToDouble(furnix[0]);
                double x01 = Convert.ToDouble(furnix[1]);

                double y0 = Convert.ToDouble(furniy[0]);
                double y01 = Convert.ToDouble(furniy[1]);

                double g = Combine(x0, x01);
                double h = Combine(y0, y01);

                Butterfly.ExecuteNonQuery("REPLACE INTO items_rooms(item_id, room_id, x, y, n) VALUES ('" + Id + "', '" + RoomId + "', '" + g + "', '" + h + "', '" + Position + "')");
            
            }
        }

        public static void DoUserWallItems(SqlDatabaseClient Phoenix, SqlDatabaseClient Butterfly)
        {
            DataTable items = Phoenix.ExecuteQueryTable("SELECT * FROM items WHERE room_id = 0 AND wall_pos LIKE ':w=%'");

            foreach (DataRow item in items.Rows)
            {

                uint Id = (uint)item["id"];
                int UserId = (int)item["user_id"];
                int BaseItem = (int)item["base_item"];

                Butterfly.ExecuteNonQuery("REPLACE INTO items_users(item_id, user_id) VALUES ('" + Id + "', '" + UserId + "')");
                Butterfly.ExecuteNonQuery("REPLACE INTO items(item_id, base_id) VALUES ('" + Id + "', '" + BaseItem + "')");
                items.Rows.Remove(item);
            }
        }

        public static void DoAllNew(SqlDatabaseClient Db)
        {
            Db.ExecuteNonQuery("INSERT INTO " + Application.FirewindDb + ".items (item_id, base_id) SELECT DISTINCT id, base_item FROM " + Application.PhoenixDb + ".items;");
            Db.ExecuteNonQuery("INSERT INTO " + Application.FirewindDb + ".items_users (item_id, user_id) SELECT DISTINCT id, user_id FROM " + Application.PhoenixDb + ".items WHERE room_id = '0';");
            Db.ExecuteNonQuery("INSERT INTO " + Application.FirewindDb + ".items_extradata (item_id, data) SELECT DISTINCT id, extra_data FROM " + Application.PhoenixDb + ".items WHERE extra_data != '';");
            Db.ExecuteNonQuery("INSERT INTO " + Application.FirewindDb + ".items_rooms (item_id, room_id, x, y, n) SELECT DISTINCT id,room_id, x + ( y / 100 ),z,rot FROM " + Application.PhoenixDb + ".items WHERE room_id != '0' AND wall_pos NOT LIKE ':w=%';");
        }

        public static double Combine(double a, double b)
        {
            return a + (b / 100);
        }

        static string AddSlashes(string txt)
        {
            string text = txt.Replace("'", "\\'");
            return text.Replace("\"", "\\\"");
        }
    }
}
