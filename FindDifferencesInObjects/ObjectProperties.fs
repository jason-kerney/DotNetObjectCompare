namespace FindDifferencesInObjects
open System

type CompareResult =
    | SameResult
    | DifferentNameResult of string * string
    | DifferentResult of string * System.Object * System.Object
    with
        override this.ToString() =
            match this with
            | SameResult -> "The items are equal."
            | DifferentNameResult(left, right) -> String.Format("Property names differ: {0} <> {1}", left, right)
            | DifferentResult(name, left, right) -> String.Format("{0}: '{1}' <> '{0}'", name, left, right)


type ObjectProperty(name : string, propertyType : System.Type, value : System.Object) =
    member val Name = name
    member val Value = value
    member val PropertyType = propertyType

    member this.Compare (other : ObjectProperty) =
        match other with
        | o when o.Name <> this.Name -> DifferentNameResult(this.Name, o.Name)
        | o when o.Value <> this.Value -> DifferentResult(this.Name, this.Value, o.Value)
        | _ -> SameResult

    override this.ToString() =
        String.Format("{0}: '{1}' {2}", this.Name, this.Value, this.PropertyType)
