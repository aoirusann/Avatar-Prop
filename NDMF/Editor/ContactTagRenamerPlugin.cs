using System.Collections.Generic;
using System.Text;
using nadena.dev.ndmf;
using UnityEngine;
using VRC.SDK3.Dynamics.Contact.Components;
using Object = UnityEngine.Object;

[assembly: ExportsPlugin(typeof(Shinano.ContactTags.Editor.ContactTagRenamerPlugin))]

namespace Shinano.ContactTags.Editor
{
    /// <summary>
    /// At build time, for every <see cref="ContactTagRenamer"/> found on the avatar, appends a
    /// per-instance unique suffix to the contact collision tags listed on that component.
    /// Runs before VRCFury (NDMF runs at SDK callback order -11000, VRCFury at -10000), so the
    /// renamed tags are what ends up in the build.
    /// </summary>
    internal sealed class ContactTagRenamerPlugin : Plugin<ContactTagRenamerPlugin>
    {
        public override string QualifiedName => "shinano.contact-tag-renamer";
        public override string DisplayName => "Contact Tag Renamer";

        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming).Run("Rename custom contact tags per instance", ctx =>
            {
                var avatarRoot = ctx.AvatarRootObject;
                if (avatarRoot == null) return;

                var renamers = avatarRoot.GetComponentsInChildren<ContactTagRenamer>(true);
                foreach (var renamer in renamers)
                {
                    if (renamer == null) continue;

                    var targets = renamer.tags;
                    if (targets == null || targets.Count == 0) continue;

                    var suffix = ResolveSuffix(renamer);
                    if (string.IsNullOrEmpty(suffix)) continue;

                    foreach (var sender in renamer.GetComponentsInChildren<VRCContactSender>(true))
                        RewriteTags(sender.collisionTags, targets, suffix);
                    foreach (var receiver in renamer.GetComponentsInChildren<VRCContactReceiver>(true))
                        RewriteTags(receiver.collisionTags, targets, suffix);

                    Object.DestroyImmediate(renamer);
                }
            });
        }

        private static string ResolveSuffix(ContactTagRenamer renamer)
        {
            if (!string.IsNullOrEmpty(renamer.suffixOverride)) return renamer.suffixOverride;
            return "." + HashPath(renamer.transform);
        }

        private static string HashPath(Transform t)
        {
            var sb = new StringBuilder();
            var cur = t;
            while (cur != null)
            {
                sb.Insert(0, cur.name);
                sb.Insert(0, '/');
                cur = cur.parent;
            }

            // FNV-1a (32-bit)
            uint hash = 2166136261u;
            foreach (var c in sb.ToString())
            {
                hash ^= c;
                hash *= 16777619u;
            }

            return hash.ToString("x8");
        }

        private static void RewriteTags(List<string> collisionTags, List<string> targets, string suffix)
        {
            if (collisionTags == null) return;

            for (var i = 0; i < collisionTags.Count; i++)
            {
                var tag = collisionTags[i];
                if (string.IsNullOrEmpty(tag)) continue;

                // Pick the longest matching target so that a tag which is a prefixed extension of
                // another target is matched by the most specific one.
                string best = null;
                foreach (var target in targets)
                {
                    if (string.IsNullOrEmpty(target)) continue;
                    var matches = tag == target
                                  || (tag.Length > target.Length && tag.StartsWith(target) && tag[target.Length] == '.');
                    if (matches && (best == null || target.Length > best.Length)) best = target;
                }

                if (best != null) collisionTags[i] = best + suffix;
            }
        }
    }
}
