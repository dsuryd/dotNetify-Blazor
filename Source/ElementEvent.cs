namespace DotNetify.Blazor
{
   public class ElementEvent
   {
      /// <summary>
      /// Identifies the DOM element raising the event.
      /// </summary>
      public string TargetId { get; set; }

      /// <summary>
      /// Event name.
      /// </summary>
      public string EventName { get; set; }

      /// <summary>
      /// Event arguments.
      /// </summary>
      public object EventArgs { get; set; }
   }
}