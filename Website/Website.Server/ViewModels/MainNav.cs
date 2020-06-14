using DotNetify;
using DotNetify.Elements;

namespace Website.Server
{
   public class MainNav : BaseVM
   {
      public MainNav()
      {
         AddProperty("NavMenu", new NavMenu(
            new NavMenuItem[]
            {
               new NavRoute("Overview", ""),
               new NavGroup
               {
                  Label = "Basic Examples",
                  Routes = new NavRoute[]
                  {
                     new NavRoute("Counter", "counter"),
                  },
                  IsExpanded = true
               },
               new NavGroup
               {
                  Label = "Examples with Elements",
                  Routes = new NavRoute[]
                  {
                     new NavRoute("Dashboard", "dashboard"),
                     new NavRoute("Form", "form"),
                  },
                  IsExpanded = true
               },
               new NavGroup
               {
                  Label = "API Reference",
                  Routes = new NavRoute[]
                  {
                     new NavRoute (".NET Client", "dotnetclient"),
                     new NavRoute ("CRUD", "crud"),
                     new NavRoute ("Dependency Injection", "di"),
                     new NavRoute ("Filter", "filter"),
                     new NavRoute ("Middleware", "middleware"),
                     new NavRoute ("Multicast","multicast")
                  },
                  IsExpanded = true
               }
            })
         );
      }
   }
}