namespace FindDifferencesInObjects
open System
open System.Reflection
open System.Linq

module BasicProperties =
    let getProperties (item : Object) =
        item.GetType().GetProperties(BindingFlags.Instance ||| BindingFlags.Public).Where(fun (info : PropertyInfo) -> info.CanRead)
