using System.IO;
using System.Reflection;
using UnityEngine;

namespace ABLoader;

public static class Loader
{
    public static AssetBundle LoadBundle(string name) {
        string location = Assembly.GetExecutingAssembly().Location;
        return AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(location), name));
    }
}