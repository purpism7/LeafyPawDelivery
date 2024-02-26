
namespace Game
{
    public class Type
    {
        public enum EGameState
        {
            None,

            Enter,
            Game,
            Edit,
            Screenshot,
        }

        public enum EElementState
        {
            None,

            Play,
            Edit,
        }

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
            Fish,
        }

        public enum EObjectCurrency
        {
            None,

            Leaf,
            Moon,
            Shellfish,
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
            //Stamp,
            Letter,
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
            Cash,
            Advertising,
        }

        public enum ECategory
        {
            None,

            Cash,
            AnimalCurrency,
            ObjectCurrency,
        }

        public enum EBoost
        {
            None,

            TwiceCurrency,
        }

        public enum ETutorialStep
        {
            None,

            HiPopo,
            Start,

            // 상단 
            DescGame,
            DescAnimalCurrency,
            DescObjectCurrency,
            DescLetter,
            GetStarter,

            // 하단
            DescEdit,
            DisappearPopo,
            DisappearPopoEndMove,
            EditAnimal,
            EditObject,
            DescStory,
            DescMap,

            HappyLeafyPawDelivery,

            End,
            ReturnGame,
        }
    }
}


