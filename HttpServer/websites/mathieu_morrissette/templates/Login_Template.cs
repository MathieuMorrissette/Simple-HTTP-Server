﻿using HttpServer.websites.mathieu_morrissette.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.websites.mathieu_morrissette.templates
{
    public class Login_Template : ITemplate
    {
        public Error[] Errors { get; set; }

        public string Render()
        {
            Header_Template headerTemplate = new Header_Template();
            Footer_Template footerTemplate = new Footer_Template();

            string content = File.ReadAllText(WebSite.WEBSITE_ROOT_PATH + "public/html/login.html");

            string error_data = string.Empty;

            foreach (Error error in Errors)
            {
                Error_Template error_template = new Error_Template(error);
                error_data += error_template.Render();
            }

            content = content.Replace("__Error__", error_data);

            return headerTemplate.Render() + content + footerTemplate.Render();
        }
    }
}
