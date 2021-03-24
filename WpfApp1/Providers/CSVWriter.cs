using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace FrequencyAnalysis
{
    public class CSVWriter : IDisposable
    {
        private CsvWriter csvWriter;
        private readonly string filePath;
        private readonly CsvConfiguration csvHelperConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
        private bool writeStarted;

        public CSVWriter(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public Task WriteRecords<T>(IEnumerable<T> records, StreamWriter sw = null)
        {
            if (records == null) throw new ArgumentNullException(nameof(records));

            if (!writeStarted && sw != null)
            {
                OpenWriter(sw);
            }
            else if (!writeStarted)
            {
                OpenWriter();
            }

            return csvWriter.WriteRecordsAsync(records);
        }

        public async Task Write<T>(T record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            if (!writeStarted)
            {
                OpenWriter();
                csvWriter.WriteHeader<T>();
                csvWriter.NextRecord();
            }

            csvWriter.WriteRecord(record);
            await csvWriter.NextRecordAsync();
        }

        private void OpenWriter()
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (File.Exists(filePath))
                File.Delete(filePath);

            this.csvWriter = new CsvWriter(File.CreateText(filePath), this.csvHelperConfiguration);
            writeStarted = true;
        }

        private void OpenWriter(StreamWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(filePath));

            this.csvWriter = new CsvWriter(writer, this.csvHelperConfiguration);
            writeStarted = true;
        }

        public static (string fileName, string filePath, string compressedFilePath) SetupExportFile(string fileName, string exportFolder)
        {
            var filePath = Path.Combine(exportFolder, $"{fileName}.csv");
            var compressedFilePath = Path.Combine(exportFolder, $"{fileName}.zip");
            if (!string.IsNullOrWhiteSpace(exportFolder) && !Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }

            if (!Directory.GetParent(filePath).Exists)
            {
                Directory.CreateDirectory(Directory.GetParent(filePath).FullName);
            }

            return (fileName, filePath, compressedFilePath);
        }
        public void Dispose() => this.csvWriter?.Dispose();
    }
}
