module MathEngine

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

let factoryAmount = 500

type Spawnable =
   | Cooldown of float32
   | Ready

type GameState = 
   {
      Factories      : List<Entities.Factory>
      Trucks         : List<Entities.Truck>
      Endpoints      : List<Entities.Endpoint>
      Spawnrate      : Spawnable
   }

type Drawable = 
   {
     Position : Vector2
     Image    : string
   }

type Terrain =
    {
      LandPixels        : seq<int * int>
      WaterPixels       : seq<int * int>
      WastelandPixels   : seq<int * int>
    } member t.getRandomTileCoordinates(typeOfTile: BitmapProcessing.TerrainType) =
       let collectionToUse = 
          match typeOfTile with
          | BitmapProcessing.TerrainType.Land ->
            t.LandPixels
          | BitmapProcessing.TerrainType.Water ->
            t.WaterPixels
          | BitmapProcessing.TerrainType.Wasteland ->
            t.WastelandPixels
          | BitmapProcessing.TerrainType.ProvinceBorder ->
            //Do nothing since not implemetned
            Seq.empty
       let randomIndex = ((new System.Random()).Next(0, (Seq.length collectionToUse) - 1))
       collectionToUse |> Seq.nth randomIndex

let mutable TerrainCollection =
   {
      LandPixels        = Seq.empty
      WaterPixels       = Seq.empty
      WastelandPixels   = Seq.empty
   }

let rec filter (fn:'a -> bool) (e:'a List) = 
   match e with
   | [] -> []
   | (x::xs) ->
      if fn x then
         x :: filter fn xs
      else
         filter fn xs

let foreachDo (fn:'a->'b->'c) (y:'a) =     //Foreach x DO fn y x and return the editted list
   let rec inspectX (x:List<'b>) = 
      match x with
         | [] -> []
         | (x::[]) -> fn y x :: []
         | (x::xs) -> (fn y x)::(inspectX xs)
   inspectX

let updateFactory (dt:float32) (factory:Entities.Factory) =
   match factory.laboring with
   |  Entities.Production.Ready ->
      { factory with laboring = Entities.Production.Working 10.0f }
   | Entities.Production.Working w ->
      if w > 0.0f then
         { factory with laboring = Entities.Production.Ready }
      else
         { factory with laboring = Entities.Production.Working(w-dt) }

let UpdateFactories (dt : float32)(spawnNewFactory)(listOfFactories:List<Entities.Factory>) =
   
   let factories = listOfFactories |> foreachDo updateFactory dt
   
   let newFactoriesList =
      if listOfFactories.Length < factoryAmount then
         if spawnNewFactory then
            let xy = TerrainCollection.getRandomTileCoordinates(BitmapProcessing.TerrainType.Land)
            {
               Entities.Factory.coordinatesX = fst xy
               Entities.Factory.coordinatesY = snd xy
               Entities.Factory.productionType = ""//resources.[RNG.Next(0, resources.Length-1)];
               Entities.Factory.laboring = Entities.Production.Working 10.0f
            } :: listOfFactories
         else
            listOfFactories
      else
         listOfFactories
   newFactoriesList

let UpdateTruck (dt:float32) (truck:Entities.Truck) = 
   truck

let deployTruck destinations (factory:Entities.Factory) = 
   factory.SpawnResource(destinations)

let UpdateTrucks(dt : float32)(factories: List<Entities.Factory>)(trucks : List<Entities.Truck>) : Entities.Truck List= 
   let trucks = trucks |> foreachDo UpdateTruck dt

   let isFactoryProductive (f:Entities.Factory) = 
      f.laboring = Entities.Production.Ready
   let productiveFactories = filter isFactoryProductive factories
   let nextShift = foreachDo deployTruck factories factories
//   let jobWellDone (t:Truck) = 
//      if t.route.lastWaypoint().coordinatesX = t.coordinatesX && t.route.lastWaypoint().coordinatesY = t.coordinatesY then
//         false
//      else true
   trucks @ nextShift

let Update(dt : float32)(gameState : GameState) =
   let spawnNewFactory, newSpawnState =
      match gameState.Spawnrate with
      | Ready ->
         true, Cooldown 0.1f
      | Cooldown c->
         if c > 0.0f then
            false, Cooldown(c-dt)
         else
            false, Ready     
   { 
      gameState with Spawnrate   = newSpawnState
                     Factories   = UpdateFactories dt spawnNewFactory gameState.Factories
                     Trucks      = gameState.Trucks
                     Endpoints   = gameState.Endpoints
   }

let drawFactory(factory:Entities.Factory) : Drawable =
  {
    Drawable.Position = Vector2((float32(fst(factory.getPosition()))),(float32(snd (factory.getPosition()))))
    Drawable.Image    = "factory.png"
  }

let drawState (gameState:GameState) =
   let newDrawableFactories = Seq.empty
   gameState.Factories |> Seq.map(fun factory -> drawFactory(factory))

let setBitMap imagePath = 
   let terrains = BitmapProcessing.mapBitMapToTerrain(imagePath)
   TerrainCollection <- {
      LandPixels = snd (terrains |> Seq.find(fun (terrain,sequence) -> terrain = BitmapProcessing.Land))
      WaterPixels = snd (terrains |> Seq.find(fun (terrain,sequence) -> terrain = BitmapProcessing.Water))
      WastelandPixels = snd (terrains |> Seq.find(fun (terrain,sequence) -> terrain = BitmapProcessing.Wasteland))
   }

let setupEngineState() =  
   {
      Factories  = []
      Trucks     = []
      Endpoints  = []
      Spawnrate  = Spawnable.Ready
   }

   