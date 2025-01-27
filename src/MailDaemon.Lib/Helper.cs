using System.IO;
using System.Net.Mime;

namespace MailDaemon.Core
{
    public class Helper
    {
        public static string GetMediaType(string fileName)
        {
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                    return MediaTypeNames.Image.Jpeg;
                case ".gif":
                    return MediaTypeNames.Image.Gif;
                case ".tiff":
                    return MediaTypeNames.Image.Tiff;
                case ".pdf":
                    return MediaTypeNames.Application.Pdf;
                case ".zip":
                    return MediaTypeNames.Application.Zip;
                case ".rtf":
                    return MediaTypeNames.Application.Rtf;
                case ".txt":
                    return MediaTypeNames.Text.Plain;
                case ".html":
                    return MediaTypeNames.Text.Html;
                case ".xml":
                    return MediaTypeNames.Text.Xml;
                default:
                    return MediaTypeNames.Application.Octet;
            }
        }

        //public static string GetMailProfileFullPath(SettingsInfo settingsInfo, string fileName)
        //{

        //}

        public static string GetMailBodyTemplateFullPath(SettingsInfo settingsInfo, string fileName)
        {
            if (fileName.StartsWith(".\\"))
                return Path.Combine(settingsInfo.AppDirectory, fileName.Replace(".\\", ""));
            
            return Path.Combine(settingsInfo.AppDirectory, settingsInfo.MailProfilesDirectory, fileName);
        }
    }
}
