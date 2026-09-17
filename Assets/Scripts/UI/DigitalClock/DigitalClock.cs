using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace SurfaceRiseOfHumanity.UI
{
    public sealed class DigitalClock : MonoBehaviour
    {
        [Serializable]
        public sealed class ClockDefinition
        {
            public string timeZoneId = "UTC";
            public string label;
        }

        [SerializeField] private List<ClockDefinition> clocks = new List<ClockDefinition>
        {
            new ClockDefinition { timeZoneId = "UTC", label = "UTC" },
            new ClockDefinition { timeZoneId = "America/New_York", label = "New York" },
            new ClockDefinition { timeZoneId = "Europe/London", label = "London" },
            new ClockDefinition { timeZoneId = "Asia/Tokyo", label = "Tokyo" }
        };

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
            if (rowsParent == null) rowsParent = transform;
            BuildRows();
        }

        private void OnEnable() => refreshRoutine = StartCoroutine(RefreshClockLoop());

        private void OnDisable()
        {
            if (refreshRoutine != null) StopCoroutine(refreshRoutine);
            refreshRoutine = null;
        }

        public void Rebuild() => BuildRows();

        public void RefreshNow()
        {
            DateTime utcNow = DateTime.UtcNow;
            for (int i = 0; i < rowLabels.Count; i++)
            {
                if (i >= resolvedZones.Count || resolvedZones[i] == null) continue;
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, resolvedZones[i]);
                string label = string.IsNullOrWhiteSpace(clocks[i].label) ? clocks[i].timeZoneId : clocks[i].label;
                rowLabels[i].text = string.Format(CultureInfo.InvariantCulture, rowFormat, label, localTime.ToString(GetTimeFormat(), CultureInfo.InvariantCulture));
            }
        }

        private IEnumerator RefreshClockLoop()
        {
            while (enabled)
            {
                RefreshNow();
                yield return new WaitForSeconds(refreshInterval);
            }
        }

        private void BuildRows()
        {
            ClearGeneratedRows();
            resolvedZones.Clear();
            if (clocks == null) return;

            foreach (ClockDefinition definition in clocks)
            {
                resolvedZones.Add(ResolveTimeZone(definition.timeZoneId));
                GameObject rowObject = new GameObject("ClockRow", typeof(RectTransform), typeof(Text));
                rowObject.transform.SetParent(rowsParent, false);
                Text row = rowObject.GetComponent<Text>();
                row.font = font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
                row.fontSize = fontSize;
                row.color = textColor;
                row.alignment = TextAnchor.MiddleLeft;
                row.raycastTarget = false;
                row.rectTransform.sizeDelta = new Vector2(640f, Mathf.Max(40f, fontSize * 1.4f));
                rowLabels.Add(row);
            }
            RefreshNow();
        }

        private void ClearGeneratedRows()
        {
            if (rowsParent != null)
                for (int i = rowsParent.childCount - 1; i >= 0; i--) Destroy(rowsParent.GetChild(i).gameObject);
            rowLabels.Clear();
        }

        private string GetTimeFormat()
        {
            if (use24HourClock) return includeSeconds ? "HH:mm:ss" : "HH:mm";
            return includeSeconds ? "h:mm:ss tt" : "h:mm tt";
        }

        private static TimeZoneInfo ResolveTimeZone(string requestedId)
        {
            if (string.IsNullOrWhiteSpace(requestedId)) return TimeZoneInfo.Utc;
            try { return TimeZoneInfo.FindSystemTimeZoneById(requestedId); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }

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
                try { return TimeZoneInfo.FindSystemTimeZoneById(windowsId); }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }

            Debug.LogWarning($"DigitalClock could not resolve '{requestedId}'. Falling back to UTC.");
            return TimeZoneInfo.Utc;
        }
    }
}
