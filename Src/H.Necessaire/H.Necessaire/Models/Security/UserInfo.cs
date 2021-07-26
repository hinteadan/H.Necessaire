namespace H.Necessaire
{
    public class UserInfo : IDentityBase
    {
        public string Username
        {
            get { return IDTag; }
            set { IDTag = value; }
        }

        public string Email
        {
            get { return GetNoteValueFor(nameof(Email)); }
            set { SetNoteValueFor(nameof(Email), value); }
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(DisplayName))
                return DisplayName;

            if (!string.IsNullOrWhiteSpace(Username))
                return Username;

            if (!string.IsNullOrWhiteSpace(Email))
                return Email;

            return ID.ToString();
        }
    }
}
