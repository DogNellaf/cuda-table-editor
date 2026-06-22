using table_editor.classes;
using Xunit;

namespace table_editor.Tests
{
    public class CellTests
    {
        [Fact]
        public void DefaultConstructor_EmptyValue_TypeIsString()
        {
            var cell = new Cell();
            Assert.Equal(string.Empty, cell.Value);
            Assert.Equal(CellType.String, cell.Type);
        }

        [Fact]
        public void SetValue_Integer_TypeIsInteger()
        {
            var cell = new Cell("42");
            Assert.Equal(CellType.Integer, cell.Type);
        }

        [Fact]
        public void SetValue_NegativeInteger_TypeIsInteger()
        {
            var cell = new Cell("-7");
            Assert.Equal(CellType.Integer, cell.Type);
        }

        [Fact]
        public void SetValue_FloatWithDot_TypeIsFloat()
        {
            var cell = new Cell("3.14");
            Assert.Equal(CellType.Float, cell.Type);
        }

        [Fact]
        public void SetValue_FloatWithComma_TypeIsString()
        {
            // Comma-separated floats are locale-specific; invariant culture treats them as strings.
            var cell = new Cell("3,14");
            Assert.Equal(CellType.String, cell.Type);
        }

        [Fact]
        public void SetValue_Formula_TypeIsFormula()
        {
            var cell = new Cell("=1A1+2A2");
            Assert.Equal(CellType.Formula, cell.Type);
        }

        [Fact]
        public void SetValue_Text_TypeIsString()
        {
            var cell = new Cell("hello");
            Assert.Equal(CellType.String, cell.Type);
        }

        [Fact]
        public void SetValue_Null_EmptyAndTypeString()
        {
            var cell = new Cell();
            cell.Value = null!;
            Assert.Equal(string.Empty, cell.Value);
            Assert.Equal(CellType.String, cell.Type);
        }

        [Fact]
        public void DisplayValue_NonFormula_MatchesValue()
        {
            var cell = new Cell("99");
            Assert.Equal("99", cell.DisplayValue);
        }

        [Fact]
        public void DisplayValue_Formula_NotAutomaticallyComputed()
        {
            // DisplayValue for a formula cell stays empty until explicitly set.
            var cell = new Cell("=1A1");
            Assert.Equal(string.Empty, cell.DisplayValue);
        }

        [Fact]
        public void SetDisplayValue_UpdatesIndependently()
        {
            var cell = new Cell("=1A1");
            cell.DisplayValue = "42";
            Assert.Equal("=1A1", cell.Value);
            Assert.Equal("42", cell.DisplayValue);
        }

        [Fact]
        public void ToString_ReturnsValue()
        {
            var cell = new Cell("test");
            Assert.Equal("test", cell.ToString());
        }

        [Fact]
        public void PropertyChanged_FiredOnValueSet()
        {
            var cell = new Cell();
            int count = 0;
            cell.PropertyChanged += (_, _) => count++;
            cell.Value = "hello";
            Assert.True(count > 0);
        }

        [Fact]
        public void SetValue_Zero_TypeIsInteger()
        {
            var cell = new Cell("0");
            Assert.Equal(CellType.Integer, cell.Type);
        }

        [Theory]
        [InlineData("1.0")]
        [InlineData("0.5")]
        [InlineData("-3.14")]
        [InlineData("1e3")]
        public void SetValue_VariousFloats_TypeIsFloat(string value)
        {
            var cell = new Cell(value);
            Assert.Equal(CellType.Float, cell.Type);
        }
    }
}
