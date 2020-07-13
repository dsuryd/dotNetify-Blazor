using System;
using System.Collections.Generic;
using System.Text;

namespace Basic.Shared
{
   public interface IHelloWorldState
   {
      string Greetings { get; set; }
      DateTime ServerTime { get; set; }

      void Submit(Person person);
   }
}