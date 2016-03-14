module Factory

open Truck

let resources = ["Grain", "Copper", "Iron", "Chair", "Coffee", "Potatoes", "Goo", "Pizza", "Math", "Wood", "Uranium", "Nuclear Warhead"]

type Factory = 
    {
        coordinatesX : int
        coordinatesY : int
        productionType : string
    }
    with
     static member Create(x, y) = 
        {
            coordinatesX = x
            coordinatesY = y
            productionType = resources.[System.Random().Next(0, resources.Length)] |> string
        }
     member f.SpawnResource() = 
        Truck.Create(ref f.coordinatesX, ref f.coordinatesY, f.productionType)


     override this.ToString() = "Factory is located at ( "+this.coordinatesX.ToString()+", "+this.coordinatesY.ToString()+" ) and producing "+this.productionType+")"