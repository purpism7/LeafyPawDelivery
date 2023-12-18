
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

        public enum EBottomType
        {
            None,

            Shop,
            Arrangement,
            Book,
            Acquire,
            Map,
        }

        public enum EPlaceName
        {
            None,

            Palmwoods,
            Moonmount,
            Heartsand,
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
            Star,
        }

        public enum EObjectCurrency
        {
            None,

            Leaf,
            Moon,
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
            Item,
        }

        public enum EItemSub
        {
            None,

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
            Stamp,
        }

        public enum EAcquireAction
        {
            None,

            Obtain, // 재화 획득, 스토리 최초 보기.
            Use, // 재화 사용, 동물/오브젝트 배치.
        }

        public enum EPayment
        {
            None,

            Money,
            Advertising,
        }

        public enum ECategory
        {
            None,

            Cash,
            AnimalCurrency,
            ObjectCurrency,
        }
    }
}


