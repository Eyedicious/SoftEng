module EntitiesManager

open Entities

let tuple x y = (x,y)
let RandomNumber(vL) = System.Random().Next(0,vL)

let spawnFactory =
   Factory.Create(RandomNumber(1600), RandomNumber(800))
   