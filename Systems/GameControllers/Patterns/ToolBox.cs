using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

public class ToolBox : Singleton<ToolBox>
{
    [SerializeField] Dictionary<string, Component> handlers = new Dictionary<string, Component>();

    public void AddHandler(Component handler, string tag)
    {
        if (handlers.ContainsKey(tag))
            return;
        else
            handlers.Add(tag, handler);
    }

    public Component GetGlobalHandler(string tag)
    {
        Component component;
        if (handlers.TryGetValue(tag, out component))
            return component;
        else
            Debug.LogWarning($"[Toolbox] Global component ID {tag} doesn't exist!");
        return null;
    }
}
