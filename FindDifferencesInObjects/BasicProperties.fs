namespace FindDifferencesInObjects
open System

module BasicProperties =
    let getProperties item =
        item.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(info => info.CanRead)
