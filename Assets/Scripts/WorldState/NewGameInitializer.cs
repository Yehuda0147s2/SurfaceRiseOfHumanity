using UnityEngine;

namespace SurfaceRiseOfHumanity.WorldState
{
    public sealed class NewGameInitializer : MonoBehaviour
    {
        [SerializeField] private WorldStateManager worldState;
        private void Start()
        {
            if (worldState == null) worldState = FindObjectOfType<WorldStateManager>();
            if (worldState != null && !worldState.HasSave()) worldState.NewGame();
        }
    }
}
