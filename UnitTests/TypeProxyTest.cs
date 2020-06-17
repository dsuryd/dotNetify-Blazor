using System;
using System.Collections.Generic;
using System.Linq;
using DotNetify.Blazor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTests
{
   [TestClass]
   public class TypeProxyTest
   {
      public interface IState
      {
         string StringValue { get; set; }
         int IntValue { get; set; }
         double DoubleValue { get; set; }
         IEnumerable<string> StringEnumerable { get; set; }
         IEnumerable<int> IntEnumerable { get; set; }
      }

      public interface IWatchedState
      {
         [Watch]
         public string StringValue { get; set; }
      }

      [TestMethod]
      public void TypeProxy_CanCreateObjectFromInterface()
      {
         var obj = TypeProxy.CreateInstance<IState>();

         obj.StringValue = "hello";
         obj.IntValue = int.MaxValue;
         obj.DoubleValue = Math.PI;
         obj.StringEnumerable = new List<string> { "Alpha", "Omega" };
         obj.IntEnumerable = new int[] { int.MinValue, int.MaxValue };

         Assert.AreEqual("hello", obj.StringValue);
         Assert.AreEqual(int.MaxValue, obj.IntValue);
         Assert.AreEqual(Math.PI, obj.DoubleValue);
         Assert.AreEqual("Alpha", obj.StringEnumerable.First());
         Assert.AreEqual("Omega", obj.StringEnumerable.Last());
         Assert.AreEqual(int.MinValue, obj.IntEnumerable.First());
         Assert.AreEqual(int.MaxValue, obj.IntEnumerable.Last());
      }

      [TestMethod]
      public void TypeProxy_CanSerializeObject()
      {
         var obj = TypeProxy.CreateInstance<IState>();

         obj.StringValue = "hello";
         obj.IntValue = int.MaxValue;
         obj.DoubleValue = Math.PI;
         obj.StringEnumerable = new List<string> { "Alpha", "Omega" };
         obj.IntEnumerable = new int[] { int.MinValue, int.MaxValue };

         string serialized = JsonConvert.SerializeObject((IState) obj);
         Assert.AreEqual("{\"StringValue\":\"hello\",\"IntValue\":2147483647,\"DoubleValue\":3.141592653589793,\"StringEnumerable\":[\"Alpha\",\"Omega\"],\"IntEnumerable\":[-2147483648,2147483647]}", serialized);
      }

      [TestMethod]
      public void TypeProxy_CanDeserializeObject()
      {
         string data = "{\"StringValue\":\"hello\",\"IntValue\":2147483647,\"DoubleValue\":3.141592653589793,\"StringEnumerable\":[\"Alpha\",\"Omega\"],\"IntEnumerable\":[-2147483648,2147483647]}";

         Type objType = TypeProxy.CreateType<IState>();
         IState obj = (IState) JsonConvert.DeserializeObject(data, objType);

         Assert.AreEqual("hello", obj.StringValue);
         Assert.AreEqual(int.MaxValue, obj.IntValue);
         Assert.AreEqual(Math.PI, obj.DoubleValue);
         Assert.AreEqual("Alpha", obj.StringEnumerable.First());
         Assert.AreEqual("Omega", obj.StringEnumerable.Last());
         Assert.AreEqual(int.MinValue, obj.IntEnumerable.First());
         Assert.AreEqual(int.MaxValue, obj.IntEnumerable.Last());
      }
   }
}