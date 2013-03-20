using System;
using System.Data;
using ConsoleWriter;
using Database_Manager.Session_Details.Interfaces;
using MySql.Data.MySqlClient;

namespace Database_Manager.Database.Session_Details
{
    class QueryAdapter : IRegularQueryAdapter
    {
        #region delcares
        /// <summary>
        /// The client which is using this information
        /// </summary>
        protected DatabaseClient client;

        /// <summary>
        /// The mysql command list
        /// </summary>
        protected MySqlCommand command;
        #endregion

        #region Stats
        //Data used by the status tool for measuring lagg, etc.

        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Database session
        /// </summary>
        /// <param name="client">The client which holds the connection</param>
        internal QueryAdapter(DatabaseClient client)
        {
            this.client = client;
        }
        #endregion

        #region Setters

        /// <see cref="IRegularQueryAdapter"/>
        public void setQuery(string query)
        {
            command.Parameters.Clear();
            command.CommandText = query;
        }

        /// <see cref="IRegularQueryAdapter"/> 
        public void addParameter(string parameterName, object val)
        {
            command.Parameters.AddWithValue(parameterName, val);
        }

        /// <see cref="IRegularQueryAdapter"/>
        public void addParameter(string name, byte[] data) {
            command.Parameters.Add(new MySqlParameter(name, MySqlDbType.Blob, data.Length));
        }

        #endregion

        #region Query kinds

        /// <see cref="IRegularQueryAdapter"/>
        public DataTable getTable()
        {
            DateTime start = DateTime.Now;

            DataTable pDataTable = new DataTable();
            try
            {
                using (MySqlDataAdapter pAdapter = new MySqlDataAdapter(command))
                {
                    pAdapter.Fill(pDataTable);
                }
            }
            catch (Exception ex)
            {
                Writer.LogQueryError(ex, command.CommandText);
            }

            TimeSpan spent = DateTime.Now - start;
            DatabaseStats.totalQueryTime += spent.Milliseconds;
            DatabaseStats.totalQueries++;

            return pDataTable;
        }

        /// <summary>
        /// Runs the given query on the database
        /// </summary>
        /// <exception cref="QueryException">If the query fails</exception>
        public void runQuery()
        {
            DateTime start = DateTime.Now;
            try
            {
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Writer.LogQueryError(ex, command.CommandText);
            }

            TimeSpan spent = DateTime.Now - start;
            DatabaseStats.totalQueryTime += spent.Milliseconds;

            string[] queries = command.CommandText.Split(';');
            DatabaseStats.totalQueries += queries.Length;
        }

        /// <summary>
        /// This method returns the id of the last inserted id in long format
        /// </summary>
        /// <returns>A long value of the inserted id</returns> 
        /// <exception cref="QueryException">If the query fails</exception>
        public long insertQuery()
        {
            DateTime start = DateTime.Now;
            long lastID = 0;
            try
            {
                command.ExecuteScalar();
                lastID = command.LastInsertedId;
                
            }
            catch (Exception ex)
            {
                Writer.LogQueryError(ex, command.CommandText);
            }

            TimeSpan spent = DateTime.Now - start;
            DatabaseStats.totalQueryTime += spent.Milliseconds;
            DatabaseStats.totalQueries++;

            return lastID;
        }


        /// <summary>
        /// returns an integer
        /// </summary>
        public int getInteger()
        {
            DateTime start = DateTime.Now;
            int i = 0;
            try
            {

                object item = command.ExecuteScalar();
                if (item != null)
                {
                    int.TryParse(item.ToString(), out i);
                }
            }
            catch (Exception ex)
            {
                Writer.LogQueryError(ex, command.CommandText);
            }

            TimeSpan spent = DateTime.Now - start;
            DatabaseStats.totalQueryTime += spent.Milliseconds;
            DatabaseStats.totalQueries++;

            return i;
        }       

        /// <see cref="IRegularQueryAdapter"/>
        public DataRow getRow()
        {
            DateTime start = DateTime.Now;
            DataRow dReturn = null;

            try
            {
                DataSet tmpSet = new DataSet();
                using (MySqlDataAdapter pAdapter = new MySqlDataAdapter(command))
                {
                    pAdapter.Fill(tmpSet);
                }
                if (tmpSet.Tables.Count > 0 && tmpSet.Tables[0].Rows.Count == 1)
                    dReturn = tmpSet.Tables[0].Rows[0];
            }
            catch (Exception ex)
            {
                Writer.LogQueryError(ex, command.CommandText);
            }

            TimeSpan spent = DateTime.Now - start;
            DatabaseStats.totalQueryTime += spent.Milliseconds;
            DatabaseStats.totalQueries++;

            return dReturn;
        }


        public void runFastQuery(string query)
        {
            DateTime start = DateTime.Now;
            setQuery(query);
            runQuery();

            TimeSpan spent = DateTime.Now - start;
            DatabaseStats.totalQueryTime += spent.Milliseconds;
            DatabaseStats.totalQueries++;
        }


        public string getString()
        {
            DateTime start = DateTime.Now;
            string pString = "";
            try
            {
                object item = command.ExecuteScalar();
                if(item != null)
                    pString = item.ToString();
            }
            catch (Exception ex) { //Out.writeSqlError(ex, command.CommandText);
                Writer.LogQueryError(ex, command.CommandText);
            }

            TimeSpan spent = DateTime.Now - start;
            DatabaseStats.totalQueryTime += spent.Milliseconds;
            DatabaseStats.totalQueries++;

            return pString;
        }

        public bool findsResult()
        {
            DateTime start = DateTime.Now;
            bool Found = false;
            try
            {
                using (MySqlDataReader dReader = command.ExecuteReader())
                {
                    Found = dReader.HasRows;
                }

            }
            catch (Exception ex)
            {
                Writer.LogQueryError(ex, command.CommandText);
            }

            TimeSpan spent = DateTime.Now - start;
            DatabaseStats.totalQueryTime += spent.Milliseconds;
            DatabaseStats.totalQueries++;

            return Found;
        }

        #endregion
    }
}