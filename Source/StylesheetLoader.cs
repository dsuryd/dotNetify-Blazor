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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DotNetify.Blazor
{
   public class StylesheetLoader : IStylesheet
   {
      private readonly List<KeyValuePair<string, string>> _stylesheets = new List<KeyValuePair<string, string>>();

      /// <summary>
      /// Loads all stylesheets in an assembly.
      /// </summary>
      internal void Load(Assembly assembly)
      {
         var stylesheets = assembly.GetManifestResourceNames()
            .Where(resource => resource.ToLower().EndsWith(".css") || resource.ToLower().EndsWith(".scss"))
            .Select(stylesheetFile =>
            {
               using Stream stream = assembly.GetManifestResourceStream(stylesheetFile);
               using StreamReader reader = new StreamReader(stream);
               return new KeyValuePair<string, string>(stylesheetFile, reader.ReadToEnd());
            });

         _stylesheets.AddRange(stylesheets);
      }

      /// <summary>
      /// Reads a stylesheet.
      /// </summary>
      /// <param name="fileName">Stylesheet file name.</param>
      public string Read(string fileName)
      {
         var stylesheet = _stylesheets.FirstOrDefault(x => x.Key.Contains(fileName));
         if (string.IsNullOrEmpty(stylesheet.Key))
            throw new FileNotFoundException($"No embedded stylesheet resource by the name of '{fileName}'.");

         // Remove whitespaces, except those between words.
         return Regex.Replace(stylesheet.Value, @"(?!\b\s+\b)\s+", string.Empty);
      }
   }
}