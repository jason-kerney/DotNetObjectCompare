namespace FindDifferencesInObjects
open System
open System.Linq
open BasicProperties
open System.Reflection

type PropertyDef = {
    Name : string;
    Value : Object;
    PropertyType : Type;
}

type PropertyValues(obj : Object) =
    member val Values = getProperties(obj).Select(fun (p : PropertyInfo ) -> { Name = p.Name; Value = p.GetValue(obj, null); PropertyType = p.PropertyType })

