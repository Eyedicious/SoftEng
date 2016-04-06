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
let updateFactory (dt:float32) (factory:Entities.Factory) = 
   

let isFactoryProductive fn (dt:float32) (factories:List<Entities.Factory>) = 
   let rec inspectFactory (factories:List<Entities.Factory>) = function
      | [] -> []
      | ([f]) -> fn dt f
      | (f::fs) -> (fn dt f)::(inspectFactory fs)
   inspectFactory

let UpdateFactories (dt : float32)(spawnNewFactory)(listOfFactories:List<Entities.Factory>) =
   let newFactoriesList =
      if listOfFactories.Length < factoryAmount then
         if spawnNewFactory then
            EntitiesManager.spawnFactory() :: listOfFactories
         else
            listOfFactories
      else
         
         listOfFactories
   newFactoriesList

let UpdateTrucks(dt : float32)(factories: List<Entities.Factory>)(trucks : List<Entities.Truck>) = 
    

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