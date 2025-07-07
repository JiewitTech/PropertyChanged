using System.Xml.Linq;

public class OnlyAddINotifyPropertyChangedInterfaceAttributeConfigTests
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged OnlyAddINotifyPropertyChangedInterfaceAttribute='false'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveOnlyAddINotifyPropertyChangedInterfaceAttributeConfig();
        Assert.False(weaver.OnlyAddINotifyPropertyChangedInterfaceAttribute);
    }

    [Fact]
    public void False0()
    {
        var xElement = XElement.Parse("<PropertyChanged OnlyAddINotifyPropertyChangedInterfaceAttribute='0'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveOnlyAddINotifyPropertyChangedInterfaceAttributeConfig();
        Assert.False(weaver.OnlyAddINotifyPropertyChangedInterfaceAttribute);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged OnlyAddINotifyPropertyChangedInterfaceAttribute='True'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveOnlyAddINotifyPropertyChangedInterfaceAttributeConfig();
        Assert.True(weaver.OnlyAddINotifyPropertyChangedInterfaceAttribute);
    }

    [Fact]
    public void True1()
    {
        var xElement = XElement.Parse("<PropertyChanged OnlyAddINotifyPropertyChangedInterfaceAttribute='1'/>");
        var weaver = new ModuleWeaver { Config = xElement };
        weaver.ResolveOnlyAddINotifyPropertyChangedInterfaceAttributeConfig();
        Assert.True(weaver.OnlyAddINotifyPropertyChangedInterfaceAttribute);
    }

    [Fact]
    public void Default()
    {
        var weaver = new ModuleWeaver();
        weaver.ResolveOnlyAddINotifyPropertyChangedInterfaceAttributeConfig();
        Assert.False(weaver.OnlyAddINotifyPropertyChangedInterfaceAttribute);
    }
}