using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Optimization;

namespace Unicorn.Mvc
{
    public static class BundleHelper
    {
        //[Conditional("DEBUG")] // remove this attribute to validate bundles in production too
        private static void CheckExistence(string virtualPath)
        {
            int i = virtualPath.LastIndexOf('/');
            string path = HostingEnvironment.MapPath(virtualPath.Substring(0, i));
            string fileName = virtualPath.Substring(i + 1);

            bool found = Directory.Exists(path);

            if (found)
            {
                if (fileName.Contains("{version}"))
                {
                    var re = new Regex(fileName.Replace(".", @"\.").Replace("{version}", @"(\d+(?:\.\d+){1,3})"));
                    fileName = fileName.Replace("{version}", "*");
                    found = Directory.EnumerateFiles(path, fileName).Where(file => re.IsMatch(file)).FirstOrDefault() != null;
                }
                else // fileName may contain '*'
                    found = Directory.EnumerateFiles(path, fileName).FirstOrDefault() != null;
            }

            if (!found)
                throw new ApplicationException(String.Format("Bundle resource '{0}' not found", virtualPath));
        }

        public static Bundle IncludeExisting(this Bundle bundle, params string[] virtualPaths)
        {
            foreach (string virtualPath in virtualPaths)
                CheckExistence(virtualPath);

            return bundle.Include(virtualPaths);
        }

        public static Bundle IncludeExistingStyle(this Bundle bundle, params string[] virtualPaths)
        {
            return IncludeExisting(bundle, new CssRewriteUrlTransformWrapper(), virtualPaths);
        }
        public static Bundle IncludeExisting(this Bundle bundle, IItemTransform transform, params string[] virtualPaths)
        {
            foreach (string virtualPath in virtualPaths)
                CheckExistence(virtualPath);
            foreach (string virtualPath in virtualPaths)
                bundle.Include(virtualPath, transform);
            return bundle;
        }
        public static Bundle IncludeExisting(this Bundle bundle, string virtualPath, params IItemTransform[] transforms)
        {
            CheckExistence(virtualPath);
            return bundle.Include(virtualPath, transforms);
        }
    }
}
