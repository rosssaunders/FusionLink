# FusionLink

FusionLink is a Xll library for Excel and a FusionInvest Toolkit which provides the ability to subscribe to real-time and query data from a FusionInvest client.

## Getting Started

### Prerequisites

1. A Toolkit license for Finastra FusionInvest / FusionCapital.
2. Knowledge of Toolkit development
3. Knowledge of C#, WCF and ExcelDNA development.
3. Visual Studio 2019
4. Excel 2003 or above

### Building the solution

In order to build the solution without access to the SDK dll's, a Mocks library is provided which match the assembly, type and method signatures of the SDK. The FusionLink toolkit when built against this library will successfully run. 

Various build configurations are supplied for versions 7.1.3.14, 7.1.3.19 & 7.2.6 of the SDK. 

*Steps*

1. Clone the Repo to your PC.
2. Open the Client solution within the src folder in VS2019.
3. Build the solution. (The first time will download all the required NuGet packages)
3. Open the Server solution within the src folder in VS2019.
4. Choose the build configuration which matches your SDK.
5. Build the solution. (The first time will download all the required NuGet packages)

## Installation

### Excel Client

1. All the files in the build directory of the "ExcelClient" project need to be distributed to your installation location of choice.
2. Start Excel
3. Open "FusionLink-AddIn.xll" from Excel. 
4. To verify the add-in has loaded successfully, you should see a new ribbon tab "FusionLink".

### Permissions

1. In the FusionLink project under the SQL folder, run all the SQL in Install.sql. This will setup a new permission under "SDK Rights" for each user and group.
2. Enable the "FusionLink" right for the users / groups that require access in the FusionInvest / FusionCapital UI.

### Toolkit

1. All the files in the build directory of the "FusionLink" project need to be placed in your Toolkit dll folder. 
2. In the AssemblyDll section of your config file add the location of the "FusionLink" dll.
3. Load FusionInvest / FusionCapital. If your user has the nessesary permission to "FusionLink" you should be able to see a MFC Caption Bar with the connection information.

## Built With

* [ExcelDNA](https://excel-dna.net/)
* [FusionInvest](https://www.finastra.com/solutions/treasury-and-capital-markets/investment-management/fusion-invest)

## Authors

* [Ross Saunders](https://github.com/rosssaunders) 
* [Debra Saunders](https://github.com/debrasaunders)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* [DG Partners](http://www.dgpartners.co.uk/) for generously letting RXD Solutions use their development environment.
* [Philippe Bonneau](http://www.it-quants.com/Blogs.aspx) for the inspiration on using the MFC Caption Bar within Sophis.

## Build Status
* Client - [![Build status](https://dev.azure.com/rxd/FusionInvest/_apis/build/status/FusionInvest-Client-CI)](https://dev.azure.com/rxd/FusionInvest/_build/latest?definitionId=2)
* Server - [![Build status](https://dev.azure.com/rxd/FusionInvest/_apis/build/status/FusionInvest-Server-CI)](https://dev.azure.com/rxd/FusionInvest/_build/latest?definitionId=1)
