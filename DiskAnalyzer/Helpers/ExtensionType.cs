using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskAnalyzer.Helpers
{
    static class ExtensionType
    {
        //public static Hashtable hashTable = new Hashtable() {
        //    { ".aif", 1 },
        //    { ".cda", 1 },
        //    { ".mid", 1 },
        //    { ".mp3", 1 },
        //    { ".mpa", 1 }
        //};

        public static Dictionary<string, int> Extensions = new Dictionary<string, int>
        {
            { ".exe", 6 }
            //{ "aif", 1 },
            //{ "cda", 1 },
            //{ "mid", 1 },
            //{ "mp3", 1 },
            //{ "mpa", 1 },
            //{ "ogg", 1 },
            //{ "wav", 1 },
            //{ "wma", 1 },
            //{ "wpl", 1 },
            //{ "7z", 2 } ,
            //{ "arj", 2 },
            //{ "deb", 2 },
            //{ "pkg", 2 },
            //{ "rar", 2 },
            //{ "rpm", 2 },
            //{ "tar", 2 },
            //{ "z", 2 },
            //{ "zip", 2 },
            //{ "dmg", 3 },
            //{ "iso", 3 },
            //{ "toast", 3 },
            //{ "vcd", 3 },
            //{ "csv", 4 },
            //{ "dat", 4 },
            //{ "db", 4 },
            //{ "log", 4 },
            //{ "mdb", 4 },
            //{ "sav", 4 },
            //{ "sql", 4 },
            //{ "xml", 4 },
            //{ "email", 5 },
            //{ "eml", 5 },
            //{ "emlx", 5 },
            //{ "msg", 5 },
            //{ "oft", 5 },
            //{ "ost", 5 },
            //{ "pst", 5 },
            //{ "vcf", 5 },
            //{ "apk", 6 },
            //{ "bat", 6 },
            //{ "bin", 6 },
            //{ "com", 6 },
            //{ "exe", 6 },
            //{ "gadget", 6 },
            //{ "jar", 6 },
            //{ "wsf", 6 },
            //{ "fnt", 7 },
            //{ "fon", 7 },
            //{ "otf", 7 },
            //{ "ttf", 7 },
            //{ "ai", 8 },
            //{ "bmp", 8 },
            //{ "gif", 8 },
            //{ "ico", 8 },
            //{ "jpeg", 8 },
            //{ "jpg", 8 },
            //{ "png", 8 },
            //{ "ps", 8 },
            //{ "psd", 8 },
            //{ "svg", 8 },
            //{ "tif", 8 },
            //{ "asp", 9 },
            //{ "aspx", 9 },
            //{ "cer", 9 },
            //{ "cfm", 9 },
            //{ "css", 9 },
            //{ "htm", 9 },
            //{ "js", 9 },
            //{ "jsp", 9 },
            //{ "part", 9 },
            //{ "rss", 9 },
            //{ "xhtml", 9 },
            //{ "key", 10 },
            //{ "odp", 10 },
            //{ "pps", 10 },
            //{ "ppt", 10 },
            //{ "pptx", 10 },
            //{ "c", 11 },
            //{ "cgi", 11 },
            //{ "pl", 11 },
            //{ "class", 11 },
            //{ "cpp", 11 },
            //{ "cs", 11 },
            //{ "h", 11 },
            //{ "java", 11 },
            //{ "php", 11 },
            //{ "py", 11 },
            //{ "sh", 11 },
            //{ "swift", 11 },
            //{ "vb", 11 },
            //{ "ods", 12 },
            //{ "xls", 12 },
            //{ "xlsm", 12 },
            //{ "xlsx", 12 },
            //{ "bak", 13 },
            //{ "cab", 13 },
            //{ "cfg", 13 },
            //{ "cpl", 13 },
            //{ "cur", 13 },
            //{ "dll", 13 },
            //{ "dmp", 13 },
            //{ "drv", 13 },
            //{ "icns", 13 },
            //{ "ini", 13 },
            //{ "lnk", 13 },
            //{ "msi", 13 },
            //{ "sys", 13 },
            //{ "tmp", 13 },
            //{ "3g2", 14 },
            //{ "3gp", 14 },
            //{ "avi", 14 },
            //{ "flv", 14 },
            //{ "h264", 14 },
            //{ "m4v", 14 },
            //{ "mkv", 14 },
            //{ "mov", 14 },
            //{ "mp4", 14 },
            //{ "mpg", 14 },
            //{ "mpeg", 14 },
            //{ "rm", 14 },
            //{ "swf", 14 },
            //{ "vob", 14 },
            //{ "wmv", 14 },
            //{ "doc", 15 },
            //{ "docx", 15 },
            //{ "odt", 15 },
            //{ "pdf", 15 },
            //{ "rtf", 15 },
            //{ "tex", 15 },
            //{ "txt", 15 },
            //{ "wpd", 15 }
        };


        public static Dictionary<int, string> ExtensionTypes = new Dictionary<int, string>
        {
            {1, "Audio" },
            {2, "Compressed" },
            {3, "Disc and media" },
            {4, "Data and database" },
            {5, "E-mail" },
            {6, "Executable" },
            {7, "Font" },
            {8, "Image file formats" },
            {9, "Internet-related" },
            {10, "Presentation" },
            {11, "Programming files" },
            {12, "Spreadsheet"  },
            {13, "System related" },
            {14, "Video" },
            {15, "Word processor and text" }
        };
        public static string GetDescription(int extensionId)
        {
            try
            {
                return ExtensionTypes[extensionId];
            }
            catch
            {
                return string.Empty;
            };
        }

        public static int GetId(string extension)
        {
            try { 
                return Extensions[extension];
            }
            catch
            {
                return 0;
            }
        }
    }
}
