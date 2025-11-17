using System;

namespace Encamina.Enmarcha.Agents.Models;

/// <summary>
/// Represents a request to update the state of a live activity.
/// </summary>
/// <param name="LiveActivityId">
/// The unique identifier of the live activity to update.
/// </param>
/// <param name="Title">
/// The main title or description of the live activity.
/// </param>
/// <param name="Subtitle">
/// An optional subtitle providing additional context for the live activity.
/// </param>
/// <param name="Status">
/// The current status of the live activity.
/// </param>
/// <param name="Content">
/// The main content or message of the live activity. Supports markdown.
/// </param>
/// <param name="ProgressPercent">
/// The progress of the live activity as a percentage (0-100). Null if not applicable.
/// </param>
/// <param name="ShowHistory">
/// Indicates whether to show the history of the live activity.
/// </param>
public record LiveActivityUpdateRequest(
  string LiveActivityId,
  string Title,
  string? Subtitle,
  string? Content,
  LiveActivityStatus Status,
  int? ProgressPercent,
  bool ShowHistory = true);
