using System.ComponentModel.DataAnnotations;

namespace Encamina.Enmarcha.Agents.Options;

/// <summary>
/// Configuration options for <see cref="Activities.LiveActivityManager"/>.
/// </summary>
public class LiveActivityManagerOptions
{
    /// <summary>
    /// Gets the maximum number of history entries to retain for a live activity.
    /// </summary>
    public required int HistoryLimit { get; init; }

    /// <summary>
    /// Gets the sliding expiration duration for cached live activity entries.
    /// </summary>
    public required TimeSpan CacheSlidingExpiration { get; init; } = TimeSpan.FromHours(3);

    /// <summary>
    /// Gets the JSON template for adaptive cards.
    /// </summary>
    [Required]
    public required string LiveTemplateJson { get; init; } = /*lang=json*/ """
{
  "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
  "type": "AdaptiveCard",
  "version": "1.5",
  "msteams": { "width": "full" },
  "body": [
    {
      "type": "Container",
      "spacing": "Medium",
      "separator": true,
      "$when": "${hasHistory == true}",
      "items": [
        {
          "type": "Container",
          "$data": "${history}",
          "items": [
            {
              "type": "Container",
              "spacing": "Small",
              "separator": true,
              "items": [
                {
                  "type": "ColumnSet",
                  "columns": [
                    {
                      "type": "Column",
                      "width": "auto",
                      "verticalContentAlignment": "Center",
                      "items": [
                        { "type": "TextBlock", "text": "${stepText}", "weight": "Bolder", "color": "accent" }
                      ]
                    },
                    {
                      "type": "Column",
                      "width": "stretch",
                      "verticalContentAlignment": "Center",
                      "items": [
                        { "type": "TextBlock", "text": "${title}", "wrap": true, "size": "Medium", "maxLines": 2 }
                      ]
                    },
                    {
                      "type": "Column",
                      "id": "histDown_${$index}",
                      "width": "auto",
                      "verticalContentAlignment": "Center",
                      "items": [
                        { "type": "TextBlock", "text": "▾", "size": "Large" }
                      ]
                    },
                    {
                      "type": "Column",
                      "id": "histUp_${$index}",
                      "width": "auto",
                      "isVisible": false,
                      "verticalContentAlignment": "Center",
                      "items": [
                        { "type": "TextBlock", "text": "▴", "size": "Large" }
                      ]
                    }
                  ],
                  "selectAction": {
                    "type": "Action.ToggleVisibility",
                    "targetElements": [
                      "histContent_${$index}",
                      "histUp_${$index}",
                      "histDown_${$index}"
                    ]
                  }
                },
                {
                  "type": "Container",
                  "id": "histContent_${$index}",
                  "isVisible": false,
                  "style": "emphasis",
                  "spacing": "Small",
                  "items": [
                    { "type": "TextBlock", "text": "${subtitle}", "wrap": true, "spacing": "None", "$when": "${subtitle != null}" },
                    { "type": "TextBlock", "text": "${content}", "isSubtle": true, "wrap": true, "$when": "${content != null}" }
                  ]
                }
              ]
            }
          ]
        }
      ]
    },

    {
      "type": "Container",
      "style": "emphasis",
      "bleed": true,
      "items": [
        { "type": "TextBlock", "text": "${title}", "size": "Medium", "weight": "Bolder", "wrap": true, "maxLines": 2 },
        { "type": "TextBlock", "text": "${subtitle}", "isSubtle": true, "wrap": true, "spacing": "None", "maxLines": 1, "$when": "${subtitle != null}" }
      ]
    },
    { "type": "TextBlock", "text": "${content}", "wrap": true, "spacing": "Medium", "$when": "${content != null}" },

    {
      "type": "ColumnSet",
      "spacing": "Medium",
      "$when": "${showStatusBar == true}",
      "columns": [
        {
          "type": "Column",
          "width": "auto",
          "verticalContentAlignment": "Center",
          "items": [
            { "type": "TextBlock", "text": "${statusGlyph}", "size": "Large", "$when": "${isRunning != true}" },
            {
              "type": "ProgressRing",
              "$when": "${isRunning == true}",
              "fallback": { "type": "TextBlock", "text": "⏳", "size": "Large" }
            }
          ]
        },
        {
          "type": "Column",
          "width": "stretch",
          "verticalContentAlignment": "Center",
          "items": [
            { "type": "TextBlock", "text": "${statusText}", "weight": "Bolder", "size": "Large", "color": "${statusColor}", "wrap": true }
          ]
        },
        {
          "type": "Column",
          "width": "auto",
          "horizontalAlignment": "Right",
          "verticalContentAlignment": "Center",
          "$when": "${showProgress == true}",
          "items": [
            { "type": "TextBlock", "text": "${progressPercentText}", "size": "Large", "weight": "Bolder", "horizontalAlignment": "Right" }
          ]
        }
      ]
    }
  ],
  "fallbackText": "This card requires Adaptive Cards 1.5."
}
""";
}
