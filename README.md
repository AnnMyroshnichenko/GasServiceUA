# GasServiceUA
GasServiceUA is a website created to help a local gas company manage its services for people in the community. The main idea behind this project is to make it easier for both the company and its customers to keep track of gas usage, payments, bills, and personal accounts — all in one place and online. The system allows customers to register for an account where they can send in their gas meter readings each month, dowload payments and gas usage reports, and see their current balance. They can also pay their bills via PayPal. The project includes unit tests with xUnit and Moq.

## Key Features

🧑‍💻 User Accounts

🔢 Meter Reading Submissions

💳 Billing & Payments

📄 PDF Reports

✅ Testing


## Build with
- ASP.NET Core MVC
- SQL Server
- HTML, CSS, JavaScript


## Getting Started
To get a local copy up and running follow the steps bellow.

### Clone the repo

``` 
git clone https://github.com/AnnMyroshnichenko/GasServiceUA.git
```

### Create a .env file in the root directory of the project

The project uses PayPal for payments, so you need to visit that website and generate ClientIDs and ClientSecrets. You will also need your apikey to use currencyapi, you can get one https://app.currencyapi.com/register.

Return to your .env file and set environment variables as shown below:

```
PAYPAL_CLIENTID=[your PAYPAL_CLIENTID]
PAYPAL_CLIENTSECRET=[your PAYPAL_CLIENTSECRET]
PAYPAL_MODE=[your PAYPAL_MODE]
CURRENCY_CONVERTER_APIKEY=[your CURRENCY_CONVERTER_APIKEY]
```

Also insert an email address credentials for sending sign up confirmation and password recovery emails

```
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_SENDERNAME=[your SMTP_SENDERNAME]
SMTP_SENDEREMAIL=[your SMTP_SENDEREMAIL]
SMTP_USERNAME=[your SMTP_USERNAME]
SMTP_PASSWORD=[your SMTP_PASSWORD]
```

### Create a SQLServer database

Add the connection string to the .env file:

```
DEFAULT_CONNECTION_STRING=[your connection string]
```

### Apply migrations

Open up a Package Manager Console in Visual Studio and run the following command:

```
Update-Database
```

## Contributing
Your contributions are greatly appreciated. If you have a suggestion to improve this project, please follow these steps to fork the repo and create a pull request:

Fork the Project
Create your Feature Branch (git checkout -b feature/NewFeature)
Commit your Changes (git commit -m 'Add some NewFeature')
Push to the Branch (git push origin feature/NewFeature)
Open a Pull Request

## License
Distributed under the MIT License.
