// See https://aka.ms/new-console-template for more information
using Order;


string fileName = "order.csv";
string projectDirectory = Directory.GetCurrentDirectory();
var h = Directory.GetParent(Directory.GetParent(Directory.GetParent(projectDirectory).FullName).FullName).FullName;
string filePath = Path.Combine(h, fileName);

var threeDaysFile = Path.Combine(h, "threeDaysFile.csv");
var daysInMonths = Path.Combine(h, "daysInMonths.csv");
var mondays = Path.Combine(h, "mondays.csv");
var sundays = Path.Combine(h, "sundays.csv");
var weekThreeMonths = Path.Combine(h, "weekThreeMonths.csv");
var allData = Path.Combine(h, "allData.csv");


var csvReader = new CsvFileReader(filePath);

var threeDaysResult = csvReader.CalculateStatsForEachThreeDays(2023, 3);
CsvFileWriter.WriteHourlyStatsForThreeDaysToCsv(threeDaysResult, threeDaysFile);

//var daysInMonthsResult = csvReader.BuildDailyMonthStats();
//CsvFileWriter.WriteDictionaryToCsv(daysInMonthsResult, daysInMonths);

//var mondaysResult = csvReader.BuildWeeklyStats(DayOfWeek.Monday);
//CsvFileWriter.WriteDictionaryToCsv(mondaysResult, mondays);

//var sundaysResult = csvReader.BuildWeeklyStats(DayOfWeek.Sunday);
//CsvFileWriter.WriteDictionaryToCsv(sundaysResult, sundays);

//var weekThreeMonthsResult = csvReader.BuildWeeklyStatsThreeMonths();
//CsvFileWriter.WriteDictionaryToCsv(weekThreeMonthsResult, weekThreeMonths);

//var allDataMonths = csvReader.BuildMonthsStats();
//CsvFileWriter.WriteDictionaryToCsv(allDataMonths, allData);

//кол-во дней за весь датасет
var daycCount = csvReader.GetDaysCount();
