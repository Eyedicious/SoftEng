module Endpoint

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