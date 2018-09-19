﻿using SonicHeroes.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using static SonicHeroes.Networking.Client_Functions;

namespace SonicHeroes.Hooking
{
    /// <summary>
    /// Provides a base storing the common features of each of the Heroes Mod Loader Hook Types
    /// </summary>
    public abstract class Hook_Base
    {
        /// <summary>
        /// Present for better code readibility, this is the length of the push and return sets of instructions themselves.
        /// </summary>
        protected const int PUSH_RETURN_INSTRUCTION_LENGTH = 6;

        /// <summary>
        /// Present for better code readibility, this is the length of a PUSH dword ASM instruction.
        /// </summary>
        protected const int PUSH_INSTRUCTION_LENGTH = 5;

        /// <summary>
        /// Present for better code readibility, this is the length of the jump (IntPtr) instruction in x86.
        /// </summary>
        protected const int JUMP_INSTRUCTION_LENGTH = 5;

        /// <summary>
        /// The amount of registers we will back up before calling own code, we will restore them right after the call!
        /// </summary>
        protected const int REGISTERS_TO_BACKUP_LENGTH = 8;

        /// <summary>
        /// Stores the representations of the PUSH instructions used to place the registers on the stack before our own method execution for compatible hooks.
        /// </summary>
        protected byte[] ASM_PUSH_REGISTERS_BYTES = new byte[REGISTERS_TO_BACKUP_LENGTH]
        {
            0x50, // PUSH EAX
            0x51, // PUSH ECX
            0x52, // PUSH EDX
            0x53, // PUSH EBX
            0x54, // PUSH ESP
            0x55, // PUSH EBP
            0x56, // PUSH ESI
            0x57 // PUSH EDI
        };

        /// <summary>
        /// Stores the representations of the POP instructions used to retrieve the registers from the stack post own method execution for compatible hooks.
        /// </summary>
        protected byte[] ASM_POP_REGISTERS_BYTES = new byte[REGISTERS_TO_BACKUP_LENGTH]
        {
            // These are in reverse order due to last in first out stack order, like a set of stacked books.
            0x5F, // POP EDI
            0x5E, // POP ESI
            0x5D, // POP EBP
            0x5C, // POP ESP
            0x5B, // POP EBX
            0x5A, // POP EDX
            0x59, // POP ECX
            0x58 // POP EAX
        };

        /// <summary>
        /// The user defined number of bytes to replace while performing a hook, there must be no stray bytes left from another instruction or set of instructions.
        /// </summary>
        public int hookLength = 0;

        /// <summary>
        /// This is the address which we will be hooking, the address where a call jmp is placed to redirect our program flow to our own function.
        /// </summary>
        public IntPtr hookAddress;

        /// <summary>
        /// This will store the old original memory protection which we will restore along with the original bytes should we wish to fully dispose of the hook.
        /// </summary>
        protected HeroesProcess.MemoryProtection originalMemoryProtection;

        /// <summary>
        /// The original source array of bytes which we will be hooking/placing a call jump to our own code from.
        /// </summary>
        protected byte[] originalBytes;

        /// <summary>
        /// The new bytes which we will place to make a call jump to our own code.
        /// </summary>
        protected byte[] newBytes;

        /// <summary>
        /// This will point to where the backing up of the registers will occur, the method call for the dll and the restoration of the registers, running of the original code and jumping back will occur (or similar).
        /// </summary>
        protected IntPtr newInstructionAddress;

        /// <summary>
        /// These are the bytes which will be stored at the new instruction address which correspond to assembly instructions. Here in this memory region, ASM to backup the registers will be written, a call to our own method, will be performed, registers will be restored and a jump will be made back.
        /// </summary>
        protected byte[] newInstructionBytes;

        /// <summary>
        /// Hold a copy of the delegate to the method we want to execute. Otherwise the .NET Garbage Collector will nuke it and spectacularly crash Sonic Heroes since it probably thinks the game is garbage.
        /// </summary>
        protected Delegate customMethodDelegate;

        /// <summary>
        /// Might aswell also store the function pointer to this delegate, .NET Garbage Collector is a scary monster! Choo Choo!
        /// </summary>
        protected IntPtr funcionPointerToOwnMethodCall;

        /// <summary>
        /// Sets up the common hook properties and fields such as length and address.
        /// </summary>
        protected void SetupHookCommon(IntPtr hookAddress, int hookLength)
        {
            // Set Hook Length, Address
            this.hookLength = hookLength;
            this.hookAddress = hookAddress;

            /// The Setup
            /// Getting Ready for Hooking!
            originalBytes = new byte[hookLength]; // Initialize storage of original bytes.
            newBytes = new byte[hookLength]; // Initialize storage of new bytes.

            /// Backup Original Bytes &
            /// Remove protection from the old set of bytes such that we may alter the bytes.
            SonicHeroes.Memory.HeroesProcess.VirtualProtect(hookAddress, (uint)hookLength, (SonicHeroes.Memory.HeroesProcess.MemoryProtection)Protection.PAGE_EXECUTE_READWRITE, out originalMemoryProtection);
            Marshal.Copy(hookAddress, originalBytes, 0, hookLength);
        }

        /// <summary>
        /// Assembles a return instruction to a specified address.
        /// </summary>
        protected byte[] AssembleReturn(int address, SonicHeroes.Networking.WebSocket_Client modLoaderServerSonic)
        {
            // Assemble code for push from new time string format address.
            string[] x86Mnemonics = new string[]
            {
                "use32",
                "push 0x" + address.ToString("X"),
                "ret"
            };
            return modLoaderServerSonic.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(x86Mnemonics), true);
        }

        /// <summary>
        /// Assembles a push instruction to a specified address.
        /// </summary>
        protected byte[] AssemblePush(int address, SonicHeroes.Networking.WebSocket_Client modLoaderServerSonic)
        {
            // Assemble code for push from new time string format address.
            string[] x86Mnemonics = new string[]
            {
                "use32",
                "push 0x" + address.ToString("X"),
            };
            return modLoaderServerSonic.SendData_Alternate(Message_Type.Client_Call_Assemble_x86_Mnemonics, Serialize_x86_ASM_Mnemonics(x86Mnemonics), true);
        }

        /// <summary>
        /// Sets the new instruction address for the hooked assembly or custom code.
        /// </summary>
        protected void SetNewInstructionAddress(int length)
        {
            // Retrieve Memory Address where to write to our own.
            newInstructionAddress = SonicHeroes.Memory.HeroesProcess.AllocateMemory(Process.GetCurrentProcess(),length);
        }

        /// <summary>
        /// Fill a byte array with NOPs until the specified hook length.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        protected byte[] FillNOPs(byte[] byteArray)
        {
            // Assign list of bytes for the new byte array.
            List<byte> newByteArray = byteArray.ToList();

            // If necessary, replace any stray bytes with NOP.
            for (int x = PUSH_RETURN_INSTRUCTION_LENGTH - 1; x < hookLength; x++) { newByteArray.Add(0x90); }

            // Return Byte Array
            return newByteArray.ToArray();
        }

        /// <summary>
        /// Produces a NOP Array of the specified passed in length.
        /// </summary>
        protected byte[] ProduceNOPArray(int length)
        {
            // Allocate Array.
            byte[] nopArray = new byte[length];

            // No Operation Array.
            for (int x = 0; x < nopArray.Length; x++) { nopArray[x] = 0x90; }

            // Return NOP Array
            return nopArray;
        }

        /// <summary>
        /// Activates the hook such that it may be used.
        /// </summary>
        public void Activate() { Marshal.Copy(newBytes, 0, hookAddress, hookLength); }

        /// <summary>
        /// Deactivates the hook such that it may be used.
        /// </summary>
        public void Deactivate() { Marshal.Copy(originalBytes, 0, hookAddress, hookLength); }

        /// <summary>
        /// Defines the different memory protection states. This is the same as in the Process flags, except in non-flag form.
        /// </summary>
        public enum Protection
        {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }
    }
}
