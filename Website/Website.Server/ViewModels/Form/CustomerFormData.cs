using System.Collections.Generic;

namespace Website.Server
{
   using StringDictionary = Dictionary<string, string>;

   public class CustomerFormData
   {
      public StringDictionary Person { get; set; }
      public StringDictionary Phone { get; set; }
      public StringDictionary Address { get; set; }
   }
}