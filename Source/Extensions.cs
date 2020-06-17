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
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace DotNetify.Blazor
{
   public static class Extensions
   {
      /// <summary>
      /// Adds DotNetify-Blazor services to the service collection.
      /// </summary>
      public static IServiceCollection AddDotNetifyBlazor(this IServiceCollection services, Action<ClientConfiguration> options = null)
      {
         var config = new ClientConfiguration();
         var stylesheetLoader = new StylesheetLoader();

         services.AddTransient<IVMProxy, VMProxy>();
         services.AddSingleton<IStylesheet>(stylesheetLoader);

         options?.Invoke(config);

         // Load stylesheet files from the specified assembly(ies).
         config.StylesheetAssemblies ??= new Assembly[] { Assembly.GetCallingAssembly() };
         foreach (var assembly in config.StylesheetAssemblies)
            stylesheetLoader.Load(assembly);

         // Execute scripts to set specified dotNetify configuration.
         var jsInterop = new JSInterop(services.BuildServiceProvider().GetRequiredService<IJSRuntime>());
         _ = jsInterop.ConfigureDotNetifyAsync(config);

         return services;
      }

      #region Internal

      internal static T As<T>(this object arg) => arg.As(s => JsonConvert.DeserializeObject<T>(s));

      internal static T As<T>(this object arg, Func<string, T> deserialize)
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
   }

   #endregion Internal
}