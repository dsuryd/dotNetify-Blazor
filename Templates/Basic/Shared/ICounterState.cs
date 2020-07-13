namespace Basic.Shared
{
   public interface ICounterState
   {
      int CurrentCount { get; set; }

      void IncrementCount();
   }
}