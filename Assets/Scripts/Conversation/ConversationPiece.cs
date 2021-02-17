using System.Collections.Generic;
using RPGM.Gameplay;
using UnityEngine;

namespace RPGM.Gameplay
{

    public enum ConversationType
    {
        normal,
        events
    }

    [System.Serializable]
    public class Conversations
    {
       public ConversationType type;

        public string id;
        public string targetID;

        [Multiline]
        public string text;
        // public TalkerData talker;
        //public TalkData.TalkType talkType;
        //public TalkData.FaceType talkFace;
        //�e�N�X�g��\������Ƃ��ɏo����ʉ�
        public AudioClip audio;
        //public Quest quest;
        //�I����
        public List<ConversationOption> options;

        public string eventName;
    }

}