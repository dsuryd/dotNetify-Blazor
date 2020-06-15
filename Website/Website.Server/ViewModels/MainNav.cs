using DotNetify;
using DotNetify.Elements;

namespace Website.Server
{
   public class MainNav : BaseVM
   {
      public const string PATH_BASE = "blazor";

      public MainNav()
      {
         AddProperty("NavMenu", new NavMenu(
            new NavMenuItem[]
            {
               new NavRoute("Overview", PATH_BASE + "/"),
               new NavGroup
               {
                  Label = "Basic Examples",
                  Routes = new NavRoute[]
                  {
                     new NavRoute("Counter", PATH_BASE + "/counter"),
                  },
                  IsExpanded = true
               },
               new NavGroup
               {
                  Label = "Examples with Elements",
                  Routes = new NavRoute[]
                  {
                     new NavRoute("Dashboard", PATH_BASE + "/dashboard"),
                     new NavRoute("Form", PATH_BASE + "/form"),
                  },
                  IsExpanded = true
               },
               new NavGroup
               {
                  Label = "API Reference",
                  Routes = new NavRoute[]
                  {
                     new NavRoute (".NET Client", PATH_BASE + "/dotnetclient"),
                     new NavRoute ("CRUD", PATH_BASE + "/crud"),
                     new NavRoute ("Dependency Injection", PATH_BASE + "/di"),
                     new NavRoute ("Filter", PATH_BASE + "/filter"),
                     new NavRoute ("Middleware", PATH_BASE + "/middleware"),
                     new NavRoute ("Multicast", PATH_BASE + "/multicast")
                  },
                  IsExpanded = false
               }
            })
         );
      }
   }
}