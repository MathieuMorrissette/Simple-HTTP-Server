﻿using HttpServer.databases;
using HttpServer.managers;
using HttpServer.websites.mathieu_morrissette.classes;
using HttpServer.websites.mathieu_morrissette.controllers;
using HttpServer.websites.mathieu_morrissette.database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.websites.mathieu_morrissette
{
    public class WebSite : BaseWebsite
    {
        const string DEFAULT_ROUTE = "login";
        public const string WEBSITE_ROOT_PATH = "../../websites/mathieu_morrissette/";

        public static BaseDatabase Database { get; private set; }

        private string connectionString;

        private static Dictionary<string, Func<IController>> routes = new Dictionary<string, Func<IController>>
        {
            { "login", () => new Login()},
            { "register", () => new Register()},
            { "home", () =>  new Home()},
            {"profile", () => new Profile() },
            { "images", () => new FileProvider()},
            { "javascript", () => new FileProvider()},
            { "css", () => new FileProvider()},
            { "font", () => new FileProvider()},
            { "user", () => new UserController()},
            { "friends", () => new Friends()},
            { "chat", () => new Chat()},
            { "api", () => new API()}
        };

        IController controller;

        private void LoadConfiguration()
        {
            MathieuMorrissetteConfig config = JsonConvert.DeserializeObject<MathieuMorrissetteConfig>(File.ReadAllText(WEBSITE_ROOT_PATH + "configs/config.cfg"));

            this.connectionString = config.database_connection_string;
        }

        public WebSite(Client client, HttpListenerContext context) : base(client, context)
        {
            this.LoadConfiguration();

            WebSite.Database = new DatabaseMySql(this.connectionString);

            //Create tables if they don't exists
            DatabaseInitialiser.CreateDatabase(WebSite.Database);

            string[] parsedArgs = WebHelper.GetUrlArguments(this.Context.Request.Url.Segments);

            string controller = string.Empty;

            if (parsedArgs.Length == 0)
            {
                controller = DEFAULT_ROUTE;
            }
            else if (parsedArgs[0] == string.Empty)
            {
                controller = DEFAULT_ROUTE;
            }
            else
            {
                controller = parsedArgs[0];
            }

            if (routes.ContainsKey(controller))
            {
                this.controller = routes[controller]();
            }
            else
            {
                Error();
                return;
            }
        }

        private void Error()
        {
            this.Context.Response.StatusCode = 404;
            this.Context.Response.OutputStream.Close();
        }

        public override void HandleRequest()
        {
            if (controller == null)
            {
                this.Context.Response.OutputStream.Close();
                return;
            }

            string[] arguments = WebHelper.GetUrlArguments(this.Context.Request.Url.Segments);

            // Remove the first arguments since it is the controller name.
            arguments = arguments.Skip(1).ToArray();

            controller.HandleRequest(this.Client, this.Context, arguments);
        }
    }
}
