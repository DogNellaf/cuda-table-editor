# Table Editor

> 🇬🇧 English | [🇷🇺 Русский](README.ru.md)

A Windows desktop application for creating, editing, and saving tabular data with GPU-accelerated column sorting and formula evaluation via CUDA.

## Features

- Create tables with a configurable number of rows and columns
- Open and save tables in CSV format
- Edit cells directly in the grid
- Add and delete rows and columns
- **GPU-accelerated column sorting** using a CUDA selection-sort kernel
- **GPU-accelerated formula evaluation** — enter a formula starting with `=` to compute arithmetic expressions
- **Cell references in formulas** — reference other cells using the `{row}A{col}` notation (e.g. `=1A2+3A4`)
- Circular reference detection (`#CIRC!`)
- Recalculate all formulas at once with the **⟳ Recalc** button
- Keyboard shortcuts: `Ctrl+N` (new), `Ctrl+O` (open), `Ctrl+S` (save)

## Tech Stack

| Layer | Technology |
|---|---|
| Platform | Windows 10/11 (x64) |
| Runtime | .NET 6 (WPF) |
| GPU compute | CUDA 10.x |
| Language | C# |
| Tests | xUnit |

## Requirements

- Windows 10/11 (x64)
- [.NET 6.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- NVIDIA GPU with CUDA support (Compute Capability 5.0+)
- [CUDA Toolkit 10.x](https://developer.nvidia.com/cuda-toolkit-archive) runtime libraries

## Installation

1. Install the [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) and Visual Studio 2022 (or the Roslyn build tools).
2. Install the CUDA Toolkit 10.x and ensure `nvcc` is on your `PATH`.
3. Open `table_editor.sln` in Visual Studio and build, or run:

```
dotnet build table_editor.sln
```

The compiled PTX kernel files (`selectionsort.ptx`, `evaluate.ptx`) are pre-built and copied to the output directory automatically. To recompile them from source:

```bash
nvcc -ptx selectionsort.cu -o selectionsort.ptx
nvcc -ptx evaluate.cu -o evaluate.ptx
```

## Formula Syntax

Formulas start with `=`. Cell references use the format `{row}A{col}` where `row` and `col` are 1-based integers.

| Input | Meaning |
|-------|---------|
| `=5+3` | Evaluates `5 + 3` on the GPU |
| `=1A2+1A3` | Adds the values of row 1 col 2 and row 1 col 3 |
| `=2A1*3A1` | Multiplies the values of row 2 col 1 and row 3 col 1 |

**Supported operators:** `+`, `-`, `*`, `/` (evaluated left-to-right; no operator precedence).

**Error codes:**

| Code | Meaning |
|------|---------|
| `#CIRC!` | Circular reference detected |
| `#REF!` | Cell address is out of range |
| `#TYPE!` | Referenced cell contains a non-numeric string |
| `#ERR: …` | GPU evaluation error |

## CSV Format

Tables are stored as semicolon-delimited CSV files. Each row ends with a trailing semicolon followed by a newline. The first column in every row is the row number (read-only in the editor).

```
1;Alice;30;Engineer;
2;Bob;25;Designer;
```

## Running Tests

```
dotnet test table_editor.Tests
```

The test suite covers `Cell`, `Row`, `TableModel`, and `FormulaParser` — all pure logic that runs without a GPU.

## Project Structure

```
table_editor/
├── classes/
│   ├── Cell.cs              — Cell model with type detection and display value
│   ├── CellType.cs          — String / Integer / Float / Formula enum
│   ├── Row.cs               — Observable row (ObservableCollection<Cell>)
│   ├── TableModel.cs        — Pure data model (no WPF dependency)
│   ├── CudaDataTable.cs     — WPF wrapper: columns + formula evaluation
│   ├── CudaSorter.cs        — GPU column sort (IColumnSorter)
│   ├── CudaEvaluator.cs     — GPU formula evaluator (IFormulaEvaluator)
│   ├── FormulaParser.cs     — Cell-reference parser (pure, testable)
│   ├── IColumnSorter.cs     — Sorter interface
│   └── IFormulaEvaluator.cs — Evaluator interface
├── windows/
│   └── NewTableWindow.xaml  — New-table dialog
├── table_editor.Tests/      — xUnit test project
├── *.cu / *.ptx             — CUDA kernel sources and compiled binaries
└── icons/                   — Toolbar icons
```

## License

[MIT](LICENSE)
