using System;

namespace H.Necessaire
{
    public class SyncNode
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public string Description { get; set; }

        public string Uri { get; set; }

        public string SyncRequestRelativeUri { get; set; }

        public override string ToString()
        {
            return $"{Name ?? ID?.ToString()} ({Uri}{SyncRequestRelativeUri})";
        }
    }
}