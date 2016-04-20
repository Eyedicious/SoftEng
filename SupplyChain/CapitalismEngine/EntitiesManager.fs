module EntitiesManager

open Entities
        
let RandomGen = new System.Random()
let RandomNumberH(vL) = RandomGen.Next(0,vL)
let RandomNumberV(vL) = RandomGen.Next(0,vL)

let spawnFactory() =
   {
      Factory.coordinatesX = float32(RandomNumberH(1600));
      Factory.coordinatesY = float32(RandomNumberV(900));
      Factory.productionType = resources.[RNG.Next(0, resources.Length-1)];
      Factory.laboring = Working 10.0f
   }
   //let newFactory = new Factory(RandomNumberH(1600), RandomNumberV(1600))
   //newFactory