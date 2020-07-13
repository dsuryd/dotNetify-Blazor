using System;
using System.Threading;
using Basic.Shared;
using DotNetify;

namespace Basic.Server
{
   public class HelloWorld : BaseVM, IHelloWorldState
   {
      private readonly Timer timer;

      public string Greetings
      {
         get => Get<string>();
         set => Set(value);
      }

      public DateTime ServerTime
      {
         get => Get<DateTime>();
         set => Set(value);
      }

      public HelloWorld()
      {
         Greetings = "Hello World";
         ServerTime = DateTime.Now;

         timer = new Timer(state =>
         {
            ServerTime = DateTime.Now;
            PushUpdates();
         }, null, 0, 1000);
      }

      public override void Dispose()
      {
         timer.Dispose();
      }

      public void Submit(Person person)
      {
         Greetings = $"Hello {person.FirstName} {person.LastName}!";
      }
   }
}