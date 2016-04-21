module BitmapProcessing

open System
open System.Drawing
open System.IO

type TerrainType = 
  | Land
  | Water
  | Wasteland
  | ProvinceBorder

//:?> = Converts a type to a type that is lower in the hierarchy. We do this for Bitmap since a Bitmap is derived from image which is higher in the hierarchy

//Solution to the current BitMap lil flunks is by shifting focus from colors to the HSV values where H > 150 = Water else H < 100 && S < 20 = Border, S < 20 = Wasteland Else Land.

let ColorToHSV(color : Color) =
   let max = Math.Max(color.R, Math.Max(color.G, color.B)) |> float32
   let min = Math.Min(color.R, Math.Min(color.G, color.B)) |> float32
   let hue = color.GetHue()
   let saturation = if (max = 0.0f) then max else 1.0f - (1.0f * min / max)
   let value = max / 255.0f
   [hue;saturation *100.0f ;value *100.0f ]

let assignTerrainType (pixelColor : Color) =
   let HSV = ColorToHSV(pixelColor)
   let WithinRange (c1:byte) (c2:byte) = Math.Abs(c1.CompareTo(c2)) < 14
   let WithinRangeX (c1:byte) (c2:byte) (difference:int) = Math.Abs(c1.CompareTo(c2)) < difference
   let isLimited (c1:byte) = c1 < 10uy
   let isBlueWithinRangeOfGreen = WithinRangeX pixelColor.G pixelColor.B 20 //Bigger amount
   let isRedWithinRangeOfGreen =  WithinRange pixelColor.R pixelColor.G
   let isBorder = pixelColor.R |> isLimited && pixelColor.G |> isLimited

   if HSV.Item(0) > 150.0f then Water 
      elif HSV.Item(0) > 100.0f && HSV.Item(1) > 10.0f || HSV.Item(2) < 70.0f then Land //Filter obvious pieces of Land and pixels with ugly border line to be land
         elif HSV.Item(0) > 50.0f && HSV.Item(1) > 12.0f then Land // For land pieces with a low saturation (polders n such!)
            elif HSV.Item(0) > 30.0f && HSV.Item(1) > 18.0f then Land 
               //elif HSV.Item(1) < 20.0f then ProvinceBorder 
               else Wasteland

let setColor(tType:TerrainType) =
   match tType with
   | TerrainType.Land ->
      Color.Green   
   | TerrainType.Water -> 
      Color.Blue
   | TerrainType.Wasteland -> 
      Color.DarkGray
   | TerrainType.ProvinceBorder ->
      Color.Black


let getColorPerPixel (bmp:Bitmap) = 
    seq {for x in 0..bmp.Width-1 do 
            for y in 0..bmp.Height-1 do 
               yield x, y, bmp.GetPixel(x, y) |> assignTerrainType}

let mapBitMapToTerrain file =
   let bmp = Bitmap.FromFile(file) :?> Bitmap
   getColorPerPixel bmp |> Seq.groupBy (fun (x, y ,terrainType) -> 
                           terrainType) |> Seq.map(fun (terrainType, sequence) -> 
                              ( terrainType, sequence |> Seq.map(fun (x, y, _) -> (x,y))))