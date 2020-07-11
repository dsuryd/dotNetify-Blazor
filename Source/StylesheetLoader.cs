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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DotNetify.Blazor
{
   internal class StyleSheetLoader : IStyleSheet
   {
      private readonly List<KeyValuePair<string, string>> _styleSheets = new List<KeyValuePair<string, string>>();

      /// <summary>
      /// Gets the content of a style sheet embedded resource. It doesn't need the full name, only a sufficiently unique substring.
      /// </summary>
      /// <param name="embeddedResourceName">Name of the embedded resource containing the style sheet.</param>
      public string this[string embeddedResourceName]
      {
         get
         {
            var styleSheet = _styleSheets.FirstOrDefault(x => x.Key.Equals(embeddedResourceName, StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrEmpty(styleSheet.Key))
               throw new FileNotFoundException($"No embedded StyleSheet resource by the name of '{embeddedResourceName}'.");

            // Remove whitespaces, except those between words.
            return Regex.Replace(styleSheet.Value, @"(?!\b\s+\b)(?!\s\d)(?!\s[-\+])\s+", string.Empty).Replace(": ", ":");
         }
      }

      /// <summary>
      /// Loads all style sheets in an assembly.
      /// </summary>
      internal void Load(Assembly assembly)
      {
         var styleSheets = assembly.GetManifestResourceNames()
            .Where(resource => resource.ToLower().EndsWith(".css") || resource.ToLower().EndsWith(".scss"))
            .Select(resourceName =>
            {
               using Stream stream = assembly.GetManifestResourceStream(resourceName);
               using StreamReader reader = new StreamReader(stream);

               // Use the resource name excluding namespace and extensions for the lookup key.
               string pattern = resourceName.Contains(".razor.") ? @"(?:.+\.)+(.+)\.razor\.s?css" : @"(?:.+\.)+(.+)\.s?css";
               var match = Regex.Match(resourceName, pattern);
               string key = match.Success && match.Groups.Count > 1 ? match.Groups[1].Value : resourceName;

               return new KeyValuePair<string, string>(key, reader.ReadToEnd());
            });

         _styleSheets.AddRange(styleSheets);
      }
   }
}