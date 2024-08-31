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
using Newtonsoft.Json;

namespace DotNetify.Blazor
{
   /// <summary>
   /// Configuration options that can be sent along with request to connect to a server-side view model.
   /// </summary>
   public class VMConnectOptions
   {
      /// <summary>
      /// Arguments to initialize the view model.
      /// </summary>
      public Dictionary<string, object> VMArg { get; set; }

      /// <summary>
      /// Request headers.
      /// </summary>
      public Dictionary<string, object> Headers { get; set; }

      /// <summary>
      /// Use HTTP Web API endpoint instead of SignalR hub.
      /// </summary>
      public bool WebApi { get; set; }

      /// <summary>
      /// Callback when getting exception from server-side view model.
      /// </summary>
      [JsonIgnore]
      public Action<ExceptionEventArgs> ExceptionHandler { get; set; }
   }
}