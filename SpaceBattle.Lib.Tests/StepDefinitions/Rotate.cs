using TechTalk.SpecFlow;

namespace SpaceBattle.Lib.Tests;

[Binding]
public class Rotate
{
    private readonly ScenarioContext _scenarioContext;

    public Rotate(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }
    [Given(@"имеет мгновенную угловую скорость (.*) град")]
    public static void ДопустимИмеетМгновеннуюУгловуюСкоростьГрад(int p0)
    {
        //_scenarioContext.Pending();
    }

    [When(@"происходит вращение вокруг собственной оси")]
    public static void КогдаПроисходитВращениеВокругСобственнойОси()
    {
        //_scenarioContext.Pending();
    }

    [Then(@"угол наклона космического корабля к оси OX составляет (.*) град")]
    public static void ТоУголНаклонаКосмическогоКорабляКОсиOXСоставляетГрад(int p0)
    {
        //_scenarioContext.Pending();
    }

    [Given(@"космический корабль имеет угол наклона (.*) град к оси OX")]
    public static void ДопустимКосмическийКорабльИмеетУголНаклонаГрадКОсиOX(int p0)
    {
        //_scenarioContext.Pending();
    }
}
