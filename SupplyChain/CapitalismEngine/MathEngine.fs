module MathEngine

open Entities
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

let factoryAmount = 10

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

let initialize() = 
  {
    Factories  = []
    Trucks     = []
    Endpoints  = []
    Spawnrate  = Spawnable.Ready
  }
//let LoadContent() =
//   {
//      //
//   }
//
//let UnloadContent() =
//   {
//      //
//   }
//
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

let updateFactory (dt:float32) (factory:Entities.Factory) : Factory =
   match factory.laboring with
   |  Entities.Production.Ready ->
      { factory with laboring = Entities.Production.Working 10.0f }
   | Entities.Production.Working w ->
      if w < 0.0f then
         { factory with laboring = Entities.Production.Ready }
      else
         { factory with laboring = Entities.Production.Working(w-dt) }

let UpdateFactories (dt : float32)(spawnNewFactory)(listOfFactories:List<Entities.Factory>) =
   
   let factories = listOfFactories |> foreachDo (updateFactory dt)

   let newFactoriesList =
      if listOfFactories.Length < factoryAmount then
         if spawnNewFactory then
            EntitiesManager.spawnFactory() :: factories
         else
            factories
      else
         factories
   newFactoriesList

let UpdateTruck (dt:float32) (truck:Entities.Truck) =
   let xCoefficient = truck.route.lastWaypoint().coordinatesX - truck.coordinatesX
   let yCoefficient = truck.route.lastWaypoint().coordinatesY - truck.coordinatesY
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
      if truck.route.lastWaypoint().coordinatesX <> truck.coordinatesX then
         if truck.route.lastWaypoint().coordinatesY <> truck.coordinatesY then
            if xCoefficient < 5.0f && xCoefficient > 5.0f then
               { truck with coordinatesX = truck.route.lastWaypoint().coordinatesX}
            else
               { truck with coordinatesX = (truck.coordinatesX + (dt * 5.0f * isXNegative))}
         else
            if xCoefficient < 10.0f && xCoefficient > 10.0f then
               { truck with coordinatesX = truck.route.lastWaypoint().coordinatesX}
            else
               { truck with coordinatesX = (truck.coordinatesX + (dt * 10.0f * isXNegative))}
      else
         truck
   let truck = 
      if truck.route.lastWaypoint().coordinatesY <> truck.coordinatesY then
         if truck.route.lastWaypoint().coordinatesX <> truck.coordinatesX then
            if yCoefficient < 5.0f && yCoefficient > 5.0f then
               { truck with coordinatesY = truck.route.lastWaypoint().coordinatesY}
            else
               { truck with coordinatesY = (truck.coordinatesY + (dt * 5.0f * isXNegative))}
         else
            if yCoefficient < 10.0f && yCoefficient > 10.0f then
               { truck with coordinatesY = truck.route.lastWaypoint().coordinatesY}
            else
               { truck with coordinatesY = (truck.coordinatesY + (dt * 10.0f * isXNegative))}
      else
         truck
   truck

let deployTruck destinations (factory:Factory) = 
   let isSamePlace location = 
      not(factory = location)
   let newDestinations = filter isSamePlace destinations
   factory.SpawnResource(newDestinations)

let UpdateTrucks(dt : float32)(factories: List<Entities.Factory>)(trucks : List<Entities.Truck>) : Truck List = 
   let jobWellDone t = 
      if (t.route.lastWaypoint().coordinatesX = t.coordinatesX) && t.route.lastWaypoint().coordinatesY = t.coordinatesY then
         false
      else true
   let currentTrucks = filter jobWellDone trucks
   let currentTrucks = trucks |> foreachDo (UpdateTruck dt)

   let isFactoryProductive (f:Entities.Factory) = 
      f.laboring = Production.Ready
   let productiveFactories = filter isFactoryProductive factories
   if factories.Length > 1 then
      let nextShift = foreachDo (deployTruck factories) productiveFactories
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
   { 
      gameState with Spawnrate   = newSpawnState
                     Factories   = UpdateFactories dt spawnNewFactory gameState.Factories
                     Trucks      = UpdateTrucks dt gameState.Factories gameState.Trucks
                     Endpoints   = gameState.Endpoints
   }

let drawFactory(factory:Entities.Factory) : Drawable =
  {
    Drawable.Position = Vector2((float32(fst(factory.getPosition()))),(float32(snd (factory.getPosition()))))
    Drawable.Image    = "factory-11.png"
  }

let drawState (gameState:GameState) : seq<Drawable> =
   let newDrawableFactories = Seq.empty
//   Terrible recursive attempt to do the map function FUCK MIDNIGHT PROGRAMMING. ps. gotta fix dis
//   let rec loop (acc:List<Entities.Factory>) = function
//      | [] -> List.rev acc
//      | head::tail -> drawFactory(head) :: newDrawableFactories loop tail
//   []
   let newList = gameState.Factories |> List.map(fun factory -> drawFactory(factory))
   newList |> Seq.cast