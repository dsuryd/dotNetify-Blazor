using DotNetify;
using System.Windows.Input;

namespace Website.Server
{
   public class Counter : BaseVM
   {
      public int CurrentCount
      {
         get => Get<int>();
         set => Set(value);
      }

      public ICommand IncrementCount => new Command(() => CurrentCount++);
   }
}