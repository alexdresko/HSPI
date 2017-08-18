using Scheduler.Classes;

namespace Hspi.HspiPlugin3.Events
{
    public class ConfigChangeArgs
    {
        public ConfigChangeType Type { get; set; }

        public string WhatChanged { get; set; }

        public Dac Dac { get; set; }

        public int DeviceReferenceNumber { get; set; }

        public string Id { get; set; }
    }
}