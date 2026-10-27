using FluentAssertions;
using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(0, Description = "PrecisionIsZero")]
    [TestCase(-1, Description = "PrecisionIsNegative")]
    public void Constructor_ShouldThrowArgumentException_WhenInvalidPrecision(int precision)
    {
        Action act = () => new NumberValidator(precision);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*precision must be*");
    }

    [TestCase(1, -1, Description = "ScaleIsNegative")]
    [TestCase(1, 1, Description = "ScaleEqualsPrecision")]
    [TestCase(1, 2, Description = "ScaleGreaterThanPrecision")]
    public void Constructor_ShouldThrowArgumentException_WhenInvalidScale(int precision, int scale)
    {
        Action act = () => new NumberValidator(precision, scale);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*scale must be*");
    }

    [TestCase(1, 0, true, Description = "MinValidPrecisionAndZeroScale")]
    [TestCase(3, 2, false, Description = "ValidPrecisionAndPositiveScale")]
    public void Constructor_ShouldNotThrow_WhenValidArguments(int precision, int scale, bool onlyPositive)
    {
        Action act = () => new NumberValidator(precision, scale, onlyPositive);

        act.Should().NotThrow();
    }

    [TestCase(null, Description = "ValueIsNull")]
    [TestCase("", Description = "ValueIsEmpty")]
    [TestCase("   ", Description = "ValueIsWhitespace")]
    [TestCase(" 1.1", Description = "LeadingWhitespace")]
    [TestCase("1.1 ", Description = "TrailingWhitespace")]
    [TestCase("a.sd", Description = "FullLettersInsteadDigits")]
    [TestCase("1.a", Description = "LettersInFractionalPart")]
    [TestCase("a.1", Description = "LettersInIntegerPart")]
    [TestCase("1.", Description = "MissingFractionalPartAfterSeparator")]
    [TestCase(".1", Description = "MissingIntegerPartBeforeSeparator")]
    [TestCase("1,.1", Description = "MultipleSeparators")]
    [TestCase("++1", Description = "MultipleSigns")]
    public void IsValidNumber_ShouldReturnFalse_WhenInvalidFormat(string? value)
    {
        var validator = new NumberValidator(10, 5);

        validator.IsValidNumber(value!).Should().BeFalse();
    }

    [TestCase("00.00", 3, 2, Description = "DigitsCountExceedsPrecision")]
    [TestCase("1234", 3, 0, Description = "IntegerLengthExceedsPrecision")]
    [TestCase("+1.23", 3, 2, Description = "LengthWithSignExceedsPrecision")]
    [TestCase("-1.23", 3, 2, Description = "LengthWithMinusSignExceedsPrecision")]
    public void IsValidNumber_ShouldReturnFalse_WhenPrecisionExceeded(string value, int precision, int scale)
    {
        var validator = new NumberValidator(precision, scale);

        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase("0.00", 5, 1, Description = "FractionalPartExceedsNonZeroScale")]
    [TestCase("1.1", 5, 0, Description = "FractionalPartPresentWhenScaleIsZero")]
    public void IsValidNumber_ShouldReturnFalse_WhenScaleExceeded(string value, int precision, int scale)
    {
        var validator = new NumberValidator(precision, scale);

        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase("-1", Description = "NegativeIntegerWhenOnlyPositive")]
    [TestCase("-1.11", Description = "NegativeFractionWhenOnlyPositive")]
    public void IsValidNumber_ShouldReturnFalse_WhenNumberIsNegativeAndOnlyPositiveIsTrue(string value)
    {
        var validator = new NumberValidator(5, 2, true);

        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase("0", 1, 0, true, Description = "SingleDigitZero")]
    [TestCase("1.1", 5, 2, true, Description = "ValidNumberWithDotSeparator")]
    [TestCase("1,1", 5, 2, true, Description = "ValidNumberWithCommaSeparator")]
    [TestCase("+1.23", 4, 2, true, Description = "PositiveWithPlusSignMatchingExactPrecisionAndScale")]
    [TestCase("-1.23", 4, 2, false, Description = "NegativeMatchingExactPrecisionAndScaleWhenAllowed")]
    [TestCase("123", 3, 2, true, Description = "IntegerWithScaleGreaterThanZero")]
    public void IsValidNumber_ShouldReturnTrue_WhenNumberIsValid(
        string value,
        int precision,
        int scale,
        bool onlyPositive)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        validator.IsValidNumber(value).Should().BeTrue();
    }
}