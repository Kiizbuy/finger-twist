using UnityEngine;

namespace GameFramework.Dialogue
{
    public class AIConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue dialogue = null;
        [SerializeField] private string conversantName;


        public void StartDialogue(PlayerConversant conversant)
        {
            if (dialogue == null)
            {
                return;
            }

            conversant.StartDialogue(this, dialogue);
        }

        public string GetName()
        {
            return conversantName;
        }
    }
}
