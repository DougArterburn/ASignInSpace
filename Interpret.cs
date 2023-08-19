using System.Collections;

namespace ASignInSpace
{
    public class Interpret
    {

        public const string Header64String = "0000011010010000001010000100000101000100100010000100010010001000";
        public const string Footer64String = "1111100000111110000000000000000000000000000000000000000000000000";

        public BitArray AlienProgram
        {
            get;
            set;
        }

        public int FileNumber
        {
            get;
            set;
        } = 0;

        public int MaxIterations
        {
            get;
            set;
        } = 65536;

        public bool TraceExecution
        {
            get;
            set;
        } = false;

        public string FilenamePrefix
        {
            get;
            set;
        } = "OISC";


        /*
         * This is kind of hack at this point in cases where the output impage looks better when skipping initial plot points.
         * 
         * I wonder if these "discarded" initial bits mean anything.
         * 
         * This isn't really necessary - just cosmetic if you want to use it. Hopefully, we'll figure out what these are and deal
         * with them more appropriately.
         * */
        public int BreakImageAtIteration
        {
            get;
            set;
        } = 0;


        public void Run(BitArray AlienProgram)
        {
            const int AddressSize = 4;
            this.AlienProgram = AlienProgram;

            SortedDictionary<string, int> saveExecutionPointer = new SortedDictionary<string, int>();
            BitArray ProgramOutput = new BitArray(65536);
            int iteration = 0;
            int ProgramOutputIndex = 0;
            int executionPointer = 0;
            int flipRelativeAddress;
            string saveState;
            do
            {

                /*
                 * When the program starts repeating, then we are done.
                 * 
                 * There must be a better way to detect END OF PROGRAM!
                 * 
                 * */
                //saveState = BitArrayHelper.ConvertToString(AlienProgram);
                //if (saveExecutionPointer.ContainsKey(saveState))
                //{
                //    if (saveExecutionPointer[saveState] == executionPointer)
                //    {
                //        Console.WriteLine($"Program ended after {iteration} iterations.");
                //        draw();
                //        return;
                //    }

                //}
                //else
                //{
                //    saveExecutionPointer[saveState] = executionPointer;
                //    if (TraceExecution)
                //    {
                //        Console.WriteLine(saveState);
                //    }
                //}

                iteration++;

                flipRelativeAddress = getValueInstruction(executionPointer);
                var flipAbsoluteAddress = flipBit(flipRelativeAddress);
                ProgramOutput[ProgramOutputIndex] = true;
                executionPointer = nextAddressInstruction(executionPointer);


                /*
                 * 
                 * Don't run forever. I have fed this interpreter bit strings that seem to run forever.
                 * 
                 * */
                if (iteration > MaxIterations)
                {
                    Console.WriteLine($"Aborting after {iteration} iterations.");
                    draw();
                    return;
                }

                if (BreakImageAtIteration == iteration)
                {
                    draw();
                }

            } while (true);


            return;

            int increment(int address)
            {
                address++;
                if (address == AlienProgram.Length)
                {
                    return 0;
                }
                return address;
            }

            int getValueInstruction(int relativeAddress)
            {
                int value = 0;
                for (int j = relativeAddress, count = 0; count < AddressSize; j = increment(j), count++)
                {
                    value *= 2;
                    if (AlienProgram[j]) value += 1;
                }
                return value;
            }

            int flipBit(int relativeAddress)
            {
                int j, count;
                for (j = executionPointer, count = 0; count < relativeAddress; j = increment(j), count++) ;
                ProgramOutputIndex = getAddressMap(executionPointer);
                AlienProgram[j] = !AlienProgram[j];
                return j;
            }

            int nextAddressInstruction(int address)
            {
                int nextAddress, count;
                for (nextAddress = address, count = 0; count < AddressSize; nextAddress = increment(nextAddress), count++) ;
                return nextAddress;
            }

            int getAddressMap(int address)
            {
                int value = 0;
                int addr, count;
                for (addr = address, count = 0; count < 16; addr = increment(addr), count++)
                {
                    value *= 2;
                    if (AlienProgram[addr]) value += 1;
                }
                return value;
            }

            void draw()
            {
                FileNumber++;
                var d = new Draw();
                d.Width = 256;
                d.Height = 256;
                d.WriteBinFile = true;
                d.PngFilename = @$"{FilenamePrefix}{FileNumber}.png";
                d.DrawBitArray(ProgramOutput);
                ProgramOutputIndex = 0;
                ProgramOutput.SetAll(false);
                Console.WriteLine(@$"Writing file {FilenamePrefix}{FileNumber}.png after {iteration} iterations.");
            }

        }


    }
}
