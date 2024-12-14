using System;
using System.Collections.Generic;

class BuddySystem
{
    private int totalMemory;
    private Dictionary<int, List<int>> freeBlocks; // stores the free memory blocks
    private Dictionary<int, int> allocatedBlocks;  // stores the allocated memory blocks

    // Constructor to initialize the system with a given memory pool size
    public BuddySystem(int memorySize)
    {
        totalMemory = memorySize;
        freeBlocks = new Dictionary<int, List<int>>();
        allocatedBlocks = new Dictionary<int, int>(); 
        freeBlocks[memorySize] = new List<int> { 0 };
    }

    // Allocates the memory.    
    public int Allocate(int size)
    {
        int blockSize = GetNextPowerOfTwo(size);

        foreach (var kvp in freeBlocks)
        {
            if (kvp.Key >= blockSize && kvp.Value.Count > 0)
            {
                int address = kvp.Value[0];
                kvp.Value.RemoveAt(0);

                int currentBlockSize = kvp.Key;
                while (currentBlockSize > blockSize)
                {
                    int halfSize = currentBlockSize / 2;

                    if (!freeBlocks.ContainsKey(halfSize))
                        freeBlocks[halfSize] = new List<int>();

                    freeBlocks[halfSize].Add(address + halfSize);

                    currentBlockSize = halfSize;
                }

                allocatedBlocks[address] = blockSize; 

                Console.WriteLine($"Allocated {blockSize} KB at address {address}.");
                return address;
            }
        }

        Console.WriteLine("Allocation failed: Not enough memory.");
        return -1;
    }


    // Free the memory blocks.    
    public void Free(int address, int size)
    {
        int blockSize = GetNextPowerOfTwo(size);

        if (allocatedBlocks.ContainsKey(address) && allocatedBlocks[address] == blockSize)
        {
            allocatedBlocks.Remove(address);

            if (!freeBlocks.ContainsKey(blockSize))
                freeBlocks[blockSize] = new List<int>();

            freeBlocks[blockSize].Add(address);

            while (true)
            {
                int buddyAddress = GetBuddyAddress(address, blockSize);
                if (freeBlocks[blockSize].Contains(buddyAddress))
                {
                    freeBlocks[blockSize].Remove(address);
                    freeBlocks[blockSize].Remove(buddyAddress);
                    address = Math.Min(address, buddyAddress);
                    blockSize *= 2;

                    if (!freeBlocks.ContainsKey(blockSize))
                        freeBlocks[blockSize] = new List<int>();

                    freeBlocks[blockSize].Add(address);
                    Console.WriteLine($"Merged blocks into {blockSize} KB at address {address}.");
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine("Error: Block not found or size mismatch.");
        }
    }

    // Gets the power of 2s for memory allocation.
    private int GetNextPowerOfTwo(int size)
    {
        int power = 1;
        while (power < size)
            power *= 2;
        return power;
    }


    // gets the memory address of the buddy.
    private int GetBuddyAddress(int address, int blockSize)
    {
        return address ^ blockSize;
    }

    // prints the memory state.
    public void PrintMemoryState()
    {
        Console.WriteLine("Memory State:");
        Console.WriteLine("Allocated Blocks:");
        foreach (var kvp in allocatedBlocks)
        {
            Console.WriteLine($"Address: {kvp.Key}, Size: {kvp.Value} KB");
        }

        Console.WriteLine("Free Blocks:");
        foreach (var kvp in freeBlocks)
        {
            if (kvp.Value.Count > 0)
                Console.WriteLine($"Block Size: {kvp.Key}");
        }
    }

    //Main method to run the program with all the functionalities.
    public static void Main(string[] args)
    {
        Console.Write("Enter the total memory pool size (in KB): ");
        int memoryPoolSize = int.Parse(Console.ReadLine());

        BuddySystem buddySystem = new BuddySystem(memoryPoolSize);

        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Allocate memory");
            Console.WriteLine("2. Free memory");
            Console.WriteLine("3. Print memory state");
            Console.WriteLine("4. Exit");

            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.Write("Enter the size of memory to allocate (in KB): ");
                    int allocateSize = int.Parse(Console.ReadLine());
                    buddySystem.Allocate(allocateSize);
                    break;

                case 2:
                    Console.Write("Enter the starting address of the block to free: ");
                    int address = int.Parse(Console.ReadLine());
                    Console.Write("Enter the size of the block to free (in KB): ");
                    int freeSize = int.Parse(Console.ReadLine());
                    buddySystem.Free(address, freeSize);
                    break;

                case 3:
                    buddySystem.PrintMemoryState();
                    break;

                case 4:
                    Console.WriteLine("Exiting...");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}


