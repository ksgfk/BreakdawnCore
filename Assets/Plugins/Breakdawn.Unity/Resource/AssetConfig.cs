using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Breakdawn.Unity
{
    [Serializable]
    public class AssetConfig
    {
        [XmlElement("assetList")] public List<AssetInfo> assetList;
    }

    /// <summary>
    /// 资源信息
    /// </summary>
    [Serializable]
    public class AssetInfo
    {
        [XmlElement("hash")] public string hash;
        [XmlElement("abName")] public string abName;
        [XmlAttribute("assetName")] public string assetName;
        [XmlElement("dependABs")] public List<string> dependABs;
    }
}