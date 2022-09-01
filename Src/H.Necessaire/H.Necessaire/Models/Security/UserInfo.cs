using System;
using System.Linq;

namespace H.Necessaire
{
    public class UserInfo : ConsumerIdentity
    {
        const string noteIdIsIronMan = "X-H.Necessaire-IsIronMan";
        static readonly Guid ironManID = Guid.Parse("{C756A1A2-4693-4425-B2C8-F11D6A8A9A6F}");

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

        public bool IsIronMan() => Notes?.Any(x => x.ID == noteIdIsIronMan) == true;

        public static UserInfo BuildIronMan(Guid? id = null, string userName = null, string displayName = null, string email = null)
        {
            return
                new UserInfo
                {
                    ID = id ?? ironManID,
                    Username = !string.IsNullOrWhiteSpace(userName) ? userName : "IronMan",
                    Email = !string.IsNullOrWhiteSpace(email) ? email : "ironman@necessaire.dev",
                    DisplayName = !string.IsNullOrWhiteSpace(displayName) ? displayName : "Iron Man",
                }
                .And(x =>
                {
                    x.Notes = x.Notes.Add(true.ToString().NoteAs(noteIdIsIronMan));
                });
        }

        public static UserInfo BuildIronMan(UserInfo peasant)
        {
            return
                BuildIronMan(
                    id: peasant?.ID,
                    userName: peasant?.Username,
                    displayName: peasant?.DisplayName,
                    email: peasant?.Email
                );
        }
    }
}
