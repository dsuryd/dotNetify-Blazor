using DotNetify;
using DotNetify.Elements;

namespace Website.Server
{
   public class Overview : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Overview.md");
   }

   #region Examples

   public class HelloWorldDoc : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Examples.HelloWorld.md");
   }

   public class ControlTypesDoc : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Examples.ControlTypes.md");
   }

   public class SimpleListDoc : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Examples.SimpleList.md");
   }

   public class CompositeViewDoc : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Examples.CompositeView.md");
   }

   public class SecurePageDoc : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Examples.SecurePage.md");
   }

   public class DashboardDoc : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Examples.Dashboard.md");
   }

   public class FormDoc : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Examples.Form.md");
   }

   #endregion Examples

   #region API References

   public class CRUD : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.CRUD.md");
   }

   public class DI : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.DI.md");
   }

   public class DotNetClient : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.DotNetClient.md");
   }

   public class Filter : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Filter.md");
   }

   public class Middleware : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Middleware.md");
   }

   public class Multicast : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Multicast.md");
   }

   public class Security : BaseVM
   {
      public string Content => new Markdown("Website.Server.Docs.Security.md");
   }

   #endregion API References
}