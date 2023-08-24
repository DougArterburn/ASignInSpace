using System.Collections;

namespace ASignInSpace
{
    public class Interpret
    {

        public const string Header64String = "0000011010010000001010000100000101000100100010000100010010001000";
        public const string Footer64String = "1111100000111110000000000000000000000000000000000000000000000000";
        public const string Header72String = "111111110000011010010000001010000100000101000100100010000100010010001000";
        public const string Footer72String = "111110000011111000000000000000000000000000000000000000000000000000001111";
        public const string HeaderFooter144String = $"11111111{Interpret.Header64String}{Interpret.Footer64String}00001111";


        public int InstructionAddressSize
        {
            get;
            set;
        } = 6;

        public int MapAddressSize
        {
            get;
            set;
        } = 12;

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

        public BitArray ProgramOutput
        {
            get;
            set;
        }

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

            this.AlienProgram = AlienProgram;
            ProgramOutput = new BitArray((int) Math.Pow(2, MapAddressSize));
            int instructionBlockSize = (int) Math.Pow(2, InstructionAddressSize);
            int mapBlockSize = (int)Math.Pow(2, MapAddressSize);

            SortedDictionary<string, int> saveExecutionPointer = new SortedDictionary<string, int>();
            int iteration = 0;
            int ProgramOutputIndex = 0;
            ProgramOutput.SetAll(false);
            int executionPointer = 0;
            int flipRelativeAddress;
            string saveState;
            StreamWriter traceWriter = null;
            StreamWriter addressWriter = null;
            if (TraceExecution)
            {
                traceWriter = new StreamWriter($"{FilenamePrefix}-TraceState.txt", append: false);
                addressWriter = new StreamWriter($"{FilenamePrefix}-TraceAddresses.txt", append: false);
            }
            do
            {

                /*
                 * When the program starts repeating, then we are done.
                 * 
                 * There must be a better way to detect END OF PROGRAM!
                 * 
                 * */
                saveState = BitArrayHelper.ConvertToString(AlienProgram);
                if (saveExecutionPointer.ContainsKey(saveState))
                {
                    if (saveExecutionPointer[saveState] == executionPointer)
                    {
                        endOfProgramProcessing();
                        Console.WriteLine($"Program ended after {iteration} iterations.");
                        return;
                    }

                }
                else
                {
                    saveExecutionPointer[saveState] = executionPointer;
                    if (TraceExecution)
                    {
                        traceWriter.WriteLine(saveState);
                    }
                }

                iteration++;

                flipRelativeAddress = getValueInstruction(executionPointer);
                var flipAbsoluteAddress = flipBit(flipRelativeAddress);
                ProgramOutput[ProgramOutputIndex] = true;
                executionPointer = nextAddressInstruction(executionPointer);


                /*
                 * 
                 * Don't run forever. I have fed this interpreter bit strings that seem to run forever without repeating.
                 * 
                 * */
                if (iteration > MaxIterations)
                {
                    endOfProgramProcessing();
                    Console.WriteLine($"Aborting after {iteration} iterations.");
                    return;
                }

                if (BreakImageAtIteration == iteration)
                {
                    draw();
                }

                //int oneCount = 0;
                //for (int c = 0; c < ProgramOutput.Length; c++)
                //{
                //    if (ProgramOutput[c]) oneCount++;
                //}
                //if (oneCount >= 625)
                //{
                //    draw();
                //}

            } while (true);



            void endOfProgramProcessing()
            {
                if (traceWriter != null)
                {
                    traceWriter.Close();
                }
                if (addressWriter != null)
                {
                    addressWriter.Close();
                }
                draw();
            }

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
                for (int j = relativeAddress, count = 0; count < InstructionAddressSize; j = increment(j), count++)
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
                if (TraceExecution)
                {
                    addressWriter.WriteLine(ProgramOutputIndex.ToString("X"));
                }
                AlienProgram[j] = !AlienProgram[j];
                return j;
            }

            int nextAddressInstruction(int address)
            {
                int nextAddress, count;
                for (nextAddress = address, count = 0; count < InstructionAddressSize; nextAddress = increment(nextAddress), count++) ;
                return nextAddress;
            }

            int getAddressMap(int address)
            {
                int value = 0;
                int addr, count;
                for (addr = address, count = 0; count < MapAddressSize; addr = increment(addr), count++)
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
                d.PngFilename = @$"{FilenamePrefix}-{FileNumber}.png";
                d.DrawBitArray(ProgramOutput);
                ProgramOutputIndex = 0;
                ProgramOutput.SetAll(false);
                Console.WriteLine(@$"Writing file {FilenamePrefix}-{FileNumber}.png after {iteration} iterations.");
            }

        }


    }
}
