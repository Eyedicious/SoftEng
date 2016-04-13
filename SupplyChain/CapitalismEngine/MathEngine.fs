module MathEngine

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
let doCheck (fn:'a->'b->'c) (y:'a) (x:List<'b>) =     //Foreach x DO fn y x and return the editted list
   let rec inspectX (x:List<'b>) = 
      match x with
         | [] -> []
         | ([x]) -> fn y x :: []
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
   
   let factories = listOfFactories |> doCheck updateFactory dt
   
   let newFactoriesList =
      if listOfFactories.Length < factoryAmount then
         if spawnNewFactory then
            EntitiesManager.spawnFactory() :: listOfFactories
         else
            listOfFactories
      else
         listOfFactories
   newFactoriesList

let UpdateTruck (dt:float32) (truck:Entities.Truck) = 
   truck

let UpdateTrucks(dt : float32)(factories: List<Entities.Factory>)(trucks : List<Entities.Truck>) = 
    let trucks = trucks |> doCheck UpdateTruck dt

    let newTrucks = 
      

    trucks

let Update(dt : float32)(gameState : GameState) =
   let spawnNewFactory, newSpawnState =
      match gameState.Spawnrate with
      | Ready ->
         true, Cooldown 0.0f
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