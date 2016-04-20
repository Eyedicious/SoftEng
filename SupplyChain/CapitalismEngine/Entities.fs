module Entities

open Microsoft.Xna.Framework

let rec last (x :'a List) = 
   match x with
      | [i] -> i
      | (i::ls) -> last ls

type Waypoint = 
    {
        coordinatesX : int
        coordinatesY : int
    }
    with
      member w.Create(x, y) = 
         {
            coordinatesX = x
            coordinatesY = y
         }

type Route = 
    {
        route : List<Waypoint>
    }
    with
      member r.Create(waypoints : List<Waypoint>) =
         {
            route = waypoints
         }
      member r.lastWaypoint() =
         last r.route
      member r.nextWaypoint() =
         r.route.Head 
         //add Take last waypoint method
         //add take next waypoint method
type Truck = 
    {
        coordinatesX : int
        coordinatesY : int
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
     //member t.Move(x, y) =
        //let add cv v = cv := v
        //add t.coordinatesX x
        //add t.coordinatesY y
     
     override this.ToString() = "Factory is located at ( "+this.coordinatesX.ToString()+", "+this.coordinatesY.ToString()+" ) 
                                 and producing "+this.contentContainer+")"

type Endpoint = 
    {
        coordinatesX : int
        coordinatesY : int
    }
    with
     static member Create(x, y) = 
        {
            coordinatesX = x
            coordinatesY = y
        }

     override this.ToString() = "Endpoint is located at ( "+this.coordinatesX.ToString()+", "+this.coordinatesY.ToString()+")"


let resources = ["Grain"; "Copper"; "Iron"; "Chair"; "Coffee"; "Potatoes"; "Goo"; "Pizza"; "Math"; "Wood"; "Uranium"; "Nuclear Warhead"; "Sugar"; "Wealthy spoiled bratty kids"; "Taco Bell"; "Gold"]
let RNG = new System.Random()
type Production = 
      | Working of float32
      | Ready

//let image = image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory+"\Graphical Interface\Images\factory-1.jpg")

type Factory = 
   {
      coordinatesX : int
      coordinatesY : int
      productionType : string
      laboring : Production
   }
   with
   member f.SpawnResource(destinations : List<Factory>) = 
      let random = new System.Random()
      let rnmbr = random.Next(destinations.Length)
      let destinationArray = destinations |> List.toArray
      let destination = destinationArray.[rnmbr]
      let waypoints = {Waypoint.coordinatesX = fst (destination.getPosition()); Waypoint.coordinatesY = snd (destination.getPosition())} :: []
      let newroute = {Route.route = waypoints}
      let newTruck = {Truck.coordinatesX = f.coordinatesX; Truck.coordinatesY = f.coordinatesY; Truck.contentContainer = f.productionType; Truck.route = newroute}
      newTruck
   member f.getPosition() = (f.coordinatesX, f.coordinatesY)
   member f.getProductivity() = f.laboring
   override this.ToString() = "Factory is located at ( "+(this.coordinatesX).ToString()+", "+(this.coordinatesY).ToString()+" ) 
                                 and producing "+this.productionType+")"