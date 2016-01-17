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
                case 2:
                    if (Argv[0] == "-t")
                    {
                        var pl0 = new pl0();
                        pl0.Run(Argv[1], true);
                    }
                    else { ShowHelp(); Environment.Exit(1); }
                    break;
                case 3:
                    int time;
                    if (Argv[0] == "-t" && int.TryParse(Argv[1], out time) && time > 0)
                    {
                        var pl0 = new pl0(time);
                        pl0.Run(Argv[2], true);
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
            Console.WriteLine("-t time\t\tSet TimeOut time with millisecond.");
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
        private int FP;
        private int SP;

        private const int MaxSize = 1000;
        private const int offset  = 800;
        private const int MemSize = MaxSize - offset;
        private int[] memory = new int[MemSize];

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

            PC = 1;
            FP = MaxSize;
            SP = MaxSize;

            memory = new int[MemSize];
        }

        public void Run(string filePath, bool isCount = false)
        {
            if (!File.Exists(filePath)) WriteErrorAndExit("File Not Exits Exception");
            string[] instructions = File.ReadAllLines(filePath);
            var sw = new Stopwatch();

            if (isCount)
            {
                Console.WriteLine(" --- Program Start! --- ");
                sw.Start();
            }
            while (1 <= PC && instructions.Length >= PC)
            {
                if (isCount && sw.ElapsedMilliseconds > TimeOut) WriteErrorAndExit($"Time Out : {TimeOut} ms");
                string[] args = instructions[PC - 1].Replace('\t', ' ').Replace("  ", " ").Trim(' ').Split(' ', ',');
                if (args[0].StartsWith("@")) { DEBUG(); args[0] = args[0].Substring(1); }
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
                    case "CALL":
                        CALL(args[1]);
                        break;
                    case "RET":
                        RET(args[1]);
                        break;
                    case "PUSHUP":
                        PUSHUP();
                        break;
                    case "PRINT":
                        PRINT(args[1]);
                        break;
                    case "PRINTLN":
                        PRINTLN();
                        break;
                    case "END":
                        if (isCount)
                        {
                            sw.Stop();
                            Console.WriteLine(" --- Program End! --- ");
                            Console.WriteLine($" Time : {sw.ElapsedMilliseconds} ms");
                        }
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

        private int AnalyzeParam(string param)
        {
            int ret;
            param = param.Replace(" ", "").Replace("#(", "").Replace(")", "");
            if (int.TryParse(param, out ret)) return ret;
            if (GetRegFromString(param, out ret)) return ret;

            string[]    tokens   = param.Split('+', '-');
            Stack<char> operands = new Stack<char>();
            param.Where(c => (c == '+' || c == '-')).ToList().ForEach(c => operands.Push(c));

            if (tokens.Length != operands.Count + 1)
                WriteErrorAndExit($"Syntax Error");

            ret = AnalyzeParam(tokens[0]);
            for (int i = 1; 0 < operands.Count; i++)
            {
                char ope = operands.Pop();
                switch (ope)
                {
                    case '+':
                        ret += AnalyzeParam(tokens[i]);
                        break;
                    case '-':
                        ret -= AnalyzeParam(tokens[i]);
                        break;
                }
            }
            return ret;
        }

        private bool GetRegFromString(string reg, out int value)
        {
            switch (reg.ToUpper())
            {
                case "A":
                    value = regA;
                    return true;
                case "B":
                    value = regB;
                    return true;
                case "C":
                    value = regC;
                    return true;
                case "FP":
                    value = FP;
                    return true;
                case "SP":
                    value = SP;
                    return true;
                default:
                    value = -1;
                    return false;
            }
        }

        private void LOAD(string reg, string adr)
        {
            int val;
            if (int.TryParse(adr, out val)) {}
            else if (GetRegFromString(adr, out val)) {}
            else
            {
                int address = AnalyzeParam(adr);
                if (offset <= address && address <= MaxSize)
                {
                    address -= offset;
                }
                else WriteErrorAndExit($"Null Pointer Exception : Wrong Address {address}");
                val = memory[address];
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
                case "FP":
                    FP = val;
                    break;
                case "SP":
                    SP = val;
                    break;
                default:
                    WriteErrorAndExit($"Syntax Error : Not Exists \"{reg}\"");
                    break;
            }
        }

        private void STORE(string reg, string adr)
        {
            int address = AnalyzeParam(adr);

            if (offset <= address && address < MaxSize)
            {
                address -= offset;
            }
            else WriteErrorAndExit($"Null Pointer Exception : Wrong Address {address}");

            memory[address] = AnalyzeParam(reg);
        }

        private void PUSH(string reg)
        {
            Action<int> Push = (int value) =>
            {
                if (offset < SP)
                {
                    memory[--SP - offset] = value;
                }
                else WriteErrorAndExit($"Out Of Memory Exception");
            };

            int val;
            if (!GetRegFromString(reg, out val)) WriteErrorAndExit($"Syntax Error : Not Exists {reg}");

            Push(val);
        }

        private void POP(string reg)
        {
            Func<int> Pop = () =>
            {
                if (SP < MaxSize)
                {
                    return memory[SP++ - offset];
                }
                else WriteErrorAndExit($"Stack Empry Exception");
                return 0;
            };
            switch (reg.ToUpper())
            {
                case "A":
                    regA = Pop();
                    break;
                case "B":
                    regB = Pop();
                    break;
                case "C":
                    regC = Pop();
                    break;
                case "FP":
                    FP = Pop();
                    break;
                case "SP":
                    SP = Pop();
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
            PC = AnalyzeParam(address) - 1;
        }

        private void JPC(string address)
        {
            if (regC == 0) JMP(address);
        }

        private void CALL(string address)
        {
            Action<int> Push = (int value) =>
            {
                if (offset < SP)
                {
                    memory[--SP - offset] = value;
                }
                else WriteErrorAndExit($"Out Of Memory Exception");
            };
            Push(PC + 1);
            JMP(address);
        }

        private void RET(string address)
        {
            Func<int> Pop = () =>
            {
                if (SP < MaxSize)
                {
                    return memory[SP++ - offset];
                }
                else WriteErrorAndExit($"Out Of Memory Exception");
                return 0;
            };
            PC = Pop() - 1;
            SP += AnalyzeParam(address);
        }

        private void PUSHUP()
        {
            SP--;
        }

        private void PRINT(string reg)
        {
            Console.Write(AnalyzeParam(reg));
        }

        private void PRINTLN()
        {
            Console.WriteLine("");
        }

        private void DEBUG()
        {
            Console.Error.Write($"regA:{regA} regB:{regB} regC:{regC} PC:{PC} FP:{FP} SP:{SP}");
            Console.Error.Write(" Stack:{");
            for (int i = SP; i < MaxSize; i++)
                Console.Error.Write($"{memory[i - offset]} ");
            Console.Error.WriteLine("}");
        }

        private void WriteErrorAndExit(string message)
        {
            Console.Error.WriteLine($"{message}.(PC:{PC})");
            DEBUG();
            Environment.Exit(1);
        }
    }
}
