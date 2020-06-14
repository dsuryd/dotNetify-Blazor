using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace DotNetify.Blazor
{
   public static class Extensions
   {
      /// <summary>
      /// Adds DotNetify-Blazor services to the service collection.
      /// </summary>
      public static IServiceCollection AddDotNetifyBlazor(this IServiceCollection services)
      {
         services.AddTransient(typeof(IVMProxy), typeof(VMProxy));
         return services;
      }

      #region Internal

      internal static T As<T>(this object arg) => arg.As(s => JsonConvert.DeserializeObject<T>(s));

      internal static T As<T>(this object arg, JsonSerializerSettings settings) => arg.As(s => JsonConvert.DeserializeObject<T>(s, settings));

      internal static T As<T>(this object arg, params JsonConverter[] converters) => arg.As(s => JsonConvert.DeserializeObject<T>(s, converters));

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