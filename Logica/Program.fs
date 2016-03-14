// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

//1. Define a function that takes two integers n, m and another integer x and do the following: if the product of n and x is m return the string
//"Tro", if the product of m and x is n return "lolo", and if the product of n and m is x return "Trololo". In all other cases just return an emtpy string.
module Program
open Factory

[<EntryPoint>]

let main argv = 
    printfn "%A" argv
    0 // return an integer exit code
