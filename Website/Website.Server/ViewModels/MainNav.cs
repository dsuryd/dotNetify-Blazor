using DotNetify;
using DotNetify.Elements;

namespace Website.Server
{
   public class MainNav : BaseVM
   {
      public const string PATH_BASE = "";

      public MainNav()
      {
         AddProperty("NavMenu", new NavMenu(
            new NavMenuItem[]
            {
               new NavRoute("Overview", PATH_BASE + "/"),
               new NavRoute("Basics", PATH_BASE + "/basics"),
               new NavRoute("Get Started", PATH_BASE + "/getstarted"),
               new NavGroup
               {
                  Label = "Topics",
                  Routes = new NavRoute[]
                  {
                     new NavRoute("AWS Integration", PATH_BASE + "/aws-integration"),
                     new NavRoute("Data Flow Pattern", PATH_BASE + "/dataflow"),
                     new NavRoute("Reactive Programming", PATH_BASE + "/reactive"),
                     new NavRoute("Real-time PostgreSQL", PATH_BASE + "/postgres"),
                     new NavRoute("Scale-Out", PATH_BASE + "/scaleout"),
                  },
                  IsExpanded = false
               },
               new NavGroup
               {
                  Label = "Examples",
                  Routes = new NavRoute[]
                  {
                     new NavRoute("Hello World", PATH_BASE + "/helloworld"),
                     new NavRoute("Control Types", PATH_BASE + "/controltypes"),
                     new NavRoute("Simple List", PATH_BASE + "/simplelist"),
                     new NavRoute("Composite View", PATH_BASE + "/compositeview"),
                     new NavRoute("Secure Page", PATH_BASE + "/securepage"),
                     new NavRoute("Chat Room", PATH_BASE + "/chatroom"),
                  },
                  IsExpanded = true
               },
               new NavGroup
               {
                  Label = "Examples with Elements",
                  Routes = new NavRoute[]
                  {
                     new NavRoute("Customer Form", PATH_BASE + "/form"),
                     new NavRoute("Admin Dashboard", PATH_BASE + "/dashboard"),
                  },
                  IsExpanded = false
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
                     new NavRoute ("Scoped CSS", PATH_BASE + "/scopedcss"),
                     new NavRoute ("Security", PATH_BASE + "/security"),
                     new NavRoute ("Web API Mode", PATH_BASE + "/webapimode")
                  },
                  IsExpanded = false
               },
               new NavGroup
               {
                  Label = "For Sponsors",
                  Icon = "material-icons volunteer_activism",
                  Routes = new NavRoute[]
                  {
                     new NavRoute ("DotNetify-ResiliencyAddon", PATH_BASE + "/dotnetify-resiliencyaddon"),
                     new NavRoute ("DotNetify-Observer", PATH_BASE + "/dotnetify-observer"),
                     new NavRoute ("DotNetify-LoadTester", PATH_BASE + "/dotnetify-loadtester"),
                     new NavRoute ("DotNetify-Testing", PATH_BASE + "/dotnetify-testing"),
                  },
                  IsExpanded = false
               }
            })
         );
      }
   }
}