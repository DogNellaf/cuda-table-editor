using table_editor.classes;
using Xunit;

namespace table_editor.Tests
{
    public class FormulaParserTests
    {
        // ── IsFormula ─────────────────────────────────────────────────────────────

        [Fact]
        public void IsFormula_StartsWithEquals_True()
        {
            Assert.True(FormulaParser.IsFormula("=1A1+2"));
        }

        [Fact]
        public void IsFormula_PlainText_False()
        {
            Assert.False(FormulaParser.IsFormula("hello"));
        }

        [Fact]
        public void IsFormula_EmptyString_False()
        {
            Assert.False(FormulaParser.IsFormula(string.Empty));
        }

        [Fact]
        public void IsFormula_Null_False()
        {
            Assert.False(FormulaParser.IsFormula(null!));
        }

        // ── GetBody ───────────────────────────────────────────────────────────────

        [Fact]
        public void GetBody_RemovesLeadingEquals()
        {
            Assert.Equal("1A1+2", FormulaParser.GetBody("=1A1+2"));
        }

        [Fact]
        public void GetBody_EmptyFormula_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, FormulaParser.GetBody("="));
        }

        // ── ExtractReferences ─────────────────────────────────────────────────────

        [Fact]
        public void ExtractReferences_NoRefs_EmptyList()
        {
            var refs = FormulaParser.ExtractReferences("1+2*3");
            Assert.Empty(refs);
        }

        [Fact]
        public void ExtractReferences_SingleRef_CorrectParsed()
        {
            var refs = FormulaParser.ExtractReferences("1A3+2");
            Assert.Single(refs);
            Assert.Equal(0, refs[0].Row);   // row 1 → 0-based = 0
            Assert.Equal(3, refs[0].Col);   // col 3 stays as 3
            Assert.Equal("1A3", refs[0].Token);
        }

        [Fact]
        public void ExtractReferences_MultipleRefs_AllFound()
        {
            var refs = FormulaParser.ExtractReferences("2A1+3A2");
            Assert.Equal(2, refs.Count);
        }

        [Fact]
        public void ExtractReferences_RowConvertedToZeroBased()
        {
            var refs = FormulaParser.ExtractReferences("5A2");
            Assert.Equal(4, refs[0].Row);   // row 5 → 0-based = 4
        }

        [Fact]
        public void ExtractReferences_ColStaysOneBased()
        {
            var refs = FormulaParser.ExtractReferences("1A7");
            Assert.Equal(7, refs[0].Col);
        }

        [Fact]
        public void ExtractReferences_LargeRowAndCol()
        {
            var refs = FormulaParser.ExtractReferences("10A15");
            Assert.Equal(9, refs[0].Row);    // 10 - 1
            Assert.Equal(15, refs[0].Col);
            Assert.Equal("10A15", refs[0].Token);
        }

        [Fact]
        public void ExtractReferences_TokenMatchesOriginalText()
        {
            var refs = FormulaParser.ExtractReferences("3A4*2");
            Assert.Equal("3A4", refs[0].Token);
        }
    }
}
