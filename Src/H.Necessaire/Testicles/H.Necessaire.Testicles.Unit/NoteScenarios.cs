using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class NoteScenarios
    {
        [Fact(DisplayName = "Notes Can Be Constructed From Dictionary")]
        public void Notes_Can_Be_Constructed_From_Dictionary()
        {
            var dictionary = new Dictionary<string, string> {
                { "NoteA", "ValueA" },
                { "NoteB", "ValueB" },
            };

            var notes = Note.FromDictionary(dictionary);

            notes.Should().BeEquivalentTo(["ValueA".NoteAs("NoteA"), "ValueB".NoteAs("NoteB")], because: "Notes should be correctly constructed from a dictionary");
        }

        [Fact(DisplayName = "Notes Can Be Constructed From Strings")]
        public void Notes_Can_Be_Constructed_From_Strings()
        {
            Note[] notes = ["NoteA =::= ValueA", "NoteB =::= ValueB"];

            notes.Should().BeEquivalentTo(["ValueA".NoteAs("NoteA"), "ValueB".NoteAs("NoteB")], because: "Notes should be correctly constructed from strings");
        }
    }
}
