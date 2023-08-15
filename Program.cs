// See https://aka.ms/new-console-template for more information
using ASignInSpace;

Console.WriteLine("Hello, World!");


/*
 * Here are some calls to the interpreter that produce an image of symbols.
 * Note that I haven't found any clearly defined symbols in the 64 bit header.
 * */

var prog = new Interpret();
prog.BreakImageAtIteration = 942;
prog.Run(BitArrayHelper.CreateBitArray(Interpret.Footer64String));
prog.BreakImageAtIteration = 949;
prog.Run(BitArrayHelper.CreateBitArray(Interpret.Footer64String, reverse: true));

return;

