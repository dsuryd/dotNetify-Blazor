using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetify;

namespace Website.Server
{
   public class MasterDetails : BaseVM
   {
      private readonly IWebStoreService _webStoreService;

      private event EventHandler<int> SelectedItem;

      public MasterDetails(IWebStoreService webStoreService)
      {
         _webStoreService = webStoreService;
      }

      public override void OnSubVMCreated(BaseVM vm)
      {
         if (vm is Master)
         {
            var master = vm as Master;
            master.Selected += (sender, id) => SelectedItem?.Invoke(this, id);
         }
         else if (vm is Details)
         {
            var detail = vm as Details;
            SelectedItem += (sender, id) =>
            {
               detail.SetData(_webStoreService.GetBookByIdAsync(id).Result);
            };
         }
      }
   }

   public class Master : BaseVM
   {
      private readonly IWebStoreService _webStoreService;

      public IEnumerable<WebStoreRecord> ListItems
      {
         get => Get<IEnumerable<WebStoreRecord>>();
         set => Set(value);
      }

      public event EventHandler<int> Selected;

      public Master(IWebStoreService webStoreService)
      {
         _webStoreService = webStoreService;
      }

      public override async Task OnCreatedAsync()
      {
         ListItems = await _webStoreService.GetAllBooksAsync();
      }

      public void Select(int id) => Selected?.Invoke(this, id);
   }

   public class Details : BaseVM
   {
      public string ItemImageUrl
      {
         get => Get<string>();
         set => Set(value);
      }

      public void SetData(WebStoreRecord data) => ItemImageUrl = data.ImageUrl;
   }
}