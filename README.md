# pl0dash-run

# What is this?
+ The program is for  running the PL0dash assembly file.
+ PL0dash compiler is in [simozono/pl0dash](https://github.com/simozono/pl0dash).

# Environment
+ Mac OS X El Capitan
+ Mono 4.2.0
+ C# 6.0

# How to Use

## install
`git clone this`

## build
`mcs pl0-run.cs`

## run
`mono pl0-run.exe [pl0dash assembly file path]`

# Virtual CPU
+ CPU has 3 registers, `A` and `B`, `C`.

# Instruction Set
|Instrunction|               syntax                 |About|
|:-----------|:-------------------------------------|:----|
|LOAD        |LOAD reg,num<br>LOAD reg,#(adr)       |substitute `num` for `reg`<br>substitute memory in pointed by `adr` for `reg`|
|STORE       |STORE reg,#(adr)<br>STORE reg1,#(reg2)|write `reg` to memory in pointed by `adr`<br>write `reg1` to memory in pointed by `reg2`|
|PUSH        |PUSH reg                              |push `reg` to stask|
|POP         |POP reg                               |pop value to `reg` from stack|
|PLUS        |PLUS                                  |registerA + registerB = registerC|
|MINUS       |MINUS                                 |registerA - registerB = registerC|
|MULTI       |MULTI                                 |registerA * registerB = registerC|
|DIV         |DIV                                   |registerA / registerB = registerC|
|CMPODD      |CMPODD                                |if registerA is odd then registerC = `1`, else `0`|
|CMPEQ       |CMPEQ                                 |if registerA == registerB then registerC = `1`, else `0`|
|CMPLT       |CMPLT                                 |if registerA < registerB then registerC = `1`, else `0`|
|CMPGT       |CMPGT                                 |if registerA > registerB then registerC = `1`, else `0`|
|CMPNOTEQ    |CMPNOTEQ                              |if registerA != registerB then registerC = `1`, else `0`|
|CMPLE       |CMPLE                                 |if registerA <= registerB then registerC = `1`, else `0`|
|CMPGE       |CMPGE                                 |if registerA >= registerB then registerC = `1`, else `0`|
|JMP         |JMP adr                               |jump to `adr`|
|JPC         |JPC adr                               |if registerC is `0` then jump to `adr`|
|PRINT       |PRINT reg                             |write `reg` to console|
|PRINTLN     |PRINTLN                               |write `\n`|
|END         |END                                   |end|

# License
Copyright (c) 2015 Iori Ikeda
This software is released under the MIT License, see [License](https://github.com/NotFounds/pl0dash-run/blob/master/LICENSE)。
