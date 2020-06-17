using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DotNetify.Blazor
{
   public abstract class BaseObject<TInterface>
   {
      private readonly Dictionary<string, object> _propValues = new Dictionary<string, object>();

      internal TInterface Interface { get; set; }

      public T Get<T>(string propName)
      {
         if (!_propValues.ContainsKey(propName))
            _propValues[propName] = default(T);

         return (T) _propValues[propName];
      }

      public void Set<T>(string propName, T value) => _propValues[propName] = value;
   }

   public static class TypeProxy
   {
      private static ModuleBuilder _builder;
      private static readonly Dictionary<string, Type> _createdTypes = new Dictionary<string, Type>();
      private static readonly object _sync = new object();

      public static Type CreateType<T>()
      {
         lock (_sync)
         {
            Type ifaceType = typeof(T);
            Type baseType = typeof(BaseObject<>).MakeGenericType(ifaceType);
            string typeName = ToObjectTypeName(ifaceType);

            Type objectType = _createdTypes.ContainsKey(typeName) ? _createdTypes[typeName] : null;
            if (objectType == null)
            {
               ModuleBuilder builder = GetModuleBuilder();

               var typeBuilder = builder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class, baseType);
               typeBuilder.AddInterfaceImplementation(ifaceType);
               typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

               var typesToBuild = new List<Type>();
               var typesToCheck = new Stack<Type>();
               typesToCheck.Push(ifaceType);
               while (typesToCheck.Count > 0)
               {
                  Type typeToCheck = typesToCheck.Pop();
                  typeToCheck.GetInterfaces().ToList().ForEach(type => typesToCheck.Push(type));
                  typesToBuild.Add(typeToCheck);
               }

               typesToBuild.ForEach(type =>
               {
                  typeBuilder.BuildProperties(type, baseType);
               });

               objectType = typeBuilder.CreateTypeInfo();
               _createdTypes[typeName] = objectType;
            }

            return objectType;
         }
      }

      public static T CreateInstance<T>() where T : class
      {
         return (T) Activator.CreateInstance(CreateType<T>());
      }

      private static ModuleBuilder GetModuleBuilder()
      {
         if (_builder != null)
            return _builder;

         var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DotNetifyBlazorDynamicAssembly"), AssemblyBuilderAccess.Run);
         _builder = assembly.DefineDynamicModule("DotNetifyBlazorDynamicModule");
         return _builder;
      }

      private static void BuildProperties(this TypeBuilder typeBuilder, Type ifaceType, Type baseType)
      {
         foreach (PropertyInfo prop in ifaceType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
         {
            var propBuilder = typeBuilder.DefineProperty(prop.Name, PropertyAttributes.HasDefault, prop.PropertyType, null);

            if (prop.CanRead)
            {
               var getMethod = prop.GetGetMethod();
               var methodParams = getMethod.GetParameters();
               bool isIndexer = methodParams.Length > 0;
               var paramTypes = isIndexer ? methodParams.Select(x => x.ParameterType) : Type.EmptyTypes;

               var methodBuilder = typeBuilder.DefineMethod(getMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, prop.PropertyType, paramTypes.ToArray());
               ILGenerator il = methodBuilder.GetILGenerator();
               il.Emit(OpCodes.Ldarg_0);
               if (!isIndexer)
               {
                  il.Emit(OpCodes.Ldstr, prop.Name);
                  il.Emit(OpCodes.Call, baseType.GetMethod("Get").MakeGenericMethod(prop.PropertyType));
               }
               il.Emit(OpCodes.Ret);

               propBuilder.SetGetMethod(methodBuilder);
            }

            if (prop.CanWrite)
            {
               var setMethod = prop.GetSetMethod();

               var baseStubMethod = baseType.GetMethod("Set").MakeGenericMethod(setMethod.GetParameters().First().ParameterType);
               var paramTypes = setMethod.GetParameters().Select(paramInfo => paramInfo.ParameterType).ToArray();
               var methodBuilder = typeBuilder.DefineMethod(setMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), paramTypes);
               ILGenerator il = methodBuilder.GetILGenerator();
               il.Emit(OpCodes.Ldarg_0);
               il.Emit(OpCodes.Ldstr, prop.Name);
               il.Emit(OpCodes.Ldarg_1);
               il.Emit(OpCodes.Call, baseStubMethod);
               il.Emit(OpCodes.Ret);

               propBuilder.SetSetMethod(methodBuilder);
            }
         }
      }

      private static string ToObjectTypeName(Type type) => $"Impl__{type.FullName}";
   }
}