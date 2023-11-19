using System;
using System.Globalization;

namespace Order;

public class CsvFileReader
{
    private List<DateTime> requests;

    public CsvFileReader(string filePath)
    {
        requests = ReadRequests(filePath);
    }

    private List<DateTime> ReadRequests(string filePath)
    {
        List<DateTime> requests = new List<DateTime>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (DateTime.TryParseExact(line, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime requestTime))
                {
                    requests.Add(requestTime);
                }
                else if (DateTime.TryParseExact(line, "yyyy-MM-dd HH:mm:ss.ff", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime requestTimeAlt))
                {
                    requests.Add(requestTimeAlt);
                }
                else if (DateTime.TryParseExact(line, "yyyy-MM-dd HH:mm:ss.f", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime requestTimeAlt2))
                {
                    requests.Add(requestTimeAlt2);
                }
                else if (DateTime.TryParseExact(line, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime requestTimeAlt3))
                {
                    requests.Add(requestTimeAlt3);
                }
                else
                {
                    Console.WriteLine($"Не удалось распознать дату: {line}");
                }
            }
        }

        return requests;
    }

    public int CountRequestsInYearAndMonth(int year, int month)
    {
        return requests.Count(r => r.Year == year && r.Month == month);
    }


    public List<Dictionary<int, int>> CalculateStatsForEachThreeDays(int yearNumber, int monthNumber)
    {
        List<Dictionary<int, int>> results = new List<Dictionary<int, int>>();
        int daysInMonth = DateTime.DaysInMonth(yearNumber, monthNumber);

        for (int day = 1; day <= daysInMonth - 2; day++)
        {
            int day1 = day;
            int day2 = day + 1;
            int day3 = day + 2;

            Dictionary<int, int> hourlyStats = BuildHourlyStatsForThreeDays(yearNumber, monthNumber, day1, day2, day3);

            results.Add(hourlyStats);
        }

        return results;
    }

    private Dictionary<int, int> BuildHourlyStatsForThreeDays(int yearNumber, int monthNumber, int day1, int day2, int day3)
    {
        Dictionary<int, int> hourlyStats = new Dictionary<int, int>();

        for (int hour = 1; hour <= 24; hour++)
        {
            int countDay1 = BuildHourlyStatsForDay(yearNumber, monthNumber, day1)[hour];
            int countDay2 = BuildHourlyStatsForDay(yearNumber, monthNumber, day2)[hour];
            int countDay3 = BuildHourlyStatsForDay(yearNumber, monthNumber, day3)[hour];


            hourlyStats.Add(hour, countDay1);
            hourlyStats.Add(hour + 24, countDay2);
            hourlyStats.Add(hour + 48, countDay3);
        }

        var sortedHourlyStats = hourlyStats.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        return sortedHourlyStats;
    }

    private Dictionary<int, int> BuildHourlyStatsForDay(int yearNumber, int monthNumber, int day)
    {
        var hourlyStats = new Dictionary<int, int>();

        var groupedRequests = requests
            .Where(r => r.Year == yearNumber && r.Month == monthNumber && r.Day == day)
            .ToArray();

        for (int hour = 0; hour < 24; hour++)
        {
            int count = groupedRequests.Count(r => r.Hour == hour);
            hourlyStats.Add(hour + 1, count);
        }

        return hourlyStats;
    }



    public Dictionary<int, int> BuildDailyMonthStats()
    {
        Dictionary<int, int> dailyStats = new Dictionary<int, int>();

        var groupedRequests = requests
            .Where(r => r.Month == 3 && r.Year == 2023)
            .GroupBy(r => r.Day)
            .ToArray();

        for(var i = 0; i < groupedRequests.Length; i++)
        {
            if (dailyStats.ContainsKey(i + 1))
            {
                dailyStats[i + 1] += groupedRequests[i].Count();
            }
            else
            {
                dailyStats.Add(i + 1, groupedRequests[i].Count());
            }
        }

        return dailyStats;
    }

    public Dictionary<int, int> BuildWeeklyStats(int year, int firstMonth, int secondMonth, int thirdMonth,  DayOfWeek dayOfWeek)
    {
        Dictionary<int, int> weeklyStats = new Dictionary<int, int>();

        var groupedRequests = requests.Where(r => (r.Month == firstMonth || r.Month == secondMonth || r.Month == thirdMonth) 
                                                    && r.DayOfWeek == dayOfWeek && r.Year == year)
                                      .GroupBy(r => CultureInfo.CurrentCulture.Calendar
                                                    .GetWeekOfYear(new DateTime(r.Year, r.Month, r.Day), CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                                      .ToArray();

        for(var i = 0; i < groupedRequests.Length; i++)
        {

            if (weeklyStats.ContainsKey(i+1))
            {
                weeklyStats[i+1] += groupedRequests[i].Count();
            }
            else
            {
                weeklyStats.Add(i+1, groupedRequests[i].Count());
            }
        }

        return weeklyStats;
    }


    public Dictionary<int, int> BuildWeeklyStatsThreeMonths(int year, int firstMonth, int secondMonth, int thirdMonth)
    {
        Dictionary<int, int> weeklyStats = new Dictionary<int, int>();

        var groupedRequests = requests.Where(r => (r.Month == firstMonth || r.Month == secondMonth || r.Month == thirdMonth) && r.Year == year) 
                                      .GroupBy(r => CultureInfo.CurrentCulture.Calendar
                                                    .GetWeekOfYear(new DateTime(r.Year, r.Month, r.Day), CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                                      .ToArray();

        int startWeek = groupedRequests.Min(group => group.Key);

        for (var i = 0; i < groupedRequests.Length; i++)
        {

            if (weeklyStats.ContainsKey(i + 1))
            {
                weeklyStats[i + 1] += groupedRequests[i].Count();
            }
            else
            {
                weeklyStats.Add(i + 1, groupedRequests[i].Count());
            }
        }

        return weeklyStats;
    }

    public Dictionary<int, int> BuildMonthsStats()
    {
        Dictionary<int, int> stats = new Dictionary<int, int>();

        var groupedRequests = requests.GroupBy(r => new { r.Year, r.Month }).ToArray();

        for (var i = 0; i < groupedRequests.Length; i++)
        {

            if (stats.ContainsKey(i + 1))
            {
                stats[i + 1] += groupedRequests[i].Count();
            }
            else
            {
                stats.Add(i + 1, groupedRequests[i].Count());
            }
        }

        return stats;
    }

    public int GetDaysCount()
    {
        var h = requests.GroupBy(r => new { r.Year, r.Month, r.Day }).ToArray();
        return h.Count();
    }
}
