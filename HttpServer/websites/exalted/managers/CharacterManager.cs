﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using HttpServer.websites.exalted;
using HttpServer.helpers;
using HttpServer.websites.exalted.model;

namespace HttpServer.websites.exalted.managers
{
    public static class CharacterManager
    {
        public static Character CreateCharacter(User user, string name, string data)
        {
            if (name == string.Empty)
            {
                return null;
            }

            if (CharacterManager.Exists(name))
            {
                return null;
            }

            string userId = user == null ? string.Empty : user.Id.ToString();

            IDbDataParameter paramUserID = WebSite.Database.CreateParameter("@UserId", userId);
            IDbDataParameter paramName = WebSite.Database.CreateParameter("@Name", name);
            IDbDataParameter paramData = WebSite.Database.CreateParameter("@Data", data);

            WebSite.Database.ExecuteNonQuery("INSERT INTO characters (UserId, Name, Data) VALUES (@UserId, @Name, @Data)", paramUserID, paramName, paramData);
            
            return CharacterManager.GetCharacter(name);
        }        

        public static Character GetCharacter(int id)
        {
            if (id == -1)
            {
                return null;
            }

            IDbDataParameter parameter = WebSite.Database.CreateParameter("@ID", id);
            DataTable table = WebSite.Database.ExecuteQuery("SELECT * FROM characters WHERE Id=@ID", parameter);

            if (table == null)
            {
                return null;
            }

            if (table.Rows.Count < 1)
            {
                return null;
            }

            DataRow dataRow = table.Rows[0];

            return CharacterHelper.FromDataRow(dataRow);
        }

        public static Character GetCharacter(string characterName)
        {
            IDbDataParameter parameter = WebSite.Database.CreateParameter("@Name", characterName);
            DataTable table = WebSite.Database.ExecuteQuery("SELECT * FROM characters WHERE Name=@Name", parameter);

            if (table == null)
            {
                return null;
            }

            if (table.Rows.Count < 1)
            {
                return null;
            }

            DataRow dataRow = table.Rows[0];

            return CharacterHelper.FromDataRow(dataRow);
        }

        public static Character[] GetCharacters(User user)
        {
            IDbDataParameter parameter = WebSite.Database.CreateParameter("@UserId", user.Id);
            DataTable table = WebSite.Database.ExecuteQuery("SELECT * FROM characters WHERE UserId=@UserId", parameter);

            if (table == null)
            {
                return new Character[0];
            }

            if (table.Rows.Count < 1)
            {
                return new Character[0];
            }

            List<Character> characters = new List<Character>();

            foreach (DataRow row in table.Rows)
            {
                characters.Add(CharacterHelper.FromDataRow(row));
            }

            return characters.ToArray();
        }

        public static void DeleteCharacter(int id)
        {
            IDbDataParameter paramUserID = WebSite.Database.CreateParameter("@Id", id);

            WebSite.Database.ExecuteNonQuery("DELETE FROM characters WHERE Id=@Id", paramUserID);
        }

        public static bool Exists(string name)
        {
            IDbDataParameter paramUsername = WebSite.Database.CreateParameter("@Name", name);
            object result = WebSite.Database.ExecuteScalar("SELECT COUNT(*) FROM characters WHERE Name=@Name", paramUsername);

            int count = 0;

            int.TryParse(result.ToString(), out count);

            return (count > 0);
        }
    }
}