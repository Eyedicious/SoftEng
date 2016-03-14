open System
open System.Drawing
open System.Windows.Forms

let button = 
    let temp = new Button()
    temp.Text <- "test"
    temp.Location <- new Drawing.Point(50, 50)
    temp

let form = 
    let temp = new Form()
    do temp.ClientSize <- new System.Drawing.Size(100,100)
    do temp.Controls.Add(button)
    temp


[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    do Application.Run(form)
    0 // return an integer exit code
