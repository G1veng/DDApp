using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DDApp.Common
{
    public class PathHelper
    {
        public static string MakeRelativePath(string fullPath)
        {
            var res = string.Empty;
            var appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var startIndex = 0;

            if(appDir == null)
            {
                throw new Exception("Dir is null");
            }

            for(int i = 0; i < appDir.Length; i++)
            {
                if(appDir[i] != fullPath[i])
                {
                    startIndex = i;
                    break;
                }
            }

            for(int i = startIndex; i < fullPath.Length; i++)
            {
                res += fullPath[i];
            }

            return res;
        }

        /*public static string GetAbsolutePath(string relativePath)
        {
            var appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            int endIndex = 0;
            string temp = string.Empty;

            for(int i = 0; i < appDir?.Length; i++)
            {
                if(appDir[i] == '\\')
                {

                }
            }
        }

        private static bool ComaperToBinString(string compareTo)
        {
            var bin = "bin";
        }*/

        public static string GetGuidNameFile(string filePath)
        {
            var guidFile = string.Empty;
            int start = 0;

            for(int i = filePath.Length - 1; i >= 0; i--)
            {
                if(filePath[i] == '\\')
                {
                    start = i + 1;
                    break;
                }
            }

            if(start == 0)
            {
                throw new Exception("File wrong format");
            }

            for(int i = start; i < filePath.Length; i++)
            {
                guidFile += filePath[i];
            }

            return guidFile;
        }

        public static string GetDiskName(string filePath)
        {
            string diskNameFile = string.Empty;

            for(int i = 0; i < filePath.Length; i++)
            {
                if(filePath[i] == ':')
                {
                    diskNameFile += filePath[i];

                    return diskNameFile;
                }

                diskNameFile += filePath[i];
            }

            throw new Exception("File wrong format");
        }

        public static string AddSeparator(string firstPart, string secondPart)
        {
            return firstPart + '\\' + secondPart;
        }
    }
}
