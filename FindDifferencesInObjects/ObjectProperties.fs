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
        if this.Name <> other.Name then
            DifferentNameResult(this.Name, other.Name)
        elif this.Value <> other.Value then
            DifferentResult(this.Name, this.Value, other.Value)
        else
            SameResult
