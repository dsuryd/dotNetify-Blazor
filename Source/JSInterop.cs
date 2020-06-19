/*
Copyright 2020 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace DotNetify.Blazor
{
   /// <summary>
   /// Javascript interop with dotnetify-blazor.js.
   /// </summary>
   public class JSInterop
   {
      private readonly IJSRuntime _jsRuntime;

      internal static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
      {
         ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      };

      internal static readonly JsonSerializerSettings _serializerCamelCaseSettings = new JsonSerializerSettings
      {
         ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
         ContractResolver = new CamelCasePropertyNamesContractResolver()
      };

      public JSInterop(IJSRuntime jsRuntime)
      {
         _jsRuntime = jsRuntime;
      }

      public async Task AddEventListenerAsync<TEventArg>(string eventName, ElementReference elementRef, Action<TEventArg> eventCallback)
      {
         var jsCallback = new JSCallback(arg => eventCallback?.Invoke(arg.As<TEventArg>()));
         await _jsRuntime.InvokeAsync<object>("dotnetify_blazor.addEventListener", eventName, elementRef, DotNetObjectReference.Create(jsCallback));
      }

      public async Task AddEventListenerAsync<TEventArg>(string eventName, string elementSelector, Action<TEventArg> eventCallback)
      {
         var jsCallback = new JSCallback(arg => eventCallback?.Invoke(arg.As<TEventArg>()));
         await _jsRuntime.InvokeAsync<object>("dotnetify_blazor.addEventListener", eventName, elementSelector, DotNetObjectReference.Create(jsCallback));
      }

      public async Task ConfigureDotNetifyAsync(ClientConfiguration clientConfiguration)
      {
         await _jsRuntime.InvokeAsync<object>("dotnetify_blazor.configure", clientConfiguration);
      }

      public async Task DispatchAsync(ElementReference elementRef, string data)
      {
         await _jsRuntime.InvokeAsync<object>("dotnetify_blazor.dispatch", elementRef, data);
      }

      public async Task RemoveAllEventListenersAsync(ElementReference elementRef)
      {
         await _jsRuntime.InvokeAsync<object>("dotnetify_blazor.removeAllEventListeners", elementRef);
      }
   }

   internal static class JSInteropExtensions
   {
      public static T As<T>(this object arg)
      {
         if (typeof(T).IsInterface)
            return arg.As(s => (T) JsonConvert.DeserializeObject(s, TypeProxy.CreateType<T>()));

         return arg.As(s => JsonConvert.DeserializeObject<T>(s));
      }

      public static T As<T>(this object arg, Func<string, T> deserialize)
      {
         if (typeof(T) == typeof(object))
            return (T) arg;

         try
         {
            return typeof(T) == typeof(string) ? (T) (object) $"{arg}" : deserialize($"{arg}");
         }
         catch (Exception ex)
         {
            throw new JsonSerializationException($"Cannot deserialize {arg} to {typeof(T)}", ex);
         }
      }

      public static string Serialize<T>(this T arg, bool camelCase = false)
      {
         if (typeof(T) == typeof(string))
            return arg.ToString();

         return JsonConvert.SerializeObject(arg, camelCase ? JSInterop._serializerCamelCaseSettings : JSInterop._serializerSettings);
      }
   }
}