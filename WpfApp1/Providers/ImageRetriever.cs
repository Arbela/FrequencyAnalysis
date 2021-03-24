using FrequencyAnalysis.Helpers;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System;

namespace FrequencyAnalysis
{
    public class ImageRetriever
    {
        private const int millisecondsInMinute = 1000;

        public ImageRetriever() { }

        public void RetrieveImages(string mp4FilePath, string outputPath, int framesPerSec = 1)
        {
            using (var engine = new Engine())
            {
                var mp4 = new MediaFile { Filename = mp4FilePath };

                engine.GetMetadata(mp4);

                for (int i = 0; i < mp4.Metadata.Duration.TotalMilliseconds; i+= millisecondsInMinute / framesPerSec)
                {
                    var options = new ConversionOptions { Seek = TimeSpan.FromMilliseconds(i)};
                    var outputFile = new MediaFile { Filename = string.Format($"{Constants.ImageName}{Constants.JpgExt}", outputPath, $"{i / millisecondsInMinute}_{i % millisecondsInMinute}") };
                    engine.GetThumbnail(mp4, outputFile, options);
                }
            }
        }
    }
}
