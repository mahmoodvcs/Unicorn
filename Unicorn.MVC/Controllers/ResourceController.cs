using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Unicorn.Mvc.Controllers
{
    public class Unicorn_ResourceController : Controller
    {
        [OutputCache(VaryByParam = "*", Duration = 3600 * 24 * 30)]
        public FileResult Index(string r, string a)
        {
            Assembly ase;
            if (a == null)
                ase = Assembly.GetExecutingAssembly();
            else
            {
                ase = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(aa => aa.GetName().Name == a);
                if (ase == null)
                {
                    ase = Assembly.GetExecutingAssembly();
                    string codeBase = ase.Location ?? ase.CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    path = Path.Combine(Path.GetDirectoryName(path), a + ".dll");
                    ase = Assembly.LoadFile(path);
                }
            }
            if (ase != null)
            {
                var str = ase.GetManifestResourceStream(r);
                if(str == null)
                {
                    ControllerContext.HttpContext.Response.StatusCode = 404;
                    return null;
                }
                return File(str, Unicorn.Web.WebUtility.GetMimeType(Path.GetExtension(r)));
            }
            ControllerContext.HttpContext.Response.StatusCode = 404;
            return null;
        }
    }
}
