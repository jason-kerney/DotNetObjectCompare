namespace FindDifferencesInObjects
open System
open System.Linq
open BasicProperties
open System.Reflection


type PropertyValues(obj : Object) =
    member val Values = getProperties(obj).Select(fun (p : PropertyInfo ) -> new ObjectProperty(p.Name, p.PropertyType, p.GetValue(obj, null))).ToDictionary(fun p -> p.Name)

    member this.Compare (other : PropertyValues) = 
        let rec compareEach keys acc =
            match keys with
            | [] -> acc
            | head :: tail when other.Values.ContainsKey(head) ->
                this.Values.[head].Compare(other.Values.[head]) :: acc |> compareEach tail
            | head :: tail -> compareEach tail acc

        let keys = this.Values.Keys |> Seq.toList

        compareEach keys [] |> List.toArray