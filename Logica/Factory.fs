module Factory

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

     override this.ToString() = "Factory is located at ( "+this.coordinatesX.ToString()+", "+this.coordinatesY.ToString()+" ) and producing "+this.productionType+")"