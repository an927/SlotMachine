# 🎰 Slot Machine Simulation

## 📝 Overview

A C# console-based slot machine simulation that provides a realistic gambling experience with detailed logging and statistical tracking.

## ✨ Features

- **3-Column Slot Machine**: Simple yet engaging gameplay
- **Wild Symbol Mechanics**: Substitution for winning combinations
- **Comprehensive Validation**: 
  - Balance checks
  - Bet validation
- **Detailed Logging**: 
  - Per-session log files
  - Comprehensive game statistics
- **Return to Player (RTP) Calculation**

## 🛠 Prerequisites

- .NET SDK 6.0+
- Visual Studio or Visual Studio Code (optional)

## 📂 Project Structure

```
slot-machine/
│
├── SlotMachine.cs         # Main simulation logic
├── math.csv               # Symbol distribution config
└── paytable.csv           # Symbol payout multipliers
```

## 🎲 Game Mechanics

### Input Parameters

1. Starting balance (cents)
2. Bet amount (cents)
3. Number of spins

### Winning Combinations

- **Matching Symbols**: All three reels show identical symbols
- **Wild Symbol**: 
  - Substitutes for any symbol
  - Enables alternative winning paths

### Payout Examples

| Reel Combination | Result | Payout |
|-----------------|--------|--------|
| `Bar \| Bar \| Bar` | Win | Bar multiplier |
| `Wild \| Bar \| Bar` | Win | Bar multiplier |
| `Wild \| Wild \| Wild` | Win | Wild multiplier |
| `Bar \| Cherry \| 777` | Lose | No payout |

## 🚀 Getting Started

### Compilation

```bash
# Clone the repository
git clone https://github.com/an927/SlotMachine.git

# Navigate to project directory
cd SlotMachine

# Build the project
dotnet build

# Run the simulation
dotnet run SlotMachine.cs
```

## 📊 Statistical Tracking

The simulation tracks:
- Total bets placed
- Total wins
- Return to Player (RTP) percentage
- Symbol hit frequencies
- Individual symbol total wins

## 🔧 Configuration

Customize game mechanics by modifying:
- `math.csv`: Adjust symbol distribution
- `paytable.csv`: Change payout multipliers

## 🚧 Limitations

- Single payline
- Console-based interface
- No persistent game state


## ⚖️ Disclaimer

**Disclaimer**: This is a simulation tool. Not intended for real gambling. Always gamble responsibly.

## 📄 License

[Specify Your License - e.g., MIT, Apache 2.0]

---

**Happy Spinning! 🎳**
