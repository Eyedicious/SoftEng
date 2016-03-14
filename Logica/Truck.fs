module Truck

let add cx x = cx := x

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
        add t.coordinatesX x
        add t.coordinatesY y
     
     override this.ToString() = "Factory is located at ( "+this.coordinatesX.ToString()+", "+this.coordinatesY.ToString()+" ) and producing "+this.productionType+")"