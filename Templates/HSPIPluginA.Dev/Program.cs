﻿using Hspi;

//TODO: namespace HSPI_$safeprojectname$

namespace HSPIPluginA.Dev
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Connector.Connect<HSPI>(args);
        }
    }
}