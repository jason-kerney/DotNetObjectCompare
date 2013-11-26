namespace FindDifferencesInObjects

type CompareResult =
    | SameResult
    | DifferentNameResult of string * string
    | DifferentResult of string * System.Object * System.Object

type ObjectProperty(name : string, propertyType : System.Type, value : System.Object) =
    member val Name = name
    member val Value = value
    member val PropertyType = propertyType

    member this.Compare (other : ObjectProperty) =
        match other with
        | o when o.Name <> this.Name -> DifferentNameResult(this.Name, o.Name)
        | o when o.Value <> this.Value -> DifferentResult(this.Name, this.Value, o.Value)
        | _ -> SameResult
