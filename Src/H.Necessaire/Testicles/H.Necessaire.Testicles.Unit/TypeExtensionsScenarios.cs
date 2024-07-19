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
    public class TypeExtensionsScenarios
    {
        [Fact(DisplayName = "Type IsMatch Finds Matching Type By Exact Entire Name")]
        public void Type_IsMatch_Finds_Matching_Type_By_Exact_Entire_Name()
        {
            var isMatch = typeof(DataBinMeta).IsMatch("DataBinMeta");
            isMatch.Should().BeTrue(because: "We tried to match a standard type by its entire name");

            isMatch = typeof(JustARandomType).IsMatch("JustARandomType");
            isMatch.Should().BeTrue(because: "We tried to match a private sub-type by its entire name");
        }

        [Fact(DisplayName = "Type IsMatch Finds Matching Type By The Beginning Of The Name")]
        public void Type_IsMatch_Finds_Matching_Type_By_The_Beginning_Of_The_Name()
        {
            var isMatch = typeof(DataBinMeta).IsMatch("DataBinMet");
            isMatch.Should().BeTrue(because: "We tried to match a standard type by its beginning of the name");

            isMatch = typeof(JustARandomType).IsMatch("JustARandom");
            isMatch.Should().BeTrue(because: "We tried to match a private sub-type by its beginning of the name");
        }

        [Fact(DisplayName = "Type IsMatch Finds Matching Type By The ID Attribute Match")]
        public void Type_IsMatch_Finds_Matching_Type_By_The_ID_Attribute_Match()
        {
            var isMatch = typeof(JustARandomType).IsMatch("IDofJustARandomType");
            isMatch.Should().BeTrue(because: "We tried to match a type by its ID Attribute");
        }

        [Fact(DisplayName = "Type IsMatch Finds Matching Type By Any Alias Attribute Match")]
        public void Type_IsMatch_Finds_Matching_Type_By_Any_Alias_Attribute_Match()
        {
            var isMatch = typeof(JustARandomType).IsMatch("AliasTwoofJustARandomType");
            isMatch.Should().BeTrue(because: "We tried to match a type by one of its Alias Attributes");
        }

        [Fact(DisplayName = "Type IsMatch Finds Matching Type In Expected Priority")]
        public void Type_IsMatch_Finds_Matching_Type_In_Expected_Priority()
        {
            var deps = IoC.NewDependencyRegistry();

            var matchedType = deps.Build<object>("JustARandom");
            matchedType.GetType().Should().Be(typeof(JustARandomTypeTwo), because: "JustARandomTypeTwo is ID-ed with JustARandomType");


            matchedType = deps.Build<object>("JustARandomT");
            matchedType.GetType().Should().Be(typeof(JustARandomTypeThree), because: "JustARandomTypeThree is Alias-ed with JustARandomT");

            matchedType = deps.Build<object>("JustARandomType");
            matchedType.GetType().Should().Be(typeof(JustARandomType), because: "JustARandomType's exact name is JustARandomType");

            matchedType = deps.Build<object>("JustARandomTypeT");
            matchedType.GetType().Should().Be(typeof(JustARandomTypeTwo), because: "JustARandomTypeTwo is the first type who's name starts with JustARandomTypeT");
        }

        [ID("IDofJustARandomType")]
        [Alias("AliasOneofJustARandomType", "AliasTwoofJustARandomType", "AliasThreeofJustARandomType")]
        private class JustARandomType
        {

        }

        [ID("JustARandom")]
        private class JustARandomTypeTwo
        {

        }

        [Alias("JustARandomT")]
        private class JustARandomTypeThree
        {

        }
    }
}
