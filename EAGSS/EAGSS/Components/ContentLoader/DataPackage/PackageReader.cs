using System.IO;

namespace EAGSS.DataPackage
{
    internal class PackageReader
    {
        internal static MemoryStream GetContent(StructDescription.ContentInfo cInfo)
        {
            byte[] bs;
            using (var br = new BinaryReader(new FileStream(cInfo.Path, FileMode.Open)))
            {
                br.BaseStream.Position = cInfo.Offset;

                bs = AESEncryptionAlgorithm.AESDecrypt(br.ReadBytes(cInfo.Length));

                //remove padding 0x00
                bs = BytesHelper.CopyBlock(bs, 0, cInfo.RealLength);
            }

            return new MemoryStream(bs);
        }
    }
}