using System.Collections.Generic;
using UnityEngine;

namespace Shinano.ContactTags
{
    /// <summary>
    /// Marker + config component. Drop it on the root of a prop:
    /// at build time the NDMF pass renames every contact collision tag in this subtree
    /// that equals one of <see cref="tags"/>, appending a per-instance unique suffix.
    /// This keeps multiple instances of the same prefab from cross-triggering each other
    /// through shared custom contact tags.
    /// </summary>
    [AddComponentMenu("Shinano/Contact Tag Renamer")]
    public sealed class ContactTagRenamer : MonoBehaviour
    {
        [Tooltip("碰撞标签名列表。子树内名称等于其中任意一项的 Contact 标签，会在构建时被加上唯一后缀。")]
        public List<string> tags = new List<string>();

        [Tooltip("留空则按本对象的层级路径自动生成唯一后缀；填写则直接使用该字符串。")]
        public string suffixOverride = "";
    }
}
