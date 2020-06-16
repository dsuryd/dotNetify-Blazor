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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace DotNetify.Blazor
{
   public interface IVMProxy : IDisposable
   {
      /// <summary>
      /// Reference to the associated 'd-vm-context' HTML markup.
      /// </summary>
      ElementReference ElementRef { get; set; }

      /// <summary>
      /// Listens to the state change event from the server-side view model.
      /// </summary>
      /// <param name="stateChangeEventCallback">Gets called when the client receives state change from the server-side view model.</param>
      Task HandleStateChangeAsync<TState>(Action<TState> stateChangeEventCallback);

      /// <summary>
      /// Listens to the events from the web component elements under this VM context.
      /// </summary>
      /// <param name="eventCallback">Gets called when an element under this VM context raises an event.</param>
      Task HandleElementEventAsync(Action<ElementEvent> eventCallback);

      /// <summary>
      /// Listens to an event from a DOM element.
      /// </summary>
      /// <typeparam name="TEventArg">Event argument type.</typeparam>
      /// <param name="domElement">Document element.</param>
      /// <param name="eventName">Event name.</param>
      /// <param name="eventHandler">Event callback.</param>
      Task HandleDomEventAsync<TEventArg>(string eventName, ElementReference domElement, Action<TEventArg> eventCallback);

      /// <summary>
      /// Dispatches property value to server-side view model.
      /// </summary>
      /// <param name="propertyName">Name that matches a server-side view model property.</param>
      /// <param name="propertyValue">Value to be dispatched.</param>
      Task DispatchAsync(string propertyName, object propertyValue = null);

      /// <summary>
      /// Disposes the context element.
      /// </summary>
      /// <returns></returns>
      Task DisposeAsync();
   }
}