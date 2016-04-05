module CanvasBuilder

//
//open System.Drawing
//open System.Windows.Forms
//
//let canvasSize = (1024,720)
//let buttonPos = (50, 50)
//type Canvas(keepAlive: bool) =
//   let _alive = keepAlive
//
//   let form = 
//      let temp = new Format()
//      do temp.ClientSize <- new Size(fst canvasSize, snd canvasSize)
//      do temp.FormBorderStyle <- FormBorderStyle.FixedSingle
//      do temp.MaximizeBox <- false
//      do temp.MinimizeBox <- false
//      let resizedImage = 
//         let image = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory+"Graphical Interface\Images\BackgroundMap.jpg")
//         let scale = if image.Size.Width > image.Size.Height then ((float(fst canvasSize)) / (float image.Size.Width))
//                     else ((float(snd canvasSize)) / (float image.Size.Height))
//         let newImage = new System.Drawing.Bitmap(image, (int ((float image.Size.Width) * scale)), (int ((float image.Size.Height) * scale)))
//         newImage
//      do temp.BackgroundImage <- resizedImage
//      do temp.BackgroundImageLayout <- ImageLayout.Center
//      //System.Drawing.Bitmap(loadedimage) <- DONT TOUCH, REALLY, DONT EVEN THINK ABOUT IT.
//      temp
//
//   let button = 
//      let temp = new Button()
//      temp.Text <- "test"
//      temp.Location <- new Point(fst buttonPos, snd buttonPos)
//      printfn "Width:%i and Height:%i" form.ClientRectangle.Width form.ClientRectangle.Height
//      printfn "Width/2:%f and Height/2:%f" (float form.ClientRectangle.Width / 2.0) (float form.ClientRectangle.Height / 2.0) 
//      temp
//
//   do form.Controls.Add(button)
//
//   member this.ExitApplication(killOrder: bool) =
//      _alive = killOrder
//
//   member this.getForm = form
//
//   //member this.spawnObject(object: Factory) =
//      //Plant on map
