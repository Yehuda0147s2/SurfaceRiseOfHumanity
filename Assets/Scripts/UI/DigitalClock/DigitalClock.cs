using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace SurfaceRiseOfHumanity.UI
{
    /// <summary>
    /// Displays live digital clocks for multiple time zones.
    /// Add this component to a Canvas or an empty UI GameObject with a RectTransform.
    /// The component creates one Text row per configured time zone at runtime.
    /// </summary>
    public sealed class DigitalClock : MonoBehaviour
    {
        [Serializable]
        public sealed class ClockDefinition
        {
            [Tooltip("IANA time-zone ID, for example Europe/London or Asia/Tokyo.")]
            public string timeZoneId = "UTC";

            [Tooltip("Optional label. If empty, the time-zone ID is used.")]
            public string label;
        }

        [Header("Time zones")]
        [SerializeField]
        private List<ClockDefinition> clocks = new List<ClockDefinition>
        {
            new ClockDefinition { timeZoneId = "UTC", label = "UTC" },
            new ClockDefinition { timeZoneId = "America/New_York", label = "New York" },
            new ClockDefinition { timeZoneId = "Europe/London", label = "London" },
            new ClockDefinition { timeZoneId = "Asia/Tokyo", label = "Tokyo" }
        };

        [Header("Display")]
        [SerializeField] private Transform rowsParent;
        [SerializeField] private Font font;
        [SerializeField] private int fontSize = 32;
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private bool use24HourClock = true;
        [SerializeField] private bool includeSeconds = true;
        [SerializeField, Min(0.05f)] private float refreshInterval = 0.25f;
        [SerializeField] private string rowFormat = "{0}: {1}";

        private readonly List<Text> rowLabels = new List<Text>();
        private readonly List<TimeZoneInfo> resolvedZones = new List<TimeZoneInfo>();
        private Coroutine refreshRoutine;

        private void Awake()
        {
            if (rowsParent == null)
                rowsParent = transform;

            BuildRows();
        }

        private void OnEnable()
        {
            refreshRoutine = StartCoroutine(RefreshClockLoop());
        }

        private void OnDisable()
        {
            if (refreshRoutine != null)
            {
                StopCoroutine(refreshRoutine);
                refreshRoutine = null;
            }
        }

        /// <summary>Rebuilds the generated UI after changing clock definitions at runtime.</summary>
        public void Rebuild()
        {
            BuildRows();
            RefreshNow();
        }

        /// <summary>Forces an immediate update without waiting for the refresh interval.</summary>
        public void RefreshNow()
        {
            DateTime utcNow = DateTime.UtcNow;

            for (int i = 0; i < rowLabels.Count; i++)
            {
                if (i >= resolvedZones.Count || resolvedZones[i] == null)
                    continue;

                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, resolvedZones[i]);
                string time = localTime.ToString(GetTimeFormat(), CultureInfo.InvariantCulture);
                string label = clocks[i].string.IsNullOrWhiteSpace() ? clocks[i].timeZoneId : clocks[i].label;
                rowLabels[i].text = string.Format(CultureInfo.InvariantCulture, rowFormat, label, time);
            }
        }

        private IEnumerator RefreshClockLoop()
        {
            WaitForSeconds wait = new WaitForSeconds(refreshInterval);

            while (enabled)
            {
                RefreshNow();
                yield return wait;
            }
        }

        private void BuildRows()
        {
            ClearGeneratedRows();
            resolvedZones.Clear();

            if (clocks == null)
                return;

            for (int i = 0; i < clocks.Count; i++)
            {
                ClockDefinition definition = clocks[i];
                TimeZoneInfo zone = ResolveTimeZone(definition.timeZoneId);
                resolvedZones.Add(zone);

                GameObject rowObject = new GameObject("Clock_" + i, typeof(RectTransform), typeof(Text));
                rowObject.transform.SetParent(rowsParent, false);

                Text row = rowObject.GetComponent<Text>();
                row.font = font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
                row.fontSize = fontSize;
                row.color = textColor;
                row.alignment = TextAnchor.MiddleLeft;
                row.horizontalOverflow = HorizontalWrapMode.Overflow;
                row.verticalOverflow = VerticalWrapMode.Overflow;
                row.raycastTarget = false;

                RectTransform rect = row.rectTransform;
                rect.sizeDelta = new Vector2(640f, Mathf.Max(40f, fontSize * 1.4f));

                rowLabels.Add(row);
            }

            RefreshNow();
        }

        private void ClearGeneratedRows()
        {
            for (int i = rowsParent != null ? rowsParent.childCount - 1 : -1; i >= 0; i--)
                Destroy(rowsParent.GetChild(i).gameObject);

            rowLabels.Clear();
        }

        private string GetTimeFormat()
        {
            if (use24HourClock)
                return includeSeconds ? "HH:mm:ss" : "HH:mm";

            return includeSeconds ? "h:mm:ss tt" : "h:mm tt";
        }

        private static TimeZoneInfo ResolveTimeZone(string requestedId)
        {
            if (string.IsNullOrWhiteSpace(requestedId))
                return TimeZoneInfo.Utc;

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(requestedId);
            }
            catch (TimeZoneNotFoundException)
            {
                // Android/Linux use IANA IDs while Windows commonly uses Windows IDs.
                string windowsId = requestedId switch
                {
                    "UTC" => "UTC",
                    "America/New_York" => "Eastern Standard Time",
                    "America/Los_Angeles" => "Pacific Standard Time",
                    "Europe/London" => "GMT Standard Time",
                    "Europe/Paris" => "Romance Standard Time",
                    "Asia/Tokyo" => "Tokyo Standard Time",
                    "Australia/Sydney" => "AUS Eastern Standard Time",
                    _ => null
                };

                if (!string.IsNullOrEmpty(windowsId))
                {
                    try
                    {
                        return TimeZoneInfo.FindSystemTimeZoneById(windowsId);
                    }
                    catch (TimeZoneNotFoundException) { }
                    catch (InvalidTimeZoneException) { }
                }
            }
            catch (InvalidTimeZoneException) { }

            Debug.LogWarning($"DigitalClock could not resolve time zone '{requestedId}'. Falling back to UTC.", this);
            return TimeZoneInfo.Utc;
        }
    }
}
