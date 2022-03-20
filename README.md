# MyORMForMySQL

MyORMForMySQL is a implementation of MyORM that uses MySQL as database. 

## Installation

.NET CLI

```bash
dotnet add package Adr.Logs.MyLog.mv --version 2.0.0.1
```

Nuget package manager

```bash
PM> Install-Package Adr.Logs.MyLog.mv -Version 2.0.0.1
```

packageReference

```bash
<PackageReference Include="Adr.Logs.MyLog.mv" Version="2.0.0.1" />
```

## Usage

```csharp
            /*
             * he have diferents instances of log writer for diferents types and values. 
             * he can pass any type and any value and we will have a instance of log to this
             */

            MyLog.TypeOf<int>.Collection[1].WriteLog(message: "some message to save 1"); //use a instance to value 1 

            MyLog.TypeOf<int>.Collection[2].WriteLog(message: "some message to save 2"); //use a instance to value 2

            MyLog.TypeOf<int>.Collection[1].WriteLog(message: "some message to save 1 again"); //use a instance to value 1 again

            MyLog.TypeOf<int>.Collection.DefaultFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"); // set default folder for ALL logs writers

            MyLog.TypeOf<int>.Collection[1].Folder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "logs 1"); //use folder for just one instance of log writer 

            MyLog.TypeOf<int>.Collection.DefaultFileSize = 10; // se default file size for all log writer 

            MyLog.TypeOf<int>.Collection.DefaultMaxFiles = 5;  // se default file count for all log writer 

            MyLog.TypeOf<int>.Collection[2].FullStack = true; //determine if log must write the stacktrace of call

            MyLog.TypeOf<int>.Collection[2].LogType = MyLog.WS.LogsType.WS; // determines the type of log

            MyLog.TypeOf<int>.Collection[2].WSConfig = new MyLog.WS.LogWSConfig  // determines the WS to send logs
            {
                URL = "http://mylogrep.com/", // the URL of WS
                Encoding = Encoding.UTF8, // the encoding of POST body request
                Headers = new List<KeyValuePair<string, string>>(), // headers of request
                ErrorHandler = exception => Console.WriteLine(exception.Message), // error event handler
                Callback = httpResponseMessage => Console.WriteLine(httpResponseMessage.Content.ToString()) // callback event handler
            };

```


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](https://choosealicense.com/licenses/mit/)
