using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace ScanSort.Services;

public class DeskewService : IDeskewService
{
    public float DetectSkewAngle(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            return 0f;

        try
        {
            using var src = Cv2.ImRead(imagePath, ImreadModes.Grayscale);
            if (src.Empty()) return 0f;

            // Apply Gaussian blur to reduce noise
            using var blurred = new Mat();
            Cv2.GaussianBlur(src, blurred, new OpenCvSharp.Size(9, 9), 0);

            // Edge detection
            using var edges = new Mat();
            Cv2.Canny(blurred, edges, 50, 150, 3);

            // Detect lines using probabilistic Hough transform
            var lines = Cv2.HoughLinesP(edges, 1, Math.PI / 180, threshold: 100,
                minLineLength: src.Width / 8.0, maxLineGap: 20);

            if (lines.Length == 0) return 0f;

            // Calculate the median angle of detected lines
            var angles = new List<double>();
            foreach (var line in lines)
            {
                double dx = line.P2.X - line.P1.X;
                double dy = line.P2.Y - line.P1.Y;
                double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;

                // Only consider near-horizontal lines (within ±45 degrees)
                if (Math.Abs(angle) < 45)
                    angles.Add(angle);
            }

            if (angles.Count == 0) return 0f;

            // Use median to be robust against outliers
            angles.Sort();
            double medianAngle = angles[angles.Count / 2];

            return (float)medianAngle;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error detecting skew for {imagePath}: {ex.Message}");
            return 0f;
        }
    }
}
