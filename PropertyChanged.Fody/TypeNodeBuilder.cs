using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Fody;
using Mono.Cecil;

public partial class ModuleWeaver
{
    List<TypeDefinition> allClasses;
    public List<TypeNode> Nodes;
    public List<TypeNode> NotifyNodes;

    public void BuildTypeNodes()
    {
        // setup a filter delegate to apply the namespace filters
        Func<TypeDefinition, bool> extraFilter =
            t => !NamespaceFilters.Any() || NamespaceFilters.Any(filter => Regex.IsMatch(t.FullName, filter));

        allClasses = ModuleDefinition
            .GetTypes()
            .Where(_ => _.IsClass &&
                        _.BaseType != null)
            .Where(extraFilter)
            .ToList();
        Nodes = new();
        NotifyNodes = new();
        while (allClasses.FirstOrDefault() is { } typeDefinition)
        {
            AddClass(typeDefinition);
        }

        //已实现INotifyPropertyChanged接口的类 且 没有加AddINotifyPropertyChangedInterfaceAttribute特性的类
        PopulateINotifyNodes(Nodes);
        foreach (var notifyNode in NotifyNodes)
        {
            Nodes.Remove(notifyNode);
        }
        //加AddINotifyPropertyChangedInterfaceAttribute特性的类
        PopulateInjectedINotifyNodes(Nodes);
    }

    void PopulateINotifyNodes(List<TypeNode> typeNodes)
    {
        foreach (var node in typeNodes)
        {
            if (HierarchyImplementsINotify(node.TypeDefinition) && !HasNotifyPropertyChangedAttribute(node.TypeDefinition))
            {
                if (!OnlyAddINotifyPropertyChangedInterfaceAttribute)
                {
                    NotifyNodes.Add(node);
                    continue;
                }
            }
            PopulateINotifyNodes(node.Nodes);
        }
    }

    void PopulateInjectedINotifyNodes(List<TypeNode> typeNodes)
    {
        foreach (var node in typeNodes)
        {
            if (HasNotifyPropertyChangedAttribute(node.TypeDefinition))
            {
                var hasImplementINotifyPropertyChanged = false;
                if (HierarchyImplementsINotify(node.TypeDefinition) || node.TypeDefinition.GetPropertyChangedAddMethods().Any())
                    hasImplementINotifyPropertyChanged = true;

                if (!hasImplementINotifyPropertyChanged)
                    InjectINotifyPropertyChangedInterface(node.TypeDefinition);
                NotifyNodes.Add(node);
                continue;
            }
            PopulateInjectedINotifyNodes(node.Nodes);
        }
    }

    static bool HasNotifyPropertyChangedAttribute(TypeDefinition typeDefinition)
    {
        return typeDefinition.CustomAttributes.ContainsAttribute("PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute");
    }

    TypeNode AddClass(TypeDefinition typeDefinition)
    {
        allClasses.Remove(typeDefinition);
        var typeNode = new TypeNode
        {
            TypeDefinition = typeDefinition
        };
        if (typeDefinition.BaseType.Scope.Name != ModuleDefinition.Name)
        {
            Nodes.Add(typeNode);
        }
        else
        {
            var baseType = Resolve(typeDefinition.BaseType);
            var parentNode = FindClassNode(baseType, Nodes);
            if (parentNode == null)
            {
                parentNode = AddClass(baseType);
            }
            parentNode.Nodes.Add(typeNode);
        }
        return typeNode;
    }

    static TypeNode FindClassNode(TypeDefinition type, IEnumerable<TypeNode> typeNode)
    {
        foreach (var node in typeNode)
        {
            if (type == node.TypeDefinition)
            {
                return node;
            }
            var findNode = FindClassNode(type, node.Nodes);
            if (findNode != null)
            {
                return findNode;
            }
        }
        return null;
    }
}