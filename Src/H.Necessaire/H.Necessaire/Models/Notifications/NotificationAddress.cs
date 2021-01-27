namespace H.Necessaire
{
    public class NotificationAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return Address;

            return $"{Name} <{Address}>";
        }
    }
}
