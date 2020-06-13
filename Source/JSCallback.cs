using Microsoft.JSInterop;
using System;

namespace DotNetify.Blazor
{
   public class JSCallback
   {
      private Action<object> _callback;

      public JSCallback(Action<object> callback)
      {
         _callback = callback;
      }

      [JSInvokable]
      public string Callback(object arg)
      {
         _callback(arg);
         return string.Empty;
      }
   }
}