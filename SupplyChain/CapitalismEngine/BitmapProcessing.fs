module BitmapProcessing

open System
open System.Drawing
open System.IO

type TerrainType = 
  | Land
  | Water
  | Wasteland

//:?> = Converts a type to a type that is lower in the hierarchy. We do this for Bitmap since a Bitmap is derived from image which is higher in the hierarchy
 
let loadAndScale file =
   Bitmap.FromFile(file) :?> Bitmap
   //bmp.GetThumbnailImage(800, 600, (fun () -> true), IntPtr.Zero) :?> Bitmap

let assignTerrainType (pixelColor : Color) =
   let WithinRange (c1:byte) (c2:byte) = Math.Abs(c1.CompareTo(c2)) < 20
   let isBlueWithinRangeOfGreen = pixelColor.B |> WithinRange pixelColor.G
   let isRedWithinRangeOfGreen =  pixelColor.R |> WithinRange pixelColor.G
   
   if isBlueWithinRangeOfGreen && isRedWithinRangeOfGreen then Wasteland elif pixelColor.B.CompareTo(pixelColor.G) >= 0 then Water else Land

let getColorPerPixel (bmp:Bitmap) = 
    let toRgb (c:Color) = c.R |> int, c.G |> int, c.B |> int
    seq {for x in 0..bmp.Width-1 do for y in 0..bmp.Height-1 do yield x, y, bmp.GetPixel(x, y) |> assignTerrainType}

let getTerrainMapping (bmp:Bitmap) =
    getColorPerPixel bmp |> Seq.groupBy (fun (x, y ,terrainType) -> terrainType)  |> Seq.map(fun (terrainType, sequence) -> ( terrainType, sequence |> Seq.map(fun (x, y, _) -> (x,y))))
