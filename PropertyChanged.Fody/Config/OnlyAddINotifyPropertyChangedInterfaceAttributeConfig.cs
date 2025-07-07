using System.Linq;
using System.Xml;

public partial class ModuleWeaver
{
    public bool OnlyAddINotifyPropertyChangedInterfaceAttribute = false;

    public void ResolveOnlyAddINotifyPropertyChangedInterfaceAttributeConfig()
    {
        var value = Config?.Attributes("OnlyAddINotifyPropertyChangedInterfaceAttribute")
            .Select(a => a.Value)
            .SingleOrDefault();
        if (value != null)
        {
            OnlyAddINotifyPropertyChangedInterfaceAttribute = XmlConvert.ToBoolean(value.ToLowerInvariant());
        }
    }
}