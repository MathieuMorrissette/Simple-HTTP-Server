﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace HttpServer
{
    public static class UserManager
    {
        public static User CreateUser(string username, string password)
        {
            if (UserManager.Exists(username))
            {
                return null;
            }

            IDbDataParameter paramUsername = WebSite.Database.CreateParameter("@Username", username);
            IDbDataParameter paramPassword = WebSite.Database.CreateParameter("@Password", password);

            WebSite.Database.ExecuteNonQuery("INSERT INTO users (Username, Password) VALUES (@Username, @Password)", paramUsername, paramPassword);
            
            return UserManager.GetUser(username);
        }

        public static User GetUser(string username)
        {
            IDbDataParameter parameter = WebSite.Database.CreateParameter("@Username", username);
            DataTable table = WebSite.Database.ExecuteQuery("SELECT Id, Username, Password FROM users WHERE Username=@Username", parameter);

            if (table == null)
            {
                return null;
            }

            if (table.Rows.Count < 1)
            {
                return null;
            }

            DataRow dataRow = table.Rows[0];

            return new User((int)dataRow[User.ID_FIELD], (string)dataRow[User.USERNAME_FIELD], (string)dataRow[User.PASSWORD_FIELD]);
        }

        public static bool Exists(string username)
        {
            IDbDataParameter paramUsername = WebSite.Database.CreateParameter("@Username", username);
            object result = WebSite.Database.ExecuteScalar("SELECT COUNT(*) FROM users WHERE Username=@Username", paramUsername);

            int count = 0;

            int.TryParse(result.ToString(), out count);

            return (count > 0);
        }

        public static bool Login(string username, string password, Client client)
        {
            User user = UserManager.GetUser(username);

            if (user == null)
            {
                return false;
            }

            if (PasswordManager.Verify(password, user.Hash))
            {
                client.Dictionary.Add("UID", user.ID);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
