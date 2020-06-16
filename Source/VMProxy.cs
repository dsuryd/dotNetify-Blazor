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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace DotNetify.Blazor
{
   public class VMProxy : IVMProxy
   {
      private readonly JSInterop _jsInterop;
      private readonly HashSet<Delegate> _delegates = new HashSet<Delegate>();
      private readonly JsonSerializerSettings _jsonSerializerSettings;
      private ElementReference? _vmContextElemRef;

      public ElementReference ElementRef
      {
         get => _vmContextElemRef.Value;
         set => _vmContextElemRef = value;
      }

      public VMProxy(IJSRuntime jsRuntime)
      {
         _jsInterop = new JSInterop(jsRuntime);
         _jsonSerializerSettings = new JsonSerializerSettings
         {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
         };
      }

      public void Dispose()
      {
         _ = DisposeAsync();
      }

      public async Task DisposeAsync()
      {
         if (_vmContextElemRef.HasValue)
            await _jsInterop.RemoveAllEventListenersAsync(ElementRef);
      }

      public Task HandleStateChangeAsync<TState>(Action<TState> stateChangeCallback)
      {
         if (!_vmContextElemRef.HasValue)
            throw new ArgumentNullException(nameof(ElementReference));

         return HandleDomEventAsync("onStateChange", ElementRef, stateChangeCallback);
      }

      public Task HandleElementEventAsync(Action<ElementEvent> eventCallback)
      {
         if (!_vmContextElemRef.HasValue)
            throw new ArgumentNullException(nameof(ElementReference));

         return HandleDomEventAsync("onElementEvent", ElementRef, eventCallback);
      }

      public Task HandleDomEventAsync<TEventArg>(string eventName, ElementReference domElement, Action<TEventArg> eventCallback)
      {
         if (_delegates.Contains(eventCallback))
            return Task.CompletedTask;

         _delegates.Add(eventCallback);
         return _jsInterop.AddEventListenerAsync<TEventArg>(eventName, domElement, arg => eventCallback?.Invoke(arg));
      }

      public async Task DispatchAsync(string propertyName, object propertyValue = null)
      {
         var data = new Dictionary<string, object>() { { propertyName, propertyValue } };
         await _jsInterop.DispatchAsync(ElementRef, JsonConvert.SerializeObject(data, _jsonSerializerSettings));
      }
   }
}