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
        [XmlElement("abName")] public string abName;
        [XmlAttribute("assetName")] public string assetName;
        [XmlElement("dependABs")] public List<string> dependABs;

        /// <summary>
        /// 仅用于生成XML
        /// </summary>
        public AssetInfo() : this(string.Empty, string.Empty, new List<string>())
        {
        }

        public AssetInfo(string abName, string assetName, List<string> dependABs)
        {
            this.abName = abName;
            this.assetName = assetName;
            this.dependABs = dependABs;
        }

        public override string ToString()
        {
            return $"asset:{assetName} AB:{abName}";
        }
    }
}