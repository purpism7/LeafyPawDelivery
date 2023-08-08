using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ConversationBehaviour : PlayableBehaviour
{
    private UI.Conversation _conversationPopup = null;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);

        Debug.Log("ConversationBehaviour OnBehaviourPlay");

        _conversationPopup = new GameSystem.PopupCreator<UI.Conversation, UI.Conversation.Data>()
            .SetCoInit(true)
            .Create();
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);

        Debug.Log("ConversationBehaviour OnBehaviourPause");

        if(_conversationPopup != null)
        {
            _conversationPopup.Deactivate();
        }
    }

    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);

        Debug.Log("ConversationBehaviour OnGraphStop");
    }
}
