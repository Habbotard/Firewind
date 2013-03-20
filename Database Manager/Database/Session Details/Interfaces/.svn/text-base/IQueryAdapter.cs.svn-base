using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Database_Manager.Session_Details.Interfaces;

namespace Database_Manager.Database.Session_Details.Interfaces
{
    public interface IQueryAdapter : IRegularQueryAdapter, IDisposable
    {
        void doRollBack();
        void doCommit();
        
        long insertQuery();
        void runQuery();
    }
}
