module Entities

open Microsoft.Xna.Framework

type Truck = 
    {
        coordinatesX : int ref
        coordinatesY : int ref
        contentContainer : string
    }
    with
     static member Create(x, y, content) = 
        {
            coordinatesX = x
            coordinatesY = y
            contentContainer = content
        }
     member t.Move(x, y) =
        let add cv v = cv := v
        add t.coordinatesX x
        add t.coordinatesY y
     
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


let resources = ["Grain", "Copper", "Iron", "Chair", "Coffee", "Potatoes", "Goo", "Pizza", "Math", "Wood", "Uranium", "Nuclear Warhead", "Sugar", "Wealthy spoiled bratty kids", "Taco Bell", "Gold"]
let RNG = System.Random().Next(0, resources.Length-1);
let getResource = resources.Item(RNG)

//let image = image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory+"\Graphical Interface\Images\factory-1.jpg")

type Factory(x, y) = 
   let coordinates = (x, y)
   let productionType = resources.Item(System.Random().Next(0, resources.Length)) |> string
   
   member f.SpawnResource() = 
      Truck.Create(ref (fst coordinates), ref (snd coordinates), productionType)
   member f.getPosition() = coordinates 
   override this.ToString() = "Factory is located at ( "+(fst coordinates).ToString()+", "+(snd coordinates).ToString()+" ) 
                                 and producing "+productionType+")"


