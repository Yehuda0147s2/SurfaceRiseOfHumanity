using UnityEngine;

namespace SurfaceRiseOfHumanity.Missions
{
    public sealed class MissionObjective : MonoBehaviour
    {
        [SerializeField] private string missionId = "M01_THE_SEALED_PATH";
        [SerializeField] private string objectiveId = "M01_INVESTIGATE_SEALED_PATH";
        [SerializeField] private string objectiveText = "Investigate the sealed path";
        [SerializeField] private bool completeOnInteraction;

        public string MissionId => missionId;
        public string ObjectiveId => objectiveId;
        public string ObjectiveText => objectiveText;
        public bool IsComplete { get; private set; }

        public void Interact()
        {
            if (completeOnInteraction) Complete();
        }

        public void Complete() => IsComplete = true;
        public void RestoreCompleted(bool completed) => IsComplete = completed;
    }
}
