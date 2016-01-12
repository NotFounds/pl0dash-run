using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NotFounds
{
    public class Program
    {
        private static void Main(string[] Argv)
        {
            if (Argv.Length == 0) ShowHelp();
            if (Argv.Length == 1)
            {
                var pl0 = new pl0();
                pl0.Run(Argv[0]);
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("mono pl0-run filename");
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

        public pl0()
        {
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
            if (!File.Exists(filePath)) Console.WriteLine("File Not Exits Exception.");
            string[] instructions = File.ReadAllLines(filePath);
            Console.WriteLine(" --- Program Start! --- ");
            while (true)
            {
                string[] args = instructions[PC].Split(' ', ',');

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
                        Console.WriteLine(" --- Program End! --- ");
                        return;
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
                val = mem[int.Parse(address)];
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
            if (regC == 1) PC = int.Parse(address) - 2;
        }

        private void PRINT(string reg)
        {
            switch (reg.ToUpper())
            {
                case "A":
                    Console.WriteLine(regA);
                    break;
                case "B":
                    Console.WriteLine(regB);
                    break;
                case "C":
                    Console.WriteLine(regC);
                    break;
            }
        }

        private void PRINTLN()
        {
            Console.WriteLine("");
        }
    }
}
