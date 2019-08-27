using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Breakdawn.Unity
{
    [Serializable]
    public class AssetBundleConfig
    {
        [XmlElement("abList")] public List<AssetBundleBase> abList;
        [XmlElement("path")] public string path;
    }

    /// <summary>
    /// AB包基础信息
    /// </summary>
    [Serializable]
    public class AssetBundleBase
    {
        [XmlElement("path")] public string path;
        [XmlElement("crc")] public uint crc;
        [XmlElement("hash")] public string hash;
        [XmlElement("name")] public string name;
        [XmlAttribute("assetName")] public string assetName;
        [XmlElement("dependence")] public List<string> dependence;
    }
}