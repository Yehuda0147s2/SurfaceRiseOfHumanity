# Random Joke Generator

`RandomJokeGenerator` fetches one random joke from the external [Official Joke API](https://official-joke-api.appspot.com/random_joke) and displays the setup and punchline in Unity UI.

## Unity setup

1. Create a Canvas.
2. Add a `Text` element for the joke and a second optional `Text` element for status messages.
3. Add a `Button` labelled **New Joke**.
4. Add an empty UI GameObject named `RandomJokeGenerator`.
5. Add `Assets/Scripts/UI/RandomJoke/RandomJokeGenerator.cs` to that object.
6. Assign the joke text, status text, and button in the Inspector.
7. Press Play and click **New Joke**.

The script also registers the button listener automatically. Alternatively, leave the button reference empty and connect `GetRandomJoke()` manually through the Button's **On Click** event.

## Network and platform notes

- The implementation uses Unity's `UnityWebRequest` and a 10-second timeout by default.
- An internet connection is required at runtime.
- On Android, verify the project has Internet permission enabled in Player Settings. Unity normally includes it when network APIs are used, but confirm this in the generated build.
- The API response is parsed with `JsonUtility`; only the `setup` and `punchline` fields are required.
- This is an external service and may be unavailable or rate-limited. The UI displays an error instead of blocking the game.
- For production, consider a first-party proxy, caching, content moderation, and a fallback local joke list rather than depending directly on a public API.

## API response shape

```json
{
  "type": "general",
  "setup": "Why did the developer go broke?",
  "punchline": "Because he used up all his cache."
}
```
