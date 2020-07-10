using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DotNetify.Elements;
using System.Threading.Tasks;

namespace Website.Server
{
   public interface IWebStoreService
   {
      Task<IEnumerable<WebStoreRecord>> GetAllBooksAsync();

      Task<WebStoreRecord> GetBookByTitleAsync(string title);

      Task<WebStoreRecord> GetBookByIdAsync(int id);
   }

   public class WebStoreRecord
   {
      public int Id { get; set; }
      public string Type { get; set; }
      public string Category { get; set; }
      public bool Recommended { get; set; }
      public string Title { get; set; }
      public string Author { get; set; }
      public float Rating { get; set; }
      public string ImageUrl { get; set; }
      public string ItemUrl { get; set; }
      public string UrlSafeTitle => ToUrlSafe(Title);

      public static string ToUrlSafe(string title) => title.ToLower()
         .Replace("\'", "")
         .Replace(".", "dot")
         .Replace("#", "sharp")
         .Replace(' ', '-');
   }

   public class WebStoreService : IWebStoreService
   {
      public async Task<IEnumerable<WebStoreRecord>> GetAllRecordsAsync() => JsonConvert.DeserializeObject<List<WebStoreRecord>>(
         await Utils.GetResource("Website.Server.Services.webstore.json", GetType().Assembly));

      public async Task<IEnumerable<WebStoreRecord>> GetAllBooksAsync() => (await GetAllRecordsAsync()).Where(i => i.Type == "Book");

      public async Task<WebStoreRecord> GetBookByTitleAsync(string title) => (await GetAllBooksAsync()).FirstOrDefault(i => i.UrlSafeTitle == title);

      public async Task<WebStoreRecord> GetBookByIdAsync(int id) => (await GetAllBooksAsync()).FirstOrDefault(i => i.Id == id);
   }
}