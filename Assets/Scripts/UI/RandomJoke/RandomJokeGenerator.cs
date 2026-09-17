using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SurfaceRiseOfHumanity.UI
{
    /// <summary>
    /// Fetches and displays a random joke from the Official Joke API.
    /// Add this component to a UI object and assign the optional fields in the Inspector.
    /// </summary>
    public sealed class RandomJokeGenerator : MonoBehaviour
    {
        [Serializable]
        private sealed class JokeResponse
        {
            public string type;
            public string setup;
            public string punchline;
        }

        private const string JokeEndpoint = "https://official-joke-api.appspot.com/random_joke";

        [Header("UI")]
        [SerializeField] private Text jokeText;
        [SerializeField] private Text statusText;
        [SerializeField] private Button generateButton;

        [Header("Network")]
        [SerializeField, Min(1)] private int timeoutSeconds = 10;
        [SerializeField] private bool fetchOnStart;

        private bool requestInProgress;

        private void Awake()
        {
            if (generateButton != null)
                generateButton.onClick.AddListener(GetRandomJoke);
        }

        private void Start()
        {
            if (fetchOnStart)
                GetRandomJoke();
        }

        private void OnDestroy()
        {
            if (generateButton != null)
                generateButton.onClick.RemoveListener(GetRandomJoke);
        }

        /// <summary>Can be assigned to a Unity UI Button OnClick event.</summary>
        public void GetRandomJoke()
        {
            if (requestInProgress) return;
            StartCoroutine(FetchJoke());
        }

        private IEnumerator FetchJoke()
        {
            requestInProgress = true;
            SetLoadingState(true);

            using (UnityWebRequest request = UnityWebRequest.Get(JokeEndpoint))
            {
                request.timeout = timeoutSeconds;
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    SetStatus($"Could not load a joke: {request.error}");
                    SetLoadingState(false);
                    requestInProgress = false;
                    yield break;
                }

                JokeResponse response = null;
                try
                {
                    response = JsonUtility.FromJson<JokeResponse>(request.downloadHandler.text);
                }
                catch (Exception error)
                {
                    SetStatus($"Invalid joke response: {error.Message}");
                }

                if (response == null || string.IsNullOrWhiteSpace(response.setup) || string.IsNullOrWhiteSpace(response.punchline))
                {
                    SetStatus("The joke service returned an incomplete joke.");
                }
                else if (jokeText != null)
                {
                    jokeText.text = $"{response.setup}\n\n{response.punchline}";
                    SetStatus("Random joke loaded.");
                }
            }

            SetLoadingState(false);
            requestInProgress = false;
        }

        private void SetLoadingState(bool loading)
        {
            if (generateButton != null)
                generateButton.interactable = !loading;
            if (loading)
                SetStatus("Loading joke...");
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }
    }
}
