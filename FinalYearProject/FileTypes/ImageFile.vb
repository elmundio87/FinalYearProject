Imports System.IO
Imports System.Drawing.Imaging
Imports System.Threading
Imports System.Runtime.InteropServices.Marshal


Public Class ImageFile
    Inherits File

    Dim x As WorkerThread
    Dim y As WorkerThread
    Dim bmp As Bitmap
    Dim bmp1, bmp2 As Bitmap

    Public Sub New(ByVal b As Byte())
        MyBase.new(b)
    End Sub

    Public Overrides Sub FilterData(ByVal f As settings)

        Dim stream As System.IO.MemoryStream
        Dim img As Image
     
        'create an image object from the file information
        stream = New System.IO.MemoryStream(data)
        img = Image.FromStream(stream)


        'Create a new FrameDimension object from this image
        Dim FrameDimensions As System.Drawing.Imaging.FrameDimension = New System.Drawing.Imaging.FrameDimension(img.FrameDimensionsList(0))

        If img.GetFrameCount(FrameDimensions) = 1 Or f.getAnimation = True Then 'if there is only one frame, or "Static images" option has been enabled

            Dim bmp As Bitmap = New Bitmap(img) 'put the image info into a bitmap

            'determine sizes for reduced image quality
            Dim sr As Rectangle = New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim dr As Rectangle

            If sr.Width > 4 And sr.Height > 4 And f.getQuality = True Then
                'reduce the image size, to decrease the amount of processing needed
                If sr.Width > 500 Then
                    dr = New Rectangle(0, 0, bmp.Width / 4, bmp.Height / 4)
                Else 'more reduction for larger images
                    dr = New Rectangle(0, 0, bmp.Width / 2, bmp.Height / 2)
                End If

                bmp = New Bitmap(bmp, dr.Width, dr.Height) 'shrink the image if imagequality=low
            End If

                If f.getMultithreading() Then
                bmp = ThreadManager(bmp, f) 'ise two threads to change the image
                Else
                    Dim worker As WorkerThread = New WorkerThread(bmp, f, 1)
                bmp = worker.manipulatepixels2(bmp, f) 'use only one thread
                End If

                If f.getQuality = True Then
                    bmp = New Bitmap(bmp, sr.Width, sr.Height) 'turn the image to its original size
                End If

                Using ms As New MemoryStream()
                bmp.Save(ms, ImageFormat.Png) 'turn bitmap back into an image file
                    filteredData = ms.ToArray()
                End Using

            img.Dispose() 'memory management
                bmp.Dispose()

        Else 'if an animated file

            'Variable declaration

            Dim memoryStream As MemoryStream
            Dim xstream As MemoryStream = New MemoryStream
            Dim delaypointer As Integer = 0
            Dim binaryWriter As BinaryWriter
            Dim imagee As Bitmap
            Dim buf1 As Byte()
            Dim buf2 As Byte()
            Dim buf3 As Byte()
            'Variable declaration

            memoryStream = New MemoryStream()
            buf2 = New Byte(18) {}
            buf3 = New Byte(7) {}
            buf2(0) = 33            'extension introducer
            buf2(1) = 255            'application extension
            buf2(2) = 11            'size of block
            buf2(3) = 78            'N
            buf2(4) = 69            'E
            buf2(5) = 84            'T
            buf2(6) = 83            'S
            buf2(7) = 67            'C
            buf2(8) = 65            'A
            buf2(9) = 80            'P
            buf2(10) = 69            'E
            buf2(11) = 50            '2
            buf2(12) = 46            '.
            buf2(13) = 48            '0
            buf2(14) = 3            'Size of block
            buf2(15) = 1            '
            buf2(16) = 0            '
            buf2(17) = 0            '
            buf2(18) = 0            'Block terminator
            buf3(0) = 33            'Extension introducer
            buf3(1) = 249            'Graphic control extension
            buf3(2) = 4            'Size of block
            buf3(3) = 9            'Flags: reserved, disposal method, user input, transparent color
            buf3(4) = 100          'Delay time low byte
            buf3(5) = 0            'Delay time high byte
            buf3(6) = 255          'Transparent color index
            buf3(7) = 0            'Block terminator

            
            binaryWriter = New BinaryWriter(xstream)

            For picCount As Integer = 0 To img.GetFrameCount(FrameDimensions) - 1
                img.SelectActiveFrame(FrameDimensions, picCount)
                imagee = New Bitmap(img) 'get each frame in the image as a bitmap

                Dim bmp As Bitmap = New Bitmap(imagee)

                If f.getMultithreading() Then
                    bmp = ThreadManager(bmp, f) 'filter the frame on 2 threads
                Else
                    Dim worker As WorkerThread = New WorkerThread(bmp, f, 1)
                    bmp = worker.manipulatepixels2(bmp, f) 'filter on one thread
                End If

                bmp.Save(memoryStream, ImageFormat.Gif) 'add the bitmap to the GIF file creation memory stream
                buf1 = memoryStream.ToArray()

                If picCount = 0 Then
                    'only write these the first time....
                    binaryWriter.Write(buf1, 0, 781) 'Header & global color table
                    binaryWriter.Write(buf2, 0, 19) 'Application extension
                End If

                'set the frame delay based on the original file's delay property, stored as an array in propertyID &H5100
                buf3(4) = (img.GetPropertyItem(&H5100).Value(delaypointer) Mod 256)
                buf3(5) = (img.GetPropertyItem(&H5100).Value(delaypointer) / 256)

                delaypointer = delaypointer + 4 'the delay for each frame is stored in every 4 values in the property list

                binaryWriter.Write(buf3, 0, 8) 'Graphic extension
                binaryWriter.Write(buf1, 789, buf1.Length - 790) 'Image data

                If picCount = img.GetFrameCount(FrameDimensions) - 1 Then
                    'only write this one the last time....
                    'Image terminator
                    binaryWriter.Write(";")
                End If

                memoryStream.SetLength(0) 'flush memorystream
            Next


            filteredData = xstream.ToArray()
            xstream.SetLength(0) 'flush memorystream
            End If
    End Sub

    Private Function ThreadManager(ByVal pbmp As Bitmap, ByVal f As settings)
        bmp = pbmp

        'determine the size that 2 images would be if they were the 2 halves of the original
        Dim size As Drawing.Rectangle = New Rectangle(0, 0, bmp.Width, Math.Round(bmp.Height / 2, 0, MidpointRounding.AwayFromZero))
        Dim size2 As Drawing.Rectangle = New Rectangle(0, size.Height, bmp.Width, bmp.Height - size.Height)
   
        bmp1 = New Bitmap(size.Width, size.Height) 'make a bitmap to hold the first half
        If size2.Height > 0 Then
            bmp2 = New Bitmap(size2.Width, size2.Height) 'make the second half
        End If

        'draw the image halves onto the blank bitmaps
        Dim GFX As Graphics = Graphics.FromImage(bmp1)
        GFX.DrawImage(pbmp, 0, 0, size, GraphicsUnit.Pixel)

        If size2.Height > 0 Then
            GFX = Graphics.FromImage(bmp2)
            GFX.DrawImage(pbmp, 0, 0, size2, GraphicsUnit.Pixel)
        End If


        Dim x As WorkerThread = New WorkerThread(bmp1, f, 1)
        Dim y As WorkerThread = New WorkerThread(bmp2, f, 2)

        Dim t1 As New Thread(AddressOf x.run)
        Dim t2 As New Thread(AddressOf y.run)

        t1.Start()
        If size2.Height > 0 Then
            t2.Start()
        End If

        'wait for both threads to finish
        t1.Join()

        If size2.Height > 0 Then
            If t2.IsAlive() Then
                t2.Join()
            End If
        End If


        If size2.Height > 0 Then
            'join the images back together
            Dim width As Integer = Math.Max(bmp1.Width, bmp2.Width)
            Dim height As Integer = bmp2.Height + bmp1.Height
            Dim fullBmp As Bitmap = New Bitmap(width, height)
            Dim gr As Graphics = Graphics.FromImage(fullBmp)
            gr.DrawImage(bmp1, 0, 0, bmp1.Width, bmp1.Height)
            gr.DrawImage(bmp2, 0, fullBmp.Height - bmp1.Height, bmp2.Width, bmp2.Height)

            'Console.WriteLine("ThreadID: 0 Join complete")
            Return fullBmp
        Else
            Return bmp1
        End If


    End Function

End Class

Class WorkerThread
    Public ID As Integer
    Public bmp As Bitmap
    Public settings As settings

    Public Sub New(ByVal pbmp As Bitmap, ByVal psettings As settings, ByVal pID As Integer)
        bmp = pbmp
        settings = psettings
        ID = pID

    End Sub

    Public Sub run()
        'Console.WriteLine("ThreadID:" & ID & "processing started")
        bmp = manipulatepixels2(bmp, settings)

        'Console.WriteLine("ThreadID:" & ID & "processing finished")
    End Sub

    Public Function manipulatePixels(ByVal bmp As Bitmap, ByVal f As settings)
        Dim a, r, g, b As Integer
        Dim filter As Filter = f.getFilter
        For x As Integer = 0 To bmp.Width - 1
            For y As Integer = 0 To bmp.Height - 1 'for all pixels
                a = bmp.GetPixel(x, y).A
                r = bmp.GetPixel(x, y).R
                g = bmp.GetPixel(x, y).G
                b = bmp.GetPixel(x, y).B

                Dim tempcol As Color = Color.FromArgb(a, r, g, b)
                Try
                    'filter the selected pixel value
                    tempcol = filter.filter(tempcol)
                    bmp.SetPixel(x, y, tempcol)
                Catch
                    'much slower than lockbits
                End Try
            Next
        Next
        filter.Flush()
        Return bmp
    End Function

    Public Function manipulatepixels2(ByVal b As Bitmap, ByVal f As settings)



        ' 32 bit per pixel
        ' 1 byte per color component per pixel (A, R, G, B).
        ' The lockbits method locks the bitmap in memory and it won't be garbage collected until unlocked
        ' Using a pointer called scan0 that is the address of the start of the data. 
        ' The first 4 bytes are the color of the top left pixel in the bitmap 

        Dim bmd As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), _
        System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        Dim scan0 As IntPtr = bmd.Scan0
        Dim stride As Integer = bmd.Stride
        Dim alpha, red, green, blue As Integer
        Dim filter As Filter = f.getFilter

        ' define an array to store each pixels color as an integer 
        Dim pixels((b.Width * b.Height) - 1) As Integer

        'system.runtime.interopservices.marshall.copy
        Copy(scan0, pixels, 0, pixels.Length)

        ' loop through all pixels and filter
        For i As Integer = 0 To pixels.Length - 1

            alpha = (pixels(i) >> 24) And &HFF
            red = (pixels(i) >> 16) And &HFF
            green = (pixels(i) >> 8) And &HFF
            blue = pixels(i) And &HFF

            ' Here we just run the colour values through the filter


            Dim tempcol As Color = Color.FromArgb(alpha, red, green, blue)

            tempcol = filter.filter(tempcol)


            red = tempcol.R
            green = tempcol.G
            blue = tempcol.B

            red = Math.Max(0, red)   ' get 0 or red whichever is bigger
            red = Math.Min(255, red) ' get 255 or red whichever is smaller
            ' no values < 0 or > 255

            green = Math.Max(0, green)
            green = Math.Min(255, green)

            blue = Math.Max(0, blue)
            blue = Math.Min(255, blue)

            pixels(i) = (alpha << 24) Or (red << 16) Or (green << 8) Or blue

        Next

        'copy the data back from the array to the locked memory
        Copy(pixels, 0, scan0, pixels.Length)

        'unlock the bits and flush the filter to save memory
        b.UnlockBits(bmd)
        filter.Flush()

        Return b

    End Function
End Class