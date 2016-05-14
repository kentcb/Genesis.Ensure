namespace Genesis.Ensure.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Genesis.Ensure;
    using Xunit;

    public sealed class EnsureFixture
    {
        [Fact]
        public void argument_not_null_does_not_throw_for_non_null_values()
        {
            Ensure.ArgumentNotNull("", "whatever");
            Ensure.ArgumentNotNull(new object(), "whatever");
            Ensure.ArgumentNotNull((int?)5, "whatever");
        }

        [Fact]
        public void argument_not_null_throws_for_null_values()
        {
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNull((string)null, "whatever"));
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNull((object)null, "whatever"));
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNull((int?)null, "whatever"));
        }

        [Fact]
        public void argument_not_null_does_not_throw_for_non_null_enumerations()
        {
            Ensure.ArgumentNotNull(new List<string>(), "whatever", true);
            Ensure.ArgumentNotNull(new string[] { "one", "two" }, "whatever", true);
        }

        [Fact]
        public void argument_not_null_throws_for_null_enumerations()
        {
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNull((List<string>)null, "whatever", true));
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNull((string[])null, "whatever", true));
        }

        [Fact]
        public void argument_not_null_does_not_throw_for_null_items_in_enumeration_if_assert_contents_is_false()
        {
            Ensure.ArgumentNotNull(new string[] { "foo", null }, "whatever", false);
        }

        [Fact]
        public void argument_not_null_throws_if_item_is_null_and_assert_contents_is_true()
        {
            Assert.Throws<ArgumentException>(() => Ensure.ArgumentNotNull(new string[] { "foo", null }, "whatever", true));
        }

        [Fact]
        public void argument_not_null_or_empty_does_not_throw_for_non_null_and_non_empty_values()
        {
            Ensure.ArgumentNotNullOrEmpty("foo", "whatever");
            Ensure.ArgumentNotNullOrEmpty("bar", "whatever");
            Ensure.ArgumentNotNullOrEmpty((IEnumerable<string>)new string[] { "foo" }, "whatever");
            Ensure.ArgumentNotNullOrEmpty(new string[] { "foo" }, "whatever");
        }

        [Fact]
        public void argument_not_null_or_empty_throws_if_argument_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNullOrEmpty((string)null, "whatever"));
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNullOrEmpty((IEnumerable<string>)null, "whatever"));
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNullOrEmpty((string[])null, "whatever"));
        }

        [Fact]
        public void argument_not_null_or_empty_throws_if_argument_is_empty()
        {
            Assert.Throws<ArgumentException>(() => Ensure.ArgumentNotNullOrEmpty("", "whatever"));
            Assert.Throws<ArgumentException>(() => Ensure.ArgumentNotNullOrEmpty(Enumerable.Empty<string>(), "whatever"));
            Assert.Throws<ArgumentException>(() => Ensure.ArgumentNotNullOrEmpty(new string[0], "whatever"));
        }

        [Fact]
        public void argument_not_null_or_whitespace_does_not_throw_for_non_null_and_non_whitespace_values()
        {
            Ensure.ArgumentNotNullOrWhiteSpace("foo", "whatever");
            Ensure.ArgumentNotNullOrWhiteSpace("bar", "whatever");
        }

        [Fact]
        public void argument_not_null_or_whitespace_throws_if_argument_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNullOrWhiteSpace(null, "whatever"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("  \t \r\n  ")]
        public void argument_not_null_or_whitespace_throws_if_argument_is_invalid(string invalidValue)
        {
            Assert.Throws<ArgumentException>(() => Ensure.ArgumentNotNullOrWhiteSpace(invalidValue, "whatever"));
        }

        [Fact]
        public void argument_condition_does_not_throw_if_condition_holds()
        {
            Ensure.ArgumentCondition(true, "message", "whatever");
        }

        [Theory]
        [InlineData("message", "argumentName")]
        [InlineData("foo", "bar")]
        public void argument_condition_throws_if_condition_does_not_hold(string message, string argumentName)
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentCondition(false, message, argumentName));
            Assert.StartsWith(message, ex.Message);
            Assert.Equal(argumentName, ex.ParamName);
        }

        [Fact]
        public void argument_is_valid_enum_throws_if_given_invalid_flag_enumeration_value()
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum((FlagsEnum)68, "test"));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value '68' is not valid for flags enumeration 'Genesis.Ensure.UnitTests.EnsureFixture+FlagsEnum'.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_throws_if_given_invalid_zero_flag_enumeration_value()
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum((FlagsEnumNoNone)0, "test"));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value '0' is not valid for flags enumeration 'Genesis.Ensure.UnitTests.EnsureFixture+FlagsEnumNoNone'.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_does_not_throw_if_enumeration_flags_are_valid()
        {
            Ensure.ArgumentIsValidEnum(FlagsEnum.None, "test");
            Ensure.ArgumentIsValidEnum(FlagsEnum.One, "test");
            Ensure.ArgumentIsValidEnum(FlagsEnum.Two, "test");
            Ensure.ArgumentIsValidEnum(FlagsEnum.Three, "test");
            Ensure.ArgumentIsValidEnum(FlagsEnum.Four, "test");
            Ensure.ArgumentIsValidEnum(FlagsEnum.One | FlagsEnum.Two, "test");
            Ensure.ArgumentIsValidEnum(FlagsEnum.Two | FlagsEnum.Four, "test");
            Ensure.ArgumentIsValidEnum(FlagsEnum.One | FlagsEnum.Two | FlagsEnum.Three, "test");
            Ensure.ArgumentIsValidEnum(FlagsEnum.One | FlagsEnum.Two | FlagsEnum.Three | FlagsEnum.Four, "test");
        }

        [Fact]
        public void argument_is_valid_enum_throws_if_enumeration_value_is_invalid()
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum((DayOfWeek)69, "test"));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value '69' is not defined for enumeration 'System.DayOfWeek'.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_does_not_throw_if_enumeration_values_are_valid()
        {
            Ensure.ArgumentIsValidEnum(DayOfWeek.Monday, "test");
            Ensure.ArgumentIsValidEnum((DayOfWeek)3, "test");
        }

        [Fact]
        public void argument_is_valid_enum_works_for_byte_flags_enumeration()
        {
            Ensure.ArgumentIsValidEnum(ByteFlagsEnum.One | ByteFlagsEnum.Three, "test");
            Ensure.ArgumentIsValidEnum(ByteFlagsEnum.None, "test");
            Ensure.ArgumentIsValidEnum(ByteFlagsEnum.One | ByteFlagsEnum.Two | ByteFlagsEnum.Three, "test");

            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum((ByteFlagsEnum)80, "test"));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value '80' is not valid for flags enumeration 'Genesis.Ensure.UnitTests.EnsureFixture+ByteFlagsEnum'.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_works_for_byte_enumeration()
        {
            Ensure.ArgumentIsValidEnum(ByteEnum.One, "test");
            Ensure.ArgumentIsValidEnum(ByteEnum.Two, "test");
            Ensure.ArgumentIsValidEnum(ByteEnum.Three, "test");

            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum((ByteEnum)10, "test"));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value '10' is not defined for enumeration 'Genesis.Ensure.UnitTests.EnsureFixture+ByteEnum'.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_throws_if_no_valid_values_are_provided()
        {
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentIsValidEnum(DayOfWeek.Monday, "test", null));
        }

        [Fact]
        public void argument_is_valid_enum_throws_if_flag_value_is_not_a_valid_value()
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum(FlagsEnum.Three, "test", FlagsEnum.One, FlagsEnum.Two, FlagsEnum.Four));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value 'Three' is not valid for flags enumeration 'Genesis.Ensure.UnitTests.EnsureFixture+FlagsEnum'.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_throws_if_valid_flag_values_are_provided_but_the_enumeration_value_is_not_a_valid_flag_enumeration_value()
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum((FlagsEnum)68, "test", FlagsEnum.One, FlagsEnum.Two, FlagsEnum.Four));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value '68' is not valid for flags enumeration 'Genesis.Ensure.UnitTests.EnsureFixture+FlagsEnum'.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_throws_if_invalid_zero_flag_enumeration_value_is_passed_in_but_it_is_not_a_valid_value()
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum((FlagsEnumNoNone)0, "test", FlagsEnumNoNone.One));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value '0' is not valid for flags enumeration 'Genesis.Ensure.UnitTests.EnsureFixture+FlagsEnumNoNone'.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_throws_if_zero_flag_enumeration_value_is_passed_in_but_it_is_not_a_valid_value()
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum(FlagsEnum.None, "test", FlagsEnum.One));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value 'None' is not valid for flags enumeration 'Genesis.Ensure.UnitTests.EnsureFixture+FlagsEnum'.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_does_not_throw_if_flag_enumeration_values_are_valid()
        {
            var validValues = new FlagsEnum[] { FlagsEnum.One, FlagsEnum.Two, FlagsEnum.Four, FlagsEnum.None };
            Ensure.ArgumentIsValidEnum(FlagsEnum.None, "test", validValues);
            Ensure.ArgumentIsValidEnum(FlagsEnum.One, "test", validValues);
            Ensure.ArgumentIsValidEnum(FlagsEnum.Two, "test", validValues);
            Ensure.ArgumentIsValidEnum(FlagsEnum.Four, "test", validValues);
            Ensure.ArgumentIsValidEnum(FlagsEnum.One | FlagsEnum.Two, "test", validValues);
            Ensure.ArgumentIsValidEnum(FlagsEnum.One | FlagsEnum.Four, "test", validValues);
            Ensure.ArgumentIsValidEnum(FlagsEnum.One | FlagsEnum.Two | FlagsEnum.Four, "test", validValues);
        }

        [Fact]
        public void argument_is_valid_enum_throws_if_value_is_not_a_valid_value()
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum(DayOfWeek.Monday, "test", DayOfWeek.Friday, DayOfWeek.Sunday));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value 'Monday' is defined for enumeration 'System.DayOfWeek' but it is not permitted in this context.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_throws_if_valid_values_are_provided_but_the_enumeration_value_is_not_a_valid_enumeration_value()
        {
            var ex = Assert.Throws<ArgumentException>(() => Ensure.ArgumentIsValidEnum((DayOfWeek)69, "test", DayOfWeek.Friday, DayOfWeek.Sunday));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, "Enum value '69' is not defined for enumeration 'System.DayOfWeek'.{0}Parameter name: test", Environment.NewLine), ex.Message);
        }

        [Fact]
        public void argument_is_valid_enum_does_not_throw_if_valid_values_are_specified_and_enumeration_values_are_valid()
        {
            var validValues = new DayOfWeek[] { DayOfWeek.Friday, DayOfWeek.Sunday, DayOfWeek.Saturday };
            Ensure.ArgumentIsValidEnum(DayOfWeek.Friday, "test", validValues);
            Ensure.ArgumentIsValidEnum(DayOfWeek.Sunday, "test", validValues);
            Ensure.ArgumentIsValidEnum(DayOfWeek.Saturday, "test", validValues);
        }

        [Fact]
        public void condition_does_not_throw_if_condition_holds()
        {
            Ensure.Condition(true, () => new Exception());
        }

        [Fact]
        public void condition_throws_if_condition_does_not_hold()
        {
            Assert.Throws<InvalidOperationException>(() => Ensure.Condition(false, () => new InvalidOperationException()));
        }

        #region Supporting Types

        [Flags]
        private enum FlagsEnum
        {
            None = 0,
            One = 1,
            Two = 2,
            Three = 4,
            Four = 8
        }

        [Flags]
        private enum FlagsEnumNoNone
        {
            One = 1,
        }

        private enum ByteEnum : byte
        {
            One,
            Two,
            Three
        }

        [Flags]
        private enum ByteFlagsEnum : byte
        {
            None = 0,
            One = 1,
            Two = 2,
            Three = 4
        }

        #endregion
    }
}