using FluentAssertions;
using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCaseSource(nameof(InvalidPrecisionCases))]
    public void Constructor_ShouldThrowArgumentException_WhenInvalidPrecision(int precision)
    {
        Action act = () => new NumberValidator(precision);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*precision must be*");
    }

    [TestCaseSource(nameof(InvalidScaleCases))]
    public void Constructor_ShouldThrowArgumentException_WhenInvalidScale(int precision, int scale)
    {
        Action act = () => new NumberValidator(precision, scale);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*scale must be*");
    }

    [TestCaseSource(nameof(ValidConstructorCases))]
    public void Constructor_ShouldNotThrow_WhenValidArguments(int precision, int scale, bool onlyPositive)
    {
        Action act = () => new NumberValidator(precision, scale, onlyPositive);

        act.Should().NotThrow();
    }

    [TestCaseSource(nameof(InvalidFormatCases))]
    public void IsValidNumber_ShouldReturnFalse_WhenInvalidFormat(string? value)
    {
        var validator = new NumberValidator(10, 5);

        validator.IsValidNumber(value!).Should().BeFalse();
    }

    [TestCaseSource(nameof(PrecisionExceededCases))]
    public void IsValidNumber_ShouldReturnFalse_WhenPrecisionExceeded(string value, int precision, int scale)
    {
        var validator = new NumberValidator(precision, scale);

        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCaseSource(nameof(ScaleExceededCases))]
    public void IsValidNumber_ShouldReturnFalse_WhenScaleExceeded(string value, int precision, int scale)
    {
        var validator = new NumberValidator(precision, scale);

        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCaseSource(nameof(NegativeNumberWhenOnlyPositiveCases))]
    public void IsValidNumber_ShouldReturnFalse_WhenNumberIsNegativeAndOnlyPositiveIsTrue(string value)
    {
        var validator = new NumberValidator(5, 2, true);

        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCaseSource(nameof(ValidNumberCases))]
    public void IsValidNumber_ShouldReturnTrue_WhenNumberIsValid(
        string value,
        int precision,
        int scale,
        bool onlyPositive)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        validator.IsValidNumber(value).Should().BeTrue();
    }

    private static IEnumerable<TestCaseData> InvalidPrecisionCases()
    {
        yield return new TestCaseData(0).SetName("{m}_PrecisionIsZero{a}"); // Собираем имя из названия теста + названия TestCase
        yield return new TestCaseData(-1).SetName("{m}_PrecisionIsNegative{a}"); // + аргументов TestCase
    }

    private static IEnumerable<TestCaseData> InvalidScaleCases()
    {
        yield return new TestCaseData(1, -1).SetName("{m}_ScaleIsNegative{a}");
        yield return new TestCaseData(1, 1).SetName("{m}_ScaleEqualsPrecision{a}");
        yield return new TestCaseData(1, 2).SetName("{m}_ScaleGreaterThanPrecision{a}");
    }

    private static IEnumerable<TestCaseData> ValidConstructorCases()
    {
        yield return new TestCaseData(1, 0, true).SetName("{m}_MinValidPrecisionAndZeroScale{a}");
        yield return new TestCaseData(3, 2, false).SetName("{m}_ValidPrecisionAndPositiveScale{a}");
    }

    private static IEnumerable<TestCaseData> InvalidFormatCases()
    {
        yield return new TestCaseData(null).SetName("{m}_ValueIsNull{a}");
        yield return new TestCaseData("").SetName("{m}_ValueIsEmpty{a}");
        yield return new TestCaseData("   ").SetName("{m}_ValueIsWhitespace{a}");
        yield return new TestCaseData(" 1.1").SetName("{m}_LeadingWhitespace{a}");
        yield return new TestCaseData("1.1 ").SetName("{m}_TrailingWhitespace{a}");
        yield return new TestCaseData("a.sd").SetName("{m}_FullLettersInsteadDigits{a}");
        yield return new TestCaseData("1.a").SetName("{m}_LettersInFractionalPart{a}");
        yield return new TestCaseData("a.1").SetName("{m}_LettersInIntegerPart{a}");
        yield return new TestCaseData("1.").SetName("{m}_MissingFractionalPartAfterSeparator{a}");
        yield return new TestCaseData(".1").SetName("{m}_MissingIntegerPartBeforeSeparator{a}");
        yield return new TestCaseData("1,.1").SetName("{m}_MultipleSeparators{a}");
        yield return new TestCaseData("++1").SetName("{m}_MultipleSigns{a}");
    }

    private static IEnumerable<TestCaseData> PrecisionExceededCases()
    {
        yield return new TestCaseData("00.00", 3, 2).SetName("{m}_DigitsCountExceedsPrecision{a}");
        yield return new TestCaseData("1234", 3, 0).SetName("{m}_IntegerLengthExceedsPrecision{a}");
        yield return new TestCaseData("+1.23", 3, 2).SetName("{m}_LengthWithSignExceedsPrecision{a}");
        yield return new TestCaseData("-1.23", 3, 2).SetName("{m}_LengthWithMinusSignExceedsPrecision{a}");
    }

    private static IEnumerable<TestCaseData> ScaleExceededCases()
    {
        yield return new TestCaseData("0.00", 5, 1).SetName("{m}_FractionalPartExceedsNonZeroScale{a}");
        yield return new TestCaseData("1.1", 5, 0).SetName("{m}_FractionalPartPresentWhenScaleIsZero{a}");
    }

    private static IEnumerable<TestCaseData> NegativeNumberWhenOnlyPositiveCases()
    {
        yield return new TestCaseData("-1").SetName("{m}_NegativeIntegerWhenOnlyPositive{a}");
        yield return new TestCaseData("-1.11").SetName("{m}_NegativeFractionWhenOnlyPositive{a}");
    }

    private static IEnumerable<TestCaseData> ValidNumberCases()
    {
        yield return new TestCaseData("0", 1, 0, true).SetName("{m}_SingleDigitZero{a}");
        yield return new TestCaseData("1.1", 5, 2, true).SetName("{m}_ValidNumberWithDotSeparator{a}");
        yield return new TestCaseData("1,1", 5, 2, true).SetName("{m}_ValidNumberWithCommaSeparator{a}");
        yield return new TestCaseData("+1.23", 4, 2, true).SetName("{m}_PositiveWithPlusSignMatchingExactPrecisionAndScale{a}");
        yield return new TestCaseData("-1.23", 4, 2, false).SetName("{m}_NegativeMatchingExactPrecisionAndScaleWhenAllowed{a}");
        yield return new TestCaseData("123", 3, 2, true).SetName("{m}_IntegerWithScaleGreaterThanZero{a}");
    }
}