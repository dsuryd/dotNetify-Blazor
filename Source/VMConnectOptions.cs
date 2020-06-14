namespace DotNetify.Blazor
{
   /// <summary>
   /// Configuration options that can be sent along with request to connect to a server-side view model.
   /// </summary>
   public class VMConnectOptions
   {
      /// <summary>
      /// Arguments to initialize the view model.
      /// </summary>
      public object VMArg { get; set; }

      /// <summary>
      /// Request headers.
      /// </summary>
      public object Headers { get; set; }
   }
}