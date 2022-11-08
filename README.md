# Simple Stocks API
API for a social media network based on the stock market. Built with ASP.Net, Entity Framework, and SQL

Technologies Used: -C# -.NET 6.0 -Entity Framework -SQL Database -Automapper -JWT

Build with npm

Requires connection string "StocksCS" and a JWT Secret

API is code first so be sure to add migrations and update database

User uploads are stored in the "Media" folder

Most endpoints are secure and require a valid Bearer Token

Refresh endpoint checks for header called "RefreshToken" to retrieve the token
