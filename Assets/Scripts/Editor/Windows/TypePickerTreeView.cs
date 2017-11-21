using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

public class TypePickerTreeView : TreeView
{
    private List<Type> items = new List<Type>();
    public Action<int> OnItemDoubleClicked;
    public Action<IList<int>> OnSelectionChanged;

    public static Dictionary<Assembly, Type[]> allTypes;

    public TypePickerTreeView(TreeViewState state) : base(state)
    {
        Reload();
    }

    public TypePickerTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
    {
        Reload();
    }

    protected override void SelectionChanged(IList<int> selectedIds)
    {
        base.SelectionChanged(selectedIds);
        if (OnSelectionChanged != null)
            OnSelectionChanged.Invoke(selectedIds);
    }

    protected override void DoubleClickedItem(int id)
    {
        base.DoubleClickedItem(id);
        if (OnItemDoubleClicked != null)
            OnItemDoubleClicked.Invoke(id);
    }

    protected override TreeViewItem BuildRoot()
    {
        if (allTypes == null)
            PopulateTypes();

        TreeViewItem root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
        int id = 0;
        items.Add(null);
        
        foreach (var keyValuePair in allTypes)
        {
            TreeViewItem parent = root;
            TreeViewItem @new;
            TreeViewItem @newRoot = new TreeViewItem(id++, parent.depth + 1, keyValuePair.Key.GetName().Name);
            parent.AddChild(@newRoot);
            parent = @newRoot;

            for (int i = 0; i < keyValuePair.Value.Length; i++)
            {
                parent = newRoot;
                Type type = keyValuePair.Value[i];
                string name = type.ToString();
                string[] split = name.Split('.');
                for (int index = 0; index < split.Length; index++)
                {
                    string s = split[index];
                    if (parent.children != null)
                    {
                        TreeViewItem item = parent.children.FirstOrDefault(x => x.displayName == s);
                        if (item == null)
                        {
                            if (index == split.Length - 1)
                                items.Add(type);
                            else
                                items.Add(null);
                            
                            @new = new TreeViewItem(id++, parent.depth + 1, s);
                            parent.AddChild(@new);
                            parent = @new;
                        }
                        else
                        {
                            parent = item;
                        }
                    }
                    else
                    {
                        if (index == split.Length - 1)
                            items.Add(type);
                        else
                            items.Add(null);
                        
                        @new = new TreeViewItem(id++, parent.depth + 1, s);
                        parent.AddChild(@new);
                        parent = @new;
                    }
                }
            }
        }

        return root;
    }

    public static void PopulateTypes()
    {
        allTypes = new Dictionary<Assembly, Type[]>();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            allTypes.Add(assembly, assembly.GetTypes());
        }
    }

    public Type GetItemById(int id)
    {
        if (items.Count > id)
            return items[id];

        return null;
    }

    public TreeViewItem Find(TreeViewItem parent, string path)
    {
        if (parent.displayName == path.Split('/').Last())
        {
            return parent;
        }

        foreach (TreeViewItem treeViewItem in parent.children)
        {
            var item = Find(treeViewItem, path);
            if (item != null)
                return item;
        }

        return null;
    }
}