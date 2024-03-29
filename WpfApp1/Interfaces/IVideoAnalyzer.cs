﻿using OxyPlot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrequencyAnalysis.Interfaces
{
    public interface IVideoAnalyzer
    {
        Task<IEnumerable<DataPoint>> Analyze(string convertedImagesPath, bool verticalOnly = false, bool horizontalOnly = false);

        void BreakIntoImages();

        Task<string> ConvertToGrayScale();

        double AnalyzeImage(string bmpPath, bool verticalOnly = false, bool horizontalOnly = false);
    }
}
