using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;

public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // исключаем из проверки поле Id типа Person
        actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
            .Excluding(info => info.Name == nameof(Person.Id) && info.DeclaringType == typeof(Person)));
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода? 
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    // 1. После добавления новых полей в Person, надо будет переписывать метод
    //    Во fluentAssertions этого не требуется, так как он проверяет все поля(за исключением исключенных) автоматически
    // 2. Если в Person будут циклические ссылки, может упасть с StackOverflowException
    //    Во fluentAssertions этого не произойдет, так как он умеет работать с циклическими ссылками(при ошибке сообщит о цикле/вообще проигнорирует его(при IgnoringCyclicReferences))
    // 3. Также не понятно, что именно не совпало, если тест упадет.
    //    Во fluentAssertions будет подробный вывод, что именно не совпало

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}