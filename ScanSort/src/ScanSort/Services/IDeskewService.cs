namespace ScanSort.Services;

public interface IDeskewService
{
    /// <summary>
    /// Detects the skew angle of a scanned image in degrees.
    /// Returns 0 if detection fails.
    /// </summary>
    float DetectSkewAngle(string imagePath);
}
