using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.Core.AspNetSample.Models;
using DbLocalizationProvider.Sync;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyProject;

namespace DbLocalizationProvider.Core.AspNetSample.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class HomeController : Controller
    {
        private readonly ILocalizationProvider _provider;
        private readonly ILogger _logger;
        private readonly ISynchronizer _synchronizer;
        private readonly IStringLocalizer<Resources.SampleResources> _localizer;
        private readonly ICommandExecutor _executor;
        private readonly IStringLocalizer _simpleLocalizer;

        public HomeController(
            ILocalizationProvider provider,
            IOptions<MvcOptions> options,
            ILogger<HomeController> logger,
            ISynchronizer synchronizer,
            IStringLocalizer<Resources.SampleResources> localizer,
            ICommandExecutor executor)
        {
            _provider = provider;
            _logger = logger;
            _synchronizer = synchronizer;
            _localizer = localizer;
            _executor = executor;
            _simpleLocalizer = localizer;

            var asms = GetAssemblies().Where(a => a.FullName.Contains("DbLocalizationProvider"));
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach(var reference in asm.GetReferencedAssemblies())
                    if(!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }
            }
            while(stack.Count > 0);
        }

        public IActionResult Index()
        {
            // register manually some of the resources
            _synchronizer.RegisterManually(new List<ManualResource>
            {
                new ManualResource("Manual.Resource.1", "English translation", new CultureInfo("en"))
            });

            var u = ControllerContext.HttpContext.User?.Identity;

            var zz = _provider.GetStringByCulture(() => ResourcesForFallback.OnlyInInvariant, new CultureInfo("sv"));

            var zzz = _localizer.GetString(r => r.PageHeader2);
            var zzzz = _localizer.GetStringByCulture(r => r.PageHeader2, new CultureInfo("fr-FR"));

            var xxx = _simpleLocalizer.GetString(() => Resources.SampleResources.PageHeader);
            var xxxx = _simpleLocalizer.GetStringByCulture(() => Resources.SampleResources.PageHeader, new CultureInfo("fr-FR"));

            ViewData["TestString"] = _provider.GetString(() => Resources.Shared.CommonResources.Yes);

            return View();
        }

        public IActionResult Routes()
        {
            return View();
        }

        public IActionResult ForeignModel()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                                    new CookieOptions
                                    {
                                        Expires = DateTimeOffset.UtcNow.AddYears(1)
                                    }
                                   );

            return LocalRedirect(returnUrl);
        }
    }
}
