using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class DataExtensionsScenarios
    {
        [Fact(DisplayName = "Batch returns NULL if the given collection is NULL")]
        public void Batch_Returns_Null_If_Collection_Is_Null()
        {
            (null as IEnumerable<int>).Batch(5).Should().BeNull("the given collection is NULL");
        }

        [Fact(DisplayName = "Batch returns empty collection if the given collection is empty")]
        public void Batch_Returns_Empty_If_Collection_Is_Empty()
        {
            Enumerable.Empty<int>().Batch(5).Should().BeEmpty("the given collection is NULL");
        }

        [Fact(DisplayName = "Batch throws invalid operation exception if the given batch size is smaller than 1")]
        public void Batch_Throws_IvalidOperationException_If_BatchSize_Is_Smaller_Than_One()
        {
            new Action(() => new int[] { 1 }.Batch(0)).Should().Throw<InvalidOperationException>("the batch size is zero");
            new Action(() => new int[] { 1 }.Batch(-3)).Should().Throw<InvalidOperationException>("the batch size is negative");
        }

        [Fact(DisplayName = "Batch returns the collection itself if the given batch size is greater than or equal to the collection count")]
        public void Batch_Returns_Self_If_Batch_Size_Is_Greater_Than_Or_Equal_To_Collection_Count()
        {
            int[] collection = new int[] { 1, 2, 3, 4, 5 };

            collection.Batch(10).Should().HaveCount(1, "the batch size is greater than the collection count");
            collection.Batch(10).Single().Should().Equal(collection, "the batch is self as the batch size is greater than the collection count");

            collection.Batch(collection.Length).Should().HaveCount(1, "the batch size is equal to the collection count");
            collection.Batch(collection.Length).Single().Should().Equal(collection, "the batch is self as the batch size is equal to the collection count");
        }

        [Fact(DisplayName = "Batch creates expected batches when all params are ok")]
        public void Batch_Creates_Expected_Batches_If_All_Params_Are_Correct()
        {
            int[] collection = new int[] { 1, 2, 3, 4, 5 };

            IEnumerable<IEnumerable<int>> batches = collection.Batch(2);

            batches.Should().HaveCount(3, "we should have 2 complete batches and 1 incomplete");
            batches.ElementAt(0).Should().BeEquivalentTo(1, 2);
            batches.ElementAt(1).Should().BeEquivalentTo(3, 4);
            batches.ElementAt(2).Should().BeEquivalentTo(5);
        }

        [Fact(DisplayName = "Stream Read As String Correctly Converts A Stream To String")]
        public async Task Stream_Read_As_String_Correctly_Converts_A_Stream_To_String()
        {
            string readValue = await (null as Stream).ReadAsStringAsync();
            readValue.Should().BeNull("the stream is null");

            readValue = await GenerateStream(null).ReadAsStringAsync();
            readValue.Should().BeEmpty("we wrote null to the stream");

            readValue = await GenerateStream(string.Empty).ReadAsStringAsync();
            readValue.Should().BeEmpty("we wrote string.Empty to the stream");

            readValue = await GenerateStream("test").ReadAsStringAsync();
            readValue.Should().Be("test", "that what the stream contains");
        }

        private static Stream GenerateStream(string content)
        {
            Stream result = new MemoryStream();

            if (string.IsNullOrEmpty(content))
                return result;

            using (StreamWriter writer = new StreamWriter(result, Encoding.UTF8, content.Length, true))
            {
                writer.Write(content);
                writer.Flush();
                result.Seek(0, SeekOrigin.Begin);
            }

            return result;
        }
    }
}
