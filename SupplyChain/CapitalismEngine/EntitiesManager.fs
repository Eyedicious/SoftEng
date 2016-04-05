module EntitiesManager

open Entities
        
let RandomGen = new System.Random()
let RandomNumberH(vL) = RandomGen.Next(0,vL)
let RandomNumberV(vL) = RandomGen.Next(0,vL)

let spawnFactory() =
   let newFactory = new Factory(RandomNumberH(1600), RandomNumberV(1600))
   newFactory
   