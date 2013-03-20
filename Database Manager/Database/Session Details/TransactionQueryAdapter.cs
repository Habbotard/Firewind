using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using Database_Manager.Database.Session_Details.Interfaces;


namespace Database_Manager.Database.Session_Details
{
    class TransactionQueryReactor : QueryAdapter, IQueryAdapter 
    {
        #region Declares
        /// <summary>
        /// Indicates if all queries where correct
        /// </summary>
        private bool finishedTransaction;

        /// <summary>
        /// The transaction of the current item
        /// </summary>
        private MySqlTransaction transaction;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Database session
        /// </summary>
        /// <param name="client">The client which holds the connection</param>
        internal TransactionQueryReactor(DatabaseClient client) : base (client)
        {
            this.initTransaction();
        }
        #endregion

        #region Auto commit information
        /// <summary>
        /// Gets the auto commit information about this item
        /// </summary>
        /// <returns></returns>
        internal bool getAutoCommit()
        {
            return false;
        }

        /// <summary>
        /// Sets the auto commit of the current item
        /// </summary>
        /// <param name="commit">The new autocommit value</param>
        private void initTransaction()
        {
            this.command = client.getNewCommand();
            this.transaction = client.getTransaction();
            this.command.Transaction = transaction;
            this.command.Connection = transaction.Connection;
            this.finishedTransaction = false;
        }

        
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the current item
        /// </summary>
        public void Dispose()
        {
            if (!this.finishedTransaction)
            {
                throw new Database_Exceptions.TransactionException("The transaction needs to be completed by commit() or rollback() before you can dispose this item.");
            }
            command.Dispose();
            client.reportDone();
        }

        #endregion

        #region Query kinds

        /// <summary>
        /// Does a rollback after a failed query
        /// </summary>
        /// <exception cref="TransActionException">When an rollback action fails</exception>
        public void doRollBack()
        {
            try
            {
                this.transaction.Rollback();
                this.finishedTransaction = true;
            }
            catch (MySqlException ex)
            {
                throw new Database_Exceptions.TransactionException(ex.Message);
            }
        }

        /// <summary>
        /// Does a rollback after a failed query
        /// </summary>
        /// <exception cref="TransActionException">When an commit action fails</exception>
        public void doCommit()
        {
            try
            {
                this.transaction.Commit();
                this.finishedTransaction = true;
            }
            catch (MySqlException ex)
            {
                throw new Database_Exceptions.TransactionException(ex.Message);
            }
        }

        #endregion
    }
}
