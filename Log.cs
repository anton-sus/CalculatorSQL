﻿namespace CalculatorSQL
{
    internal class Log
    {
        public int id { get; set; }
        private string info;
        public string Info 
        { 
            get { return info; }
            set { info = value; }
        }

        public Log() { }
        public Log(string info)
        {
            this.info = info;
        }
    }
}