using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

class SlotMachine
{
    private List<string> column1Symbols = [];
    private List<string> column2Symbols = [];
    private List<string> column3Symbols = [];
    private Dictionary<string, long> payTable;
    private StreamWriter logWriter;

    public SlotMachine(string mathCsvPath, string paytableCsvPath, string logFilePath)
    {
        LoadSymbols(mathCsvPath);
        LoadPayTable(paytableCsvPath);

        // Create log file with a unique timestamp
        logWriter = new StreamWriter(logFilePath, false);
        LogMessage("Slot Machine Simulation Log");
        LogMessage($"Started at: {DateTime.Now}");
        LogMessage("======================================");
    }

    private void LogMessage(string message)
    {
        // Log to console
        Console.WriteLine(message);

        // Log to file
        logWriter.WriteLine(message);
        logWriter.Flush();
    }

    private void LogSpin(long spinNumber, string[] reel, long win, long balance)
    {
        StringBuilder spinLog = new StringBuilder();
        spinLog.AppendLine("-------------------------------");
        spinLog.AppendLine($"| {reel[0]} | {reel[1]} | {reel[2]} |");
        spinLog.AppendLine("-------------------------------");
        spinLog.AppendLine($"Spin: {spinNumber}, Win: {win} ({(win > 0 ? reel.FirstOrDefault(s => s != "Wild") ?? "Wild" : "No Win")}), Balance: {balance}");

        LogMessage(spinLog.ToString());
    }

    private void LoadSymbols(string mathCsvPath)
    {
        var lines = File.ReadAllLines(mathCsvPath);

        for (int i = 1; i < lines.Length; i++)
        {
            var columns = lines[i].Split(',');

            if (columns.Length >= 3)
            {
                column1Symbols.Add(columns[0].Trim().Trim('"'));
                column2Symbols.Add(columns[1].Trim().Trim('"'));
                column3Symbols.Add(columns[2].Trim().Trim('"'));
            }
        }
    }

    private void LoadPayTable(string paytableCsvPath)
    {
        payTable = new Dictionary<string, long>();
        var lines = File.ReadAllLines(paytableCsvPath);

        for (int i = 0; i < lines.Length; i++)
        {
            var columns = lines[i].Split(',');
            string symbol = columns[0].Trim('"');
            long multiplier = long.Parse(columns[1]);
            payTable[symbol] = multiplier;
        }
    }

    private string GetRandomSymbolForColumn(int columnIndex)
    {
        switch (columnIndex)
        {
            
            case 0: return column1Symbols[RandomNumberGenerator.GetInt32(0, column1Symbols.Count)];
            case 1: return column2Symbols[RandomNumberGenerator.GetInt32(0, column2Symbols.Count)];
            case 2: return column3Symbols[RandomNumberGenerator.GetInt32(0, column3Symbols.Count)];
            default: throw new ArgumentException("Invalid column index");
        }
    }

    public (string[] Reel, long Win) Spin(long betAmount)
    {
        // Generate reel symbols
        string[] reel = new string[3];
        for (int i = 0; i < 3; i++)
        {
            reel[i] = GetRandomSymbolForColumn(i);
        }

        // Check for win
        long win = CalculateWin(reel, betAmount);

        return (reel, win);
    }

    private long CalculateWin(string[] reel, long betAmount)
    {
        // Check if all symbols match or can match with Wild
        bool isWin = IsWinningCombination(reel);

        if (isWin)
        {
            // Get the winning symbol (first non-Wild symbol or Wild if all are Wild)
            string winningSymbol = GetWinningSymbol(reel);

            // Look up the multiplier and calculate win
            if (payTable.TryGetValue(winningSymbol, out long multiplier))
            {
                return betAmount * multiplier;
            }
        }

        return 0;
    }

    private string GetWinningSymbol(string[] reel)
    {
        // If all symbols are Wild, return "Wild"
        if (reel.All(s => s == "Wild"))
        {
            return "Wild";
        }

        // Filter out Wild symbols and get the first remaining symbol
        var nonWildSymbols = reel.Where(s => s != "Wild").Distinct().ToList();

        // If there's only one non-Wild symbol type after filtering, return that
        return nonWildSymbols.Count == 1 ? nonWildSymbols[0] : "";
    }

    private bool IsWinningCombination(string[] reel)
    {
        // If all symbols are the same, it's a win
        if (reel.Distinct().Count() == 1)
        {
            return true;
        }

        // If only one symbol (excluding Wilds) is present, it's a win
        var nonWildSymbols = reel.Where(s => s != "Wild").Distinct().ToList();
        if (nonWildSymbols.Count == 1)
        {
            return true;
        }

        return false;
    }

    public Dictionary<string, long> getPlayTable()
    {
        return payTable.Keys.ToDictionary(key => key, key => 0L);
    }

    public static void Main(string[] args)
    {
        try
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string mathCsvPath = Path.Combine(baseDirectory, "math.csv");
            string paytableCsvPath = Path.Combine(baseDirectory, "paytable.csv");
            string logFilePath = Path.Combine(baseDirectory, $"SlotMachine_Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            // Create slot machine
            SlotMachine slotMachine = new SlotMachine(mathCsvPath, paytableCsvPath, logFilePath);

            // Get user input with validation
            long balance = GetValidatedBalance();
            long betAmount = GetValidatedBet(balance);
            long numberOfSpins = GetValidatedSpins();

            slotMachine.LogMessage($"Balance={balance}, Bet={betAmount}, Spin number={numberOfSpins}");
            slotMachine.LogMessage("\n---------- Start Spin ---------");

            // Tracking variables
            long totalBet = 0;
            long totalWin = 0;
            Dictionary<string, long> symbolHits = slotMachine.getPlayTable();
            Dictionary<string, long> symbolTotalWins = slotMachine.getPlayTable();


            // Play spins
            string stopReason = "";
            long spinNumber = 0;
            while (spinNumber < numberOfSpins && balance >= betAmount)
            {
                // Deduct bet from balance
                balance -= betAmount;
                totalBet += betAmount;

                // Spin the reel
                var (reel, win) = slotMachine.Spin(betAmount);

                // Update balance and tracking
                balance += win;
                totalWin += win;

                slotMachine.LogSpin(spinNumber, reel, win, balance);

                // Track symbol hits and wins
                if (win > 0)
                {
                    string winningSymbol = reel.FirstOrDefault(s => s != "Wild") ?? "Wild";

                    symbolHits[winningSymbol]++;
                    symbolTotalWins[winningSymbol] += win;
                }

                spinNumber++;
            }

            // Determine stop reason
            if (spinNumber == numberOfSpins)
            {
                stopReason = "All spins done";
            }
            else
            {
                stopReason = "Not enough money on the balance";
            }

            // Calculate and display final statistics
            slotMachine.LogMessage("\n------- Spin Statistics -------");
            slotMachine.LogMessage($"Stop Reason: {stopReason}");

            double rtp = totalBet > 0 ? (double)totalWin / totalBet * 100.00 : 0;
            slotMachine.LogMessage($"RTP: {rtp:F2}%, Spins: {spinNumber}, Total bet: {totalBet}, Total win: {totalWin}, Final Balance: {balance}");

            slotMachine.LogMessage("Win Stats:");
            var sortedSymbols = symbolHits.Keys.OrderBy(s => s);
            foreach (var symbol in sortedSymbols)
            {
                long hits = symbolHits.ContainsKey(symbol) ? symbolHits[symbol] : 0;
                long totalSymbolWin = symbolTotalWins.ContainsKey(symbol) ? symbolTotalWins[symbol] : 0;
                slotMachine.LogMessage($"  \"{symbol}\" - {hits} hit, Total win: {totalSymbolWin}");
            }

            // Close log file
            slotMachine.logWriter.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static long GetValidatedBalance()
    {
        while (true)
        {
            Console.WriteLine("Starting balance (in cents):");
            if (long.TryParse(Console.ReadLine(), out long balance) && balance > 0)
            {
                return balance;
            }
            Console.WriteLine("Invalid balance. Please enter a positive number.");
        }
    }

    private static long GetValidatedBet(long balance)
    {
        while (true)
        {
            Console.WriteLine("Bet (in cents):");
            if (long.TryParse(Console.ReadLine(), out long betAmount) &&
                betAmount > 0 &&
                betAmount <= balance)
            {
                return betAmount;
            }
            Console.WriteLine($"Invalid bet. Please enter a positive number not exceeding your current balance of {balance} cents.");
        }
    }

    private static long GetValidatedSpins()
    {
        while (true)
        {
            Console.WriteLine("Number of spins to play:");
            if (long.TryParse(Console.ReadLine(), out long numberOfSpins) &&
                numberOfSpins > 0)
            {
                return numberOfSpins;
            }
            Console.WriteLine($"Invalid number of spins. Please enter a positive number spins.");
        }
    }

    public void Dispose()
    {
        logWriter?.Close();
    }
}