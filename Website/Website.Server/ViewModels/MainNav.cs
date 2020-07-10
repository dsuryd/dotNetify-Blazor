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
               new NavRoute("Basics", PATH_BASE + "/basics"),
               new NavRoute("Data Flow Pattern", PATH_BASE + "/dataflow"),
               new NavRoute("Reactive", PATH_BASE + "/reactive"),
               new NavRoute("Get Started", PATH_BASE + "/getstarted"),
               new NavGroup
               {
                  Label = "Examples",
                  Routes = new NavRoute[]
                  {
                     new NavRoute("Hello World", PATH_BASE + "/helloworld"),
                     new NavRoute("Control Types", PATH_BASE + "/controltypes"),
                     new NavRoute("Simple List", PATH_BASE + "/simplelist"),
                     new NavRoute("Composite View", PATH_BASE + "/compositeview"),
                     new NavRoute("Secure Page", PATH_BASE + "/securepage")
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
                     new NavRoute ("Multicast", PATH_BASE + "/multicast"),
                     new NavRoute ("Security", PATH_BASE + "/security"),
                     new NavRoute ("Web API Mode", PATH_BASE + "/webapimode")
                  },
                  IsExpanded = false
               }
            })
         );
      }
   }
}