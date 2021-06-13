using FluentAssertions;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class CollectionExtensionsScenarios
    {
        [Fact(DisplayName = "Array Extensions Can Push A New Elements At The End Of The Array")]
        public void Array_Extensions_Can_Push_A_New_Elements_At_The_End_Of_The_Array()
        {
            int[] array = null;

            array = array.Push(1);
            array.Should().NotBeNullOrEmpty("be just pushed a new element");
            array.Length.Should().Be(1, "we pushed exactly one element");

            array = array.Push(2).Push(3);
            array.Length.Should().Be(3, "we pushed 2 more elements");
            array.Should().BeEquivalentTo(new[] { 1, 2, 3 }, "we pushed 1,2,3");
        }

        [Fact(DisplayName = "Array Extensions Can Remove Elements based on predicate")]
        public void Array_Extensions_Can_Remove_Elements_based_on_predicate()
        {
            int[] array = null;
            array = array.Remove(null);
            array.Should().BeNull("array is null so nothing to remove");

            array = new[] { 1, 2, 3, 4, 5, 6 };
            array = array.Remove(null);
            array.Should().BeEquivalentTo(array, "no predicate was specified, so nothing should be removed");

            array = array.Remove(x => x % 2 == 0);
            array.Should().BeEquivalentTo(new[] { 1, 3, 5 }, "we removed all even numbers");
        }

        [Fact(DisplayName = "Array Extensions Can Set An Element Base On Predicate")]
        public void Array_Extensions_Can_Set_An_Element_Base_On_Predicate()
        {
            Note[] array = null;
            array = array.Set(x => x.Id == "Email", new Note("Email", "test@test.com"));
            array.Should().NotBeNullOrEmpty("we just added an element");
            array.Should().BeEquivalentTo(new Note("Email", "test@test.com").AsArray(), "we just added this specific element");
        }

        [Fact(DisplayName = "Array Extensions Can Get An Element Base On Predicate")]
        public void Array_Extensions_Can_Get_An_Element_Base_On_Predicate()
        {
            Note[] array = null;
            Note note = array.Get(x => x.Id == "Email");
            note.Should().Be(default(Note), "the note was not found so we defaulted to the specified default value");

            array = new Note("Email", "test@test.com").AsArray();
            note = array.Get(null);
            note.Should().Be(default(Note), "the note was not found because no predicate was specified so we defaulted to the specified default value");
            note = array.Get(x => x.Id == "Email");
            note.Should().Be(array[0], "we retrieved the Email note");
        }
    }
}
