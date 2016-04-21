module Entities

open Microsoft.Xna.Framework

let rec last (x :seq<'a>) = 
    x |> Seq.toArray |> Array.rev |> Array.get <| 0

type Waypoint = 
    {
        coordinatesX : float32
        coordinatesY : float32
    }
    with
      member w.Create(x, y) = 
         {
            coordinatesX = x
            coordinatesY = y
         }

type Route = 
    {
        route : seq<Waypoint>
    }
    with
      member r.Create(waypoints : List<Waypoint>) =
         {
            route = waypoints
         }
      member r.lastWaypoint() =
         last r.route
      member r.nextWaypoint() =
          r.route |> Seq.take 1
         //add Take last waypoint method
         //add take next waypoint method
type Truck = 
    {
        coordinatesX : float32
        coordinatesY : float32
        contentContainer : string
        route : Route
    }
    with
     member t.Create(x, y, content, route) = 
        {
            coordinatesX = x
            coordinatesY = y
            contentContainer = content
            route = route
        }
     member t.getPosition() = 
      (t.coordinatesX, t.coordinatesY)
     
     override this.ToString() = "Truck is located at ( "+this.coordinatesX.ToString()+", "+this.coordinatesY.ToString()+" ) 
                                 and carrying "+this.contentContainer+")"

type Hub = 
    {
        coordinatesX : int
        coordinatesY : int
    }
    with
     member h.position with get() = (h.coordinatesX, h.coordinatesY)
     override this.ToString() = "Hub is located at ( "+this.coordinatesX.ToString()+", "+this.coordinatesY.ToString()+")"

type City = 
    {
        coordinatesX : int
        coordinatesY : int
    }
    with    
     member c.position with get() = (c.coordinatesX, c.coordinatesY)
     override this.ToString() = "Hub is located at ( "+this.coordinatesX.ToString()+", "+this.coordinatesY.ToString()+")"


let resources = ["Grain"; "Copper"; "Iron"; "Chair"; "Coffee"; "Potatoes"; "Goo"; "Pizza"; "Math"; "Wood"; "Uranium"; "Nuclear Warhead"; "Sugar"; "Wealthy spoiled bratty kids"; "Taco Bell"; "Gold"]
let RNG = new System.Random()
type Production = 
      | Working of float32
      | Ready

type Factory = 
   {
      coordinatesX : float32
      coordinatesY : float32
      productionType : string
      mutable laboring : Production
   }
   with
   member f.SpawnResource(destinations : List<Hub>) = 
      let random = new System.Random()
      let rnmbr = random.Next(destinations.Length)
      let destinationArray = destinations |> List.toArray
      let destination = destinationArray.[rnmbr]
      let waypoints = seq{ yield {  Waypoint.coordinatesX = fst (destination.position) |> float32; 
                                    Waypoint.coordinatesY = snd (destination.position) |> float32 } }
      let newroute = {Route.route = waypoints}
      let newTruck = {  Truck.coordinatesX = f.coordinatesX; 
                        Truck.coordinatesY = f.coordinatesY; 
                        Truck.contentContainer = f.productionType; 
                        Truck.route = newroute }
      newTruck
   member f.position with get() = (f.coordinatesX, f.coordinatesY)
   member f.productionCycle with get() = f.laboring and set(value) = f.laboring <- value
   override this.ToString() = "Factory is located at ( "+(this.coordinatesX).ToString()+", "+(this.coordinatesY).ToString()+" ) 
                                 and producing "+this.productionType+")"