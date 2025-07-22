# SSRS PDF Report Generator

A C# console application that fetches data from a database, generates SSRS reports in PDF format, merges them into a single document, and compresses the final output using Ghostscript.

## 🧰 Features

- Connects to SQL database and retrieves required data.
- Generates PDF reports from **SSRS** (SQL Server Reporting Services).
- Merges multiple PDF reports into one.
- Compresses the final PDF using **Ghostscript** to reduce size.

## 🏗️ Technologies Used

- C# (.NET Core or .NET Framework)
- SQL Server [SqlClient]
- SSRS Report Execution Web Service
- [PdfSharp] (for merging PDFs)
- Ghostscript

## 🚀 Getting Started

### Prerequisites

- .NET SDK installed
- SSRS endpoint configured and accessible
- Ghostscript installed
- Connection string and report paths configured

### Setup

1. Clone this repo:
   ```bash
   git clone https://github.com/yourusername/ssrs-pdf-generator.git
