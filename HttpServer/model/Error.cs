﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class Error
    {
        public Error(string message)
        {
            this.Message = message;
        }

        public string Code { get; set; }
        public string Message { get; set;  }
    }
}