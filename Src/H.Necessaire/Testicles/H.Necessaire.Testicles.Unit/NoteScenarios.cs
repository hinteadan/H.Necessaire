using FluentAssertions;
using System.Collections.Generic;
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

        [Fact(DisplayName = "Notes Can Be DeConstructed To Strings")]
        public void Notes_Can_Be_DeConstructed_To_Strings()
        {
            string[] strings = ["ValueA".NoteAs("NoteA"), "ValueB".NoteAs("NoteB")];

            strings.Should().BeEquivalentTo(["NoteA =::= ValueA", "NoteB =::= ValueB"], because: "Notes should be correctly deconstructed to strings");
        }
    }
}
