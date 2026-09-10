using ConnectionTester.Api.Engine.Live;
using ConnectionTester.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace PMUConnectionTester.Api.Tests.Engine.Live;

/// <summary>
/// Unit-level tests for <see cref="LiveSession.Complete"/> - exercises the
/// CalculatedFrameRate/TotalMissingFrames plumbing directly, independent of GSF's real
/// frame-rate-calculation timing (which <see cref="LiveCaptureEngineTests"/> shows does not populate
/// for TransportProtocol.File playback), so this is what actually proves the wiring is correct.
/// </summary>
[TestClass]
public class LiveSessionTests
{
    [TestMethod]
    public void Complete_NoMeasurements_ProducesEmptyMeasurementSeries()
    {
        LiveSession session = new();

        session.Complete(DateTime.UtcNow, DateTime.UtcNow, captureDurationSeconds: 60, calculatedFrameRate: 59.9, totalMissingFrames: 0);

        Assert.IsTrue(session.TryGetMeasurements(out var series));
        Assert.AreEqual(0, series.Count);
    }

    [TestMethod]
    public void Complete_WithMeasurements_PropagatesCalculatedFrameRateAndTotalMissingFramesToEverySeries()
    {
        LiveSession session = new();
        session.AppendMeasurement(10105, new PmuMeasurementSampleDto());
        session.AppendMeasurement(10106, new PmuMeasurementSampleDto());

        DateTime start = new(2026, 6, 29, 15, 24, 0, DateTimeKind.Utc);
        DateTime end = new(2026, 6, 29, 15, 26, 0, DateTimeKind.Utc);

        session.Complete(start, end, captureDurationSeconds: 120, calculatedFrameRate: 59.87, totalMissingFrames: 3);

        Assert.IsTrue(session.TryGetMeasurements(out var series));
        Assert.AreEqual(2, series.Count);
        Assert.IsTrue(series.All(s => s.CalculatedFrameRate == 59.87));
        Assert.IsTrue(series.All(s => s.TotalMissingFrames == 3));
        Assert.IsTrue(series.All(s => s.FpsWindowSeconds == 120));
        Assert.IsTrue(series.All(s => s.CaptureStartTime == start && s.CaptureEndTime == end));
    }
}
