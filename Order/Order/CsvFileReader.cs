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

    public Dictionary<int, int> BuildHourlyStatsForThreeDays(int yearNumber, int monthNumber)
    {
        Dictionary<int, int> hourlyStats = new Dictionary<int, int>();

        var groupedRequests = requests
                                      .Where(r => r.Year == yearNumber && r.Month == monthNumber)
                                      .ToArray();
  

        for(var i = 0; i < groupedRequests.Length; i++)
        {

        }

        //foreach (var group in groupedRequests)
        //{
        //    var hourlyGroupedRequests = group.GroupBy(r => r.Hour);

        //    foreach (var hourlyGroup in hourlyGroupedRequests)
        //    {
        //        int hour = hourlyGroup.Key;

        //        if (hourlyStats.ContainsKey(hour))
        //        {
        //            hourlyStats[hour] += hourlyGroup.Count();
        //        }
        //        else
        //        {
        //            hourlyStats.Add(hour, hourlyGroup.Count());
        //        }
        //    }
        //}

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

    public Dictionary<int, int> BuildWeeklyStats(DayOfWeek dayOfWeek)
    {
        Dictionary<int, int> weeklyStats = new Dictionary<int, int>();

        var groupedRequests = requests.Where(r => (r.Month == 2 || r.Month == 3 || r.Month == 4) && r.DayOfWeek == dayOfWeek && r.Year == 2023)
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


    public Dictionary<int, int> BuildWeeklyStatsThreeMonths()
    {
        Dictionary<int, int> weeklyStats = new Dictionary<int, int>();

        // Группировка заявок по неделям за три месяца
        // Выбираем данные за март, апрель и февраль
        var groupedRequests = requests.Where(r => (r.Month == 2 || r.Month == 3 || r.Month == 4) && r.Year == 2023) 
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
