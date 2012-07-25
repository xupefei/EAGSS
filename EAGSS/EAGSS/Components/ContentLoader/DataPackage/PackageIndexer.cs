using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace EAGSS.DataPackage
{
    internal class PackageIndexer
    {
        internal Dictionary<string, StructDescription.ContentInfo> DictContents =
            new Dictionary<string, StructDescription.ContentInfo>(StringComparer.OrdinalIgnoreCase);

        internal PackageIndexer(string rootDirectory)
        {
            string contentPath = rootDirectory;

            if (!Directory.Exists(contentPath))
                return;

            string[] pkgNames = Directory.GetFiles(contentPath, GameSettings.DataPackageParameter,
                                                   SearchOption.AllDirectories);

            foreach (string pkgName in pkgNames)
            {
                Dictionary<string, StructDescription.ContentInfo> dictSegments;
                using (var br = new BinaryReader(new FileStream(pkgName, FileMode.Open)))
                {
                    var pInfo = new StructDescription.PackageInfo();
                    pInfo =
                        BytesHelper.BytesToStruct<StructDescription.PackageInfo>(
                            br.ReadBytes(Marshal.SizeOf(pInfo)));

                    dictSegments = GetOnePackageIndex(pkgName, pInfo, br);
                }

                foreach (var pair in dictSegments)
                {
                    if (DictContents.Keys.Contains(pair.Key))
                        DictContents[pair.Key] = pair.Value;
                    else
                        DictContents.Add(pair.Key, pair.Value);
                }
            }
        }

        private Dictionary<string, StructDescription.ContentInfo>
            GetOnePackageIndex(string pkgName, StructDescription.PackageInfo pInfo, BinaryReader br)
        {
            var dictEntryInfo =
                new Dictionary<string, StructDescription.ContentInfo>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < pInfo.EntryCount; i++)
            {
                var eInfo =
                    BytesHelper.BytesToStruct<StructDescription.EntryInfo>(
                        br.ReadBytes(Marshal.SizeOf(new StructDescription.EntryInfo())));

                var cInfo = new StructDescription.ContentInfo(pkgName, eInfo.Start, eInfo.Length,
                                                              eInfo.RealLength);

                dictEntryInfo.Add(eInfo.Name, cInfo);
            }

            return dictEntryInfo;
        }
    }
}