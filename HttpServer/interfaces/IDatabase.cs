﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public interface IDatabase
    {
        IDbConnection CreateConnection();
        IDbDataParameter CreateParameter(string parameterName, object value);
        IDbCommand CreateCommand(string query);
    }
}
