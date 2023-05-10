﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhimsyLog.Models
{
    public class LogItem
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public LogItem(string message, DateTime timestamp)
        {
            Message = message;
            Timestamp = timestamp;
        }
    }
}
