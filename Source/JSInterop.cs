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
}