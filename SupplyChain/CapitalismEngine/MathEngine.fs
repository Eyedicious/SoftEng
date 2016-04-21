module MathEngine

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open Entities

let factoryAmount = 2
let hubAmount = 1
let endpointAmount = 1

type Spawnable =
   | Cooldown of float32
   | Ready

type GameState = 
   {
      Factories      : List<Entities.Factory>
      Trucks         : List<Entities.Truck>
      Hubs            : List<Entities.Hub>
      Endpoints      : List<Entities.City>
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

let setupEngineState() =  
   {
      Factories  = []
      Trucks     = []
      Hubs        = []
      Endpoints  = []
      Spawnrate  = Spawnable.Ready
   }

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

let rec foreachDo (fn:'a->'b) (l:List<'a>) : 'b List =     //Foreach x DO fn y x and return the editted list
    match l with
       | [] -> []
       | (x::xs) ->
          (fn x) :: (foreachDo fn xs)

let updateFactory (dt:float32) (factory:Entities.Factory) =
   match factory.laboring with
   |  Entities.Production.Ready ->
      { factory with laboring = Entities.Production.Working 10.0f }
   | Entities.Production.Working w ->
      if not(w > 0.0f) then
         { factory with laboring = Entities.Production.Ready }
      else
         { factory with laboring = Entities.Production.Working(w-dt) }

let UpdateFactories (dt : float32)(spawnNewFactory)(listOfFactories:List<Entities.Factory>) =
   
   let factories = listOfFactories |> foreachDo (updateFactory dt)
   
   let newFactoriesList =
      if factories.Length < factoryAmount then
         if spawnNewFactory then
            let xy = TerrainCollection.getRandomTileCoordinates(BitmapProcessing.TerrainType.Land)
            {
               Entities.Factory.coordinatesX = fst xy |> float32
               Entities.Factory.coordinatesY = snd xy |> float32
               Entities.Factory.productionType = ""//resources.[RNG.Next(0, resources.Length-1)];
               Entities.Factory.laboring = Entities.Production.Working 10.0f
            } :: factories
         else
            factories
      else
         factories
   newFactoriesList

let UpdateTruck (dt:float32) (truck:Entities.Truck) =
   let truck = 
      if truck.route.nextWaypoint().coordinatesX = truck.coordinatesX && truck.route.nextWaypoint().coordinatesY = truck.coordinatesY then
         if not(truck.route.route.Tail.IsEmpty) then
            { truck with route = { truck.route with route = truck.route.route.Tail}}
         else
            truck
      else
         truck
   let xCoefficient = truck.route.nextWaypoint().coordinatesX - truck.coordinatesX
   let yCoefficient = truck.route.nextWaypoint().coordinatesY - truck.coordinatesY
   let isXNegative =
      if xCoefficient < 0.0f then
         -1.0f
      else
          1.0f
   let isYNegative = 
      if yCoefficient < 0.0f then
         -1.0f
      else
         1.0f
 
   let truck = 
      if truck.route.nextWaypoint().coordinatesX <> truck.coordinatesX then
         if truck.route.nextWaypoint().coordinatesY <> truck.coordinatesY then
            if xCoefficient < 0.5f && xCoefficient > -0.5f then
               { truck with coordinatesX = truck.route.nextWaypoint().coordinatesX}
            else
               { truck with coordinatesX = (truck.coordinatesX + (dt * 5.0f * isXNegative))}
         else
            if xCoefficient < 0.5f && xCoefficient > -0.5f then
               { truck with coordinatesX = truck.route.nextWaypoint().coordinatesX}
            else
               { truck with coordinatesX = (truck.coordinatesX + (dt * 10.0f * isXNegative))}
      else
         truck
   let truck = 
      if truck.route.nextWaypoint().coordinatesY <> truck.coordinatesY then
         if truck.route.nextWaypoint().coordinatesX <> truck.coordinatesX then
            if yCoefficient < 0.5f && yCoefficient > -0.5f then
               { truck with coordinatesY = truck.route.nextWaypoint().coordinatesY}
            else
               { truck with coordinatesY = (truck.coordinatesY + (dt * 5.0f * isYNegative))}
         else
            if yCoefficient < 0.5f && yCoefficient > -0.5f then
               { truck with coordinatesY = truck.route.nextWaypoint().coordinatesY}
            else
               { truck with coordinatesY = (truck.coordinatesY + (dt * 10.0f * isYNegative))}
      else
         truck
   truck

let deployTruck hubs destinations (factory:Entities.Factory) = 
   factory.SpawnResource(hubs, destinations)

let moveOn truck = 
   truck

let UpdateTrucks(dt : float32)(factories: List<Entities.Factory>)(hubs : Hub List)(cities : City List)(trucks : List<Entities.Truck>) : Entities.Truck List = 
   let jobWellDone (t:Entities.Truck) = 
      not((t.route.lastWaypoint().coordinatesX = t.coordinatesX) && (t.route.lastWaypoint().coordinatesY = t.coordinatesY))
   let drivingTrucks = filter jobWellDone trucks
   let currentTrucks = drivingTrucks |> foreachDo (UpdateTruck dt)
   let isFactoryProductive (f:Entities.Factory) =
      f.laboring = Entities.Production.Ready
   let productiveFactories = filter isFactoryProductive factories
   if factories.Length > 1 then
      let nextShift = foreachDo (deployTruck hubs cities) productiveFactories
      currentTrucks @ nextShift
   else
      currentTrucks

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
   let newEndpoints = 
      if not(Seq.length gameState.Endpoints > 0) then
         let mutable newEndpoints = []
         for n = 0 to endpointAmount do
            let xy = TerrainCollection.getRandomTileCoordinates(BitmapProcessing.TerrainType.Land)
            newEndpoints <- {
                              Entities.City.coordinatesX = fst xy |> float32
                              Entities.City.coordinatesY = snd xy |> float32
                           } :: newEndpoints
         newEndpoints
      else
         gameState.Endpoints
   let newHubs = 
      if not(Seq.length gameState.Hubs > 0) then
         let mutable newHubs = []
         for n = 0 to hubAmount do
            let xy = TerrainCollection.getRandomTileCoordinates(BitmapProcessing.TerrainType.Land)
            newHubs <- {
               Entities.Hub.coordinatesX = fst xy |> float32
               Entities.Hub.coordinatesY = snd xy |> float32
            } :: newHubs
         newHubs
      else
         gameState.Hubs
   { 
      gameState with Spawnrate   = newSpawnState
                     Factories   = UpdateFactories dt spawnNewFactory gameState.Factories
                     Trucks      = UpdateTrucks dt gameState.Factories gameState.Hubs gameState.Endpoints gameState.Trucks
                     Hubs        = newHubs
                     Endpoints   = newEndpoints
   }

let drawTruck(truck:Entities.Truck) : Drawable =
   {
      Drawable.Position = Vector2((float32(fst(truck.getPosition()))),(float32(snd (truck.getPosition()))))
      Drawable.Image = "truck.png"
   }

let drawFactory(factory:Entities.Factory) : Drawable =
  {
    Drawable.Position = Vector2((float32(fst(factory.position))),(float32(snd (factory.position))))
    Drawable.Image    = "factory.png"
  }

let drawHub(hub:Entities.Hub) : Drawable =
   {
      Drawable.Position = Vector2((float32(fst(hub.position))),(float32(snd (hub.position))))
      Drawable.Image = "hub.png"
   }

let drawCity(city:Entities.City) : Drawable =
  {
    Drawable.Position = Vector2((float32(fst(city.position))),(float32(snd (city.position))))
    Drawable.Image    = "city.png"
  }

let drawState (gameState:GameState) : seq<Drawable> =
      let seqX = Seq.append (gameState.Factories |> foreachDo drawFactory) (gameState.Trucks |> foreachDo drawTruck) 
      let seqY = Seq.append (gameState.Hubs |> foreachDo drawHub) (gameState.Endpoints |> foreachDo drawCity)
      Seq.append seqY seqX

let setBitMap imagePath = 
   let terrains = BitmapProcessing.mapBitMapToTerrain(imagePath)
   TerrainCollection <- {
      LandPixels = snd (terrains |> Seq.find(fun (terrain,sequence) -> terrain = BitmapProcessing.Land))
      WaterPixels = snd (terrains |> Seq.find(fun (terrain,sequence) -> terrain = BitmapProcessing.Water))
      WastelandPixels = snd (terrains |> Seq.find(fun (terrain,sequence) -> terrain = BitmapProcessing.Wasteland))
   }
   