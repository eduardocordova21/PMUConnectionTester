using System;
using System.Collections.Generic;

namespace ConnectionTester.Api.Models;

/// <summary>
/// Response entry for <c>GET /api/pmuconnectiontester/live/sessions/{sessionId}/measurements</c> -
/// the captured time series for a single device.
/// </summary>
public class PmuMeasurementSeriesDto
{
    /// <summary>
    /// GSF's own live frame-rate calculation for the <see cref="GSF.PhasorProtocols.MultiProtocolFrameParser"/>
    /// connection that served this session (<c>MultiProtocolFrameParser.CalculatedFrameRate</c>),
    /// sampled once right after the connection is stopped. This is the exact same property the
    /// desktop app's status bar displays continuously while connected - unlike <see
    /// cref="ReceivedFrameCount"/> divided by <see cref="FpsWindowSeconds"/> (a single average spread
    /// across the whole fixed capture window, permanently dragged down by any stall anywhere in it),
    /// this reflects GSF's own rolling frame-rate tracking and is what makes an apples-to-apples
    /// comparison against the desktop tool possible. Tracked over the connection's whole lifetime,
    /// not scoped strictly to the "CapturandoDados" phase. Same value duplicated across every device
    /// series of a batch, since they all share one physical connection.
    /// </summary>
    public double CalculatedFrameRate { get; set; }

    /// <summary>
    /// UTC timestamp the capture window ended.
    /// </summary>
    public DateTime CaptureEndTime { get; set; }

    /// <summary>
    /// UTC timestamp the capture window started.
    /// </summary>
    public DateTime CaptureStartTime { get; set; }

    /// <summary>
    /// Length of the capture window, in seconds.
    /// </summary>
    public int FpsWindowSeconds { get; set; }

    /// <summary>
    /// idCode of the device, as a string.
    /// </summary>
    public string IdPmu { get; set; }

    /// <summary>
    /// One entry per data frame received for this device.
    /// </summary>
    public List<PmuMeasurementSampleDto> Measurements { get; set; } = new();

    /// <summary>
    /// Total number of data frames received for this device during the capture window.
    /// </summary>
    public long ReceivedFrameCount { get; set; }

    /// <summary>
    /// Total data frames GSF's protocol-level frame counter detected as missing for this connection
    /// (<c>MultiProtocolFrameParser.TotalMissingFrames</c>, based on sequence/timestamp gaps) -
    /// an authoritative, GSF-native signal that data stopped flowing at some point, independent from
    /// the window-average FPS derived from <see cref="ReceivedFrameCount"/>. Same value duplicated
    /// across every device series of a batch, since they all share one physical connection.
    /// </summary>
    public long TotalMissingFrames { get; set; }
}