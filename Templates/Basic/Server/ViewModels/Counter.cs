using Basic.Shared;
using DotNetify;

namespace Basic.Server
{
   public class Counter : BaseVM, ICounterState
   {
      public int CurrentCount
      {
         get => Get<int>();
         set => Set(value);
      }

      public void IncrementCount() => CurrentCount++;
   }
}