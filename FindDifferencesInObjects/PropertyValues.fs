namespace FindDifferencesInObjects
open System
open System.Linq

type PropertyDef = {
    Name : string;
    Value : Object;
    PropertyType : Type;
}

type PropertyValues(obj : Object) =
    member val Values = BasicProperties.getProperties(obj).Select(fun p -> { Name = "Test"; Value = "Hello"; PropertyType = typedefof<string> })

