// See https://aka.ms/new-console-template for more information
using ASignInSpace;
using System.Collections;

//RunFooter();
//RunTheStarmap();

//BitArray step2 = BitArrayHelper.CreateBitArray(File.ReadAllBytes(@"C:\ASignInSpace\Source\AlienInterpreter\bin\Debug\net6.0-windows\OISC1Save.bin"));
//BitArray step1 = GetStarmap();
//var prog = new Interpret();
//prog.FilenamePrefix = "StarMapStep1-";
//prog.MaxIterations = 65536/4;
//prog.Run(step1);

BitArray step2 = BitArrayHelper.CreateBitArray(File.ReadAllBytes("StarMapStep1-1.bin"));
var prog = new Interpret();
prog.FilenamePrefix = "StarMapStep2-";
prog.MaxIterations = 65536;
prog.Run(step2);


return;

void RunFooter()
{
    /*
 * Here are some calls to the interpreter that produce an image of symbols.
 * Note that I haven't found any clearly defined symbols in the 64 bit header.
 * */

    var prog = new Interpret();
    //prog.BreakImageAtIteration = 942;
    prog.Run(BitArrayHelper.CreateBitArray(Interpret.Footer64String));
    //prog.BreakImageAtIteration = 949;
    //prog.Run(BitArrayHelper.CreateBitArray(Interpret.Footer64String, reverse: true));
}
void RunTheStarmap()
{

    /*
     * These calls run the Starmap through the interpreter, 64 bits at a time.
     * */
    var starMap = GetStarmap();
    var AlienProgram = new BitArray(64);
    var prog = new Interpret();
    for (int blocks = 0; blocks < starMap.Length / 64; blocks++)
    {
        int zeroCount = 0;
        for (int i = 0; i < 64; i++)
        {
            AlienProgram[i] = starMap[i + blocks * 64];
            if (!AlienProgram[i]) zeroCount++;
        }
        if (zeroCount == 64) continue;
        prog.FileNumber = blocks;
        prog.Run(AlienProgram);
    }
}

BitArray GetStarmap()
{
    System.Reflection.Assembly assem = System.Reflection.Assembly.GetExecutingAssembly();
    Stream starmapStream = assem.GetManifestResourceStream("ASignInSpace.data17square.bin");
    byte[] starmap = new byte[starmapStream.Length];
    starmapStream.Read(starmap, 0, (int)starmapStream.Length);
    var dataBitsIn = BitArrayHelper.CreateBitArray(starmap);
    return dataBitsIn;
}
