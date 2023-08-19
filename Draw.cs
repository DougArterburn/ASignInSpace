using System.Drawing;
using System.Collections;

namespace ASignInSpace
{

    internal class Draw
    {
        public int Width
        {
            get;
            set;
        } = 256;

        public int Height
        {
            get;
            set;
        } = 256;

        public bool WriteBinFile
        {
            get;
            set;
        } = false;

        public string PngFilename
        {
            get;
            set;
        } = @$"Default.png";

        public int[] AddressesBlue
        {
            get;
            set;
        } = Array.Empty<int>();

        public int[] AddressesYellow
        {
            get;
            set;
        } = Array.Empty<int>();

        public void DrawStarMap()
        {
            System.Reflection.Assembly assem = System.Reflection.Assembly.GetExecutingAssembly();
            Stream starmapStream = assem.GetManifestResourceStream("ASignInSpace.data17square.bin");
            byte[] starmap = new byte[starmapStream.Length];
            starmapStream.Read(starmap, 0, (int)starmapStream.Length);
            var dataBitsIn = BitArrayHelper.CreateBitArray(starmap);
            DrawBitArray(dataBitsIn);
        }

        public void DrawFile(string filename)
        {
            var dataBytesIn = File.ReadAllBytes(filename);
            var dataBitsIn = BitArrayHelper.CreateBitArray(dataBytesIn);
            DrawBitArray(dataBitsIn);
        }


        public void DrawBitArray(BitArray data)
        {
            int width = (int)Math.Sqrt(data.Length);
            Bitmap dialImage = new Bitmap(Width, Height);
            int x = 0, y = 0;
            Graphics g = Graphics.FromImage(dialImage);
            g.Clear(Color.Black);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i])
                {
                    var brush = Brushes.White;
                    if (AddressesBlue.Contains(i))
                    {
                        brush = Brushes.Blue;
                    }
                    else if (AddressesYellow.Contains(i))
                    {
                        brush = Brushes.Yellow;
                    }
                    g.FillRectangle(brush, x, y, 1, 1);
                }

                x++;
                if (x == dialImage.Width)
                {
                    x = 0;
                    y++;
                }

            }
            if (WriteBinFile)
            {
                File.WriteAllBytes(@$"{Path.GetFileNameWithoutExtension(PngFilename)}.bin", BitArrayHelper.CreateByteArray(data));
            }

            dialImage.Save(PngFilename, System.Drawing.Imaging.ImageFormat.Png);
            int onCount = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i]) onCount++;
            }
           Console.WriteLine($"On count = {onCount}");
        }
    }
}
