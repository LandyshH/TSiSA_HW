namespace Order;

public class CsvFileWriter
{
    public static void WriteDictionaryToCsv(Dictionary<int, int> data, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            writer.WriteLine("Key,Value");

            foreach (var kvp in data)
            {
                writer.WriteLine($"{kvp.Key},{kvp.Value}");
            }
        }

        Console.WriteLine($"Данные успешно записаны в файл: {outputFilePath}");
    }

    public static void WriteHourlyStatsForThreeDaysToCsv(List<Dictionary<int, int>> hourlyStatsList, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            writer.WriteLine("Num,Hour,Requests");

            for (int dayIndex = 0; dayIndex < hourlyStatsList.Count; dayIndex++)
            {
                foreach (var kvp in hourlyStatsList[dayIndex])
                {
                    writer.WriteLine($"{dayIndex + 1},{kvp.Key},{kvp.Value}");
                }
            }
        }

        Console.WriteLine($"Данные успешно записаны в файл: {outputFilePath}");
    }

}
