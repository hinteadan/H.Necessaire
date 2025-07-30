namespace H.Necessaire
{
    public class NotificationAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Note[] Notes { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return Address;

            return $"{Name} <{Address}>";
        }

        public static implicit operator NotificationAddress(string address) => new NotificationAddress { Address = address };
        public static implicit operator NotificationAddress((string address, string name) parts) => new NotificationAddress { Address = parts.address, Name = parts.name };
    }
}
