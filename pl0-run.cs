using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NotFounds
{
    public class Program
    {
        private static void Main(string[] Argv)
        {
            switch (Argv.Length)
            {
                case 1:
                    if (Argv[0].StartsWith("-"))
                    {
                        if (Argv[0] == "-v") ShowVersion();
                        else if (Argv[0] == "-h") ShowHelp();
                        else { ShowHelp(); Environment.Exit(1); }
                    }
                    else
                    {
                        var pl0 = new pl0();
                        pl0.Run(Argv[0]);
                    }
                    break;
                case 3:
                    int time;
                    if (Argv[0] == "-t" && int.TryParse(Argv[1], out time) && time > 0)
                    {
                        var pl0 = new pl0(time);
                        pl0.Run(Argv[2]);
                    }
                    else { ShowHelp(); Environment.Exit(1); }
                    break;
                default:
                    ShowHelp();
                    Environment.Exit(1);
                    break;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("[Usage]");
            Console.WriteLine("mono pl0-run [options] filename");
            Console.WriteLine("[Options]");
            Console.WriteLine("-h\t\tShow Help.");
            Console.WriteLine("-v\t\tShow Version.");
            Console.WriteLine("-t time\t\tSet TimeOut time is millisecond.");
        }

        private static void ShowVersion()
        {
            Console.WriteLine(FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location));
        }
    }

    public class pl0
    {
        private int regA;
        private int regB;
        private int regC;

        private int PC;

        private Stack<int> stack;
        private Dictionary<int, int> mem;

        private int TimeOut;

        public pl0(int timeout = 5000)
        {
            TimeOut = timeout;
            Init();
        }

        private void Init()
        {
            regA = 0;
            regB = 0;
            regC = 0;

            PC = 0;

            stack = new Stack<int>();
            mem   = new Dictionary<int, int>();
        }

        public void Run(string filePath)
        {
            if (!File.Exists(filePath)) WriteErrorAndExit("File Not Exits Exception");
            string[] instructions = File.ReadAllLines(filePath);
            var sw = new Stopwatch();

            Console.WriteLine(" --- Program Start! --- ");
            sw.Start();
            while (0 <= PC && instructions.Length > PC)
            {
                if (sw.ElapsedMilliseconds > TimeOut) WriteErrorAndExit($"Time Out : {TimeOut} ms");
                string[] args = instructions[PC].Replace('\t', ' ').Replace("  ", " ").Trim(' ').Split(' ', ',');
                switch (args[0].ToUpper())
                {
                    case "LOAD":
                        LOAD(args[1], args[2]);
                        break;
                    case "STORE":
                        STORE(args[1], args[2]);
                        break;
                    case "PUSH":
                        PUSH(args[1]);
                        break;
                    case "POP":
                        POP(args[1]);
                        break;
                    case "PLUS":
                        PLUS();
                        break;
                    case "MINUS":
                        MINUS();
                        break;
                    case "MULTI":
                        MULTI();
                        break;
                    case "DIV":
                        DIV();
                        break;
                    case "CMPODD":
                        CMPODD();
                        break;
                    case "CMPEQ":
                        CMPEQ();
                        break;
                    case "CMPLT":
                        CMPLT();
                        break;
                    case "CMPGT":
                        CMPGT();
                        break;
                    case "CMPNOTEQ":
                        CMPNOTEQ();
                        break;
                    case "CMPLE":
                        CMPLE();
                        break;
                    case "CMPGE":
                        CMPGE();
                        break;
                    case "JMP":
                        JMP(args[1]);
                        break;
                    case "JPC":
                        JPC(args[1]);
                        break;
                    case "PRINT":
                        PRINT(args[1]);
                        break;
                    case "PRINTLN":
                        PRINTLN();
                        break;
                    case "END":
                        sw.Stop();
                        Console.WriteLine(" --- Program End! --- ");
                        Console.WriteLine($" Time : {sw.ElapsedMilliseconds} ms");
                        return;
                    case "DEBUG":
                        DEBUG();
                        break;
                    default:
                        WriteErrorAndExit("Syntax Error");
                        break;
                }
                PC++;
            }
        }

        private void LOAD(string reg, string adr)
        {
            int val;
            if (!int.TryParse(adr, out val))
            {
                string address = string.Concat(adr.Where(d => (Char.IsDigit(d))).ToArray());
                if (!mem.TryGetValue(int.Parse(address), out val))
                {
                    WriteErrorAndExit("Null Pointer Exception : Wrong Adress");
                }
            }

            switch (reg.ToUpper())
            {
                case "A":
                    regA = val;
                    break;
                case "B":
                    regB = val;
                    break;
                case "C":
                    regC = val;
                    break;
                default:
                    WriteErrorAndExit($"Syntax Error : Not Exists \"{reg}\"");
                    break;
            }
        }

        private void STORE(string reg, string adr)
        {
            string tmp = string.Concat(adr.Where(d => (Char.IsDigit(d) || "ABC".IndexOf(Char.ToUpper(d)) != -1)).ToArray());
            int address;
            if (!int.TryParse(tmp, out address))
            {
                switch (tmp)
                {
                    case "A":
                        address = regA;
                        break;
                    case "B":
                        address = regB;
                        break;
                    case "C":
                        address = regC;
                        break;
                    default:
                        WriteErrorAndExit($"Systax Error : Not Exists \"{tmp}\"");
                        break;
                }
            }
            switch (reg.ToUpper())
            {
                case "A":
                    mem[address] = regA;
                    break;
                case "B":
                    mem[address] = regB;
                    break;
                case "C":
                    mem[address] = regC;
                    break;
                default:
                    WriteErrorAndExit($"Systax Error : Not Exists \"{reg}\"");
                    break;
            }
        }

        private void PUSH(string reg)
        {
            switch (reg.ToUpper())
            {
                case "A":
                    stack.Push(regA);
                    break;
                case "B":
                    stack.Push(regB);
                    break;
                case "C":
                    stack.Push(regC);
                    break;
                default:
                    WriteErrorAndExit($"Systax Error : Not Exists \"{reg}\"");
                    break;
            }
        }

        private void POP(string reg)
        {
            switch (reg.ToUpper())
            {
                case "A":
                    regA = stack.Pop();
                    break;
                case "B":
                    regB = stack.Pop();
                    break;
                case "C":
                    regC = stack.Pop();
                    break;
                default:
                    WriteErrorAndExit($"Systax Error : Not Exists \"{reg}\"");
                    break;
            }
        }

        private void PLUS()
        {
            regC = regA + regB;
        }

        private void MINUS()
        {
            regC = regA - regB;
        }

        private void MULTI()
        {
            regC = regA * regB;
        }

        private void DIV()
        {
            if (regB == 0) WriteErrorAndExit($"Zero Division Exception");
            regC = regA / regB;
        }

        private void CMPODD()
        {
            if (regA % 2 == 1) regC = 1;
            else regC = 0;
        }

        private void CMPEQ()
        {
            if (regA == regB) regC = 1;
            else regC = 0;
        }

        private void CMPLT()
        {
            if (regA < regB) regC = 1;
            else regC = 0;
        }

        private void CMPGT()
        {
            if (regA > regB) regC = 1;
            else regC = 0;
        }

        private void CMPNOTEQ()
        {
            if (regA != regB) regC = 1;
            else regC = 0;
        }

        private void CMPLE()
        {
            if (regA <= regB) regC = 1;
            else regC = 0;
        }

        private void CMPGE()
        {
            if (regA >= regB) regC = 1;
            else regC = 0;
        }

        private void JMP(string address)
        {
            PC = int.Parse(address) - 2;
        }

        private void JPC(string address)
        {
            if (regC == 0) PC = int.Parse(address) - 2;
        }

        private void PRINT(string reg)
        {
            switch (reg.ToUpper())
            {
                case "A":
                    Console.Write(regA);
                    break;
                case "B":
                    Console.Write(regB);
                    break;
                case "C":
                    Console.Write(regC);
                    break;
                default:
                    WriteErrorAndExit($"Systax Error : Not Exists \"{reg}\"");
                    break;
            }
        }

        private void PRINTLN()
        {
            Console.WriteLine("");
        }

        private void DEBUG()
        {
            Console.Error.WriteLine($"StackLen:[{stack.Count}] regA:{regA} regB:{regB} regC:{regC} PC:{PC}");
            foreach (var pair in mem)
            {
                Console.Error.WriteLine($"Adress:{pair.Key} Value:{pair.Value}");
            }
        }

        private void WriteErrorAndExit(string message)
        {
            Console.Error.WriteLine($"{message}.(PC:{PC})");
            Environment.Exit(1);
        }
    }
}
