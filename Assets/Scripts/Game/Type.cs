
namespace Game
{
    public class Type
    {
        public enum ETab
        {
            Animal,
            Object,
            Story,
            DailyMission,
            Achievement,
        }

        public enum EElement
        {
            None,

            Animal,
            Object,
        }

        public enum EScene
        {
            None,

            Loading,
            Logo,
            Login,
            Game,
        }

        public enum EOpen
        {
            None,

            Animal,
            Object,
            Story,
        }

        public enum EAnimalCurrency
        {
            None,

            Acorn,
        }

        public enum EObjectCurrency
        {
            None,

            Leaf,
        }

        public enum EObjectGrade
        {
            None,

            Unique,
            Epic,
            Rare,
            Normal,
        }

        public enum EAnimalSkin
        {
            None,

            Base,
        }

        public enum EItem
        {
            None,

            Currency,
            Letter,
        }

        public enum EAcquire
        {
            None,

            Animal,
            Object,
            AnimalCurrency,
            ObjectCurrency,
            AnimalSkin,
            Story,
        }

        public enum EAcquireAction
        {
            None,

            Obtain, // 재화 획득, 스토리 최초 보기.
            Use, // 재화 사용, 동물/오브젝트 배치.
        }
    }
}


